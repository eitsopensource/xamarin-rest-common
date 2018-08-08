using xamarinrest.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace xamarinrest.Database
{
    public class SQLiteRepository
    {
        private static readonly string dbPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal ), "appdatabase.db" );
        private static readonly SQLiteConnection db = new SQLiteConnection( dbPath );
        
        public static void Init()
        {
            Console.WriteLine("Creating database, if it doesn't already exist");
            db.CreateTable<Pessoa>();
            db.CreateTable<Empresa>();
        }
        
        //Método para dar merge na list de entidades
        public static async void SyncEntities<T>( List<T> entityList ) where T : new()
        {
            //Insere todas as entidades async passadas por parametro
            var rowsAffectedAll = 0;

            foreach (T entity in entityList)
            {
                var rowsAffected = 0;
                try //Quando a requisição tenta inserir uma entidade deletada ocorre um erro de constraint
                {
                    rowsAffected = await Task.FromResult(db.Update(entity));
                    if (rowsAffected == 0) rowsAffected = await Task.FromResult(db.Insert(entity));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                rowsAffectedAll += rowsAffected;
            }
            

            if ( rowsAffectedAll > 0 )
            {
                foreach (Subscription<T> subscription in Subscription<T>.subscriptionDictionary.Values)
                {
                    subscription.Callback.Invoke();
                }
            }
        }

        //Apaga do cache. Remove do cache local as entidades que foram deletadas na nuvem
        public static async void SyncDeletedEntities<T>( List<long> deletedIds ) where T : new()
        {
            var rowsAffected = 0;
            foreach ( long entityId in deletedIds )
            {
                rowsAffected += await Task.FromResult( db.Delete<T>( entityId ) );
            }

            if (rowsAffected > 0)
            {
                foreach (Subscription<T> subscription in Subscription<T>.subscriptionDictionary.Values)
                {
                    subscription.Callback.Invoke();
                }
            }
        }

        //Retorna uma lista do que foi descrito na query
        public static async Task<ObservableCollection<T>> Query<T>( string query ) where T : new()
        {
            return await Task.FromResult( new ObservableCollection<T>( db.Query<T>( query ) ) );      
        }

        //Retorna a entidade com o Id especificado
        public static async Task<T> FindById<T>( long id ) where T : new()
        {
            return await Task.FromResult( db.Get<T>( id ) );
        }
    }
}
