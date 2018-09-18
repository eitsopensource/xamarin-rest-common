﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamarinrest.Configuration;
using xamarinrest.Database;
using xamarinrest.Models;

namespace xamarinrest.Services.Rest
{
    public class SyncService
    {
        private static readonly TimeSpan defaultSyncTimeSpan = new TimeSpan( 0, 0, 20 );
        private static readonly TimeSpan defaultSyncDeleteTimeSpan = new TimeSpan( 0, 0, 20 );

        //Faz com que seja chamado o REST no endereço de SYNC
        public static void Sync<T>() where T : new ()
        {
            Task.Run(() => {
                RequestRestAndSync<T>();
            });
        }

        //Faz com que seja chamado o REST no endereço de SYNC a cada X segundos
        public static void StartAutoSync<T>( string syncUri, TimeSpan? syncTimeSpan = null ) where T : new()
        {
            new RestHolder<T>();
            RestHolder<T>.instance.SyncUri = syncUri;

            Task.Run(() => {
                RequestRestAndSync<T>();
            });

            var holder = RestHolder<T>.instance;
            Device.StartTimer( syncTimeSpan ?? defaultSyncTimeSpan, () => {

                Task.Run(() => {
                    RequestRestAndSync<T>();
                });

                return true; //restart timer
            });
        }

        //Requisita o endereço REST para Sincronizar a classe específica no banco SQLite local
        private static void RequestRestAndSync<T>() where T : new()
        {
            var holder = RestHolder<T>.instance;
            if ( holder.LockSyncThread ) return;
            holder.LockSyncThread = true;
            
            //Pega o DateTime da ultima requisição desta Uri
            DateTime lastRequest = Prefs.getDateTime( holder.SyncUri );

            //Request e Sync
            long unixTimestamp = lastRequest.Ticks - new DateTime(1970, 1, 1).Ticks;

            Stopwatch stopwatch = new Stopwatch();
            StringBuilder log = new StringBuilder();

            stopwatch.Start();

            RestService.GetAsync( holder.SyncUri + (unixTimestamp / TimeSpan.TicksPerMillisecond),
                //onSuccess
                ( response, json ) => {
                    log = log.AppendFormat("\n-------- REST REQUEST TIME <{1}> {0} ------- ", stopwatch.Elapsed, typeof(T).Name);

                    stopwatch.Restart();
                    List<T> entities = JsonConvert.DeserializeObject<List<T>>( json );
                    log = log.AppendFormat("\n-------- Deserialize TIME <{1}> {0} ------- ", stopwatch.Elapsed, typeof(T).Name);

                    stopwatch.Restart();
                    SQLiteRepository.SyncEntities<T>(entities);
                    log = log.AppendFormat("\n-------- SYNC TIME <{1}> {0} -------", stopwatch.Elapsed, typeof(T).Name);

                    //Seta o DateTime da ultima requisição para AGORA
                    Prefs.setDateTime(holder.SyncUri, DateTime.Now);
                    holder.LockSyncThread = false;
                    Debug.WriteLine(log);
                },

                //onFailure
                ( e ) => {
                    holder.LockSyncThread = false;
                });
        }

        //Faz com que seja chamado o REST no endereço de SYNC DELETED a cada X segundos
        public static void StartAutoSyncDeleted<T>( string deleteUri, TimeSpan? syncDeleteTimeSpan = null ) where T : new()
        {
            new RestHolder<T>();
            RestHolder<T>.instance.DeleteUri = deleteUri;

            Task.Run(() => {
                RequestRestAndSyncDeleted<T>();
            });

            var holder = RestHolder<T>.instance;
            Device.StartTimer( syncDeleteTimeSpan ?? defaultSyncDeleteTimeSpan, () => { 

                Task.Run(() => {
                    RequestRestAndSyncDeleted<T>();
                });

                return true; //restart timer
            });
        }

        //Requisita o endereço REST para Sincronizar a classe específica no banco SQLite local
        private static void RequestRestAndSyncDeleted<T>() where T : new()
        {
            var holder = RestHolder<T>.instance;
            if ( holder.LockSyncDeletedThread ) return;
            holder.LockSyncDeletedThread = true;

            //Pega o DateTime da ultima requisição desta Uri
            DateTime lastRequest = Prefs.getDateTime( holder.SyncDeletedUri );

            //Request e Sync
            long unixTimestamp = lastRequest.Ticks - new DateTime(1970, 1, 1).Ticks;

            Stopwatch stopwatch = new Stopwatch();
            StringBuilder log = new StringBuilder();
            stopwatch.Start();

            RestService.GetAsync( holder.SyncDeletedUri + ( unixTimestamp / TimeSpan.TicksPerMillisecond ),
                //onSuccess
                ( response, json ) => {
                    log = log.AppendFormat("\n-------- REST REQUEST TIME <{1}> {0} ------- ", stopwatch.Elapsed, typeof(T).Name);

                    stopwatch.Restart();
                    List<long> deletedIds = JsonConvert.DeserializeObject<List<long>>( json );
                    log = log.AppendFormat("\n-------- Deserialize TIME <{1}> {0} ------- ", stopwatch.Elapsed, typeof(T).Name);

                    stopwatch.Restart();
                    SQLiteRepository.SyncDeletedEntities<T>( deletedIds );
                    log = log.AppendFormat("\n-------- SYNC TIME <{1}> {0} -------", stopwatch.Elapsed, typeof(T).Name);

                    //Seta o DateTime da ultima requisição para AGORA
                    Prefs.setDateTime( holder.SyncDeletedUri, DateTime.Now );
                    holder.LockSyncDeletedThread = false;

                    Debug.WriteLine(log);
                },

                //onFailure
                ( e ) => {
                    holder.LockSyncDeletedThread = false;
                });
        }
    }
}
