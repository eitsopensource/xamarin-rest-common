using xamarinrest.Configuration;
using xamarinrest.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;

namespace xamarinrest.Services.Rest
{
    public class RestHolder<T>
    {
        //self instance singleton para RestEntityHolder, que auxilia a mexer com as Uri's REST
        public static RestHolder<T> instance = null;

        private string _syncUri;
        private string _syncDeletedUri;
        
        private string _insertUri;
        private string _updateUri;
        private string _deleteUri;

        public RestHolder()
        {
            instance = instance ?? this;
        }

        public bool LockSyncThread { get; set; }
        public bool LockSyncDeletedThread { get; set; }

        public string InsertUri
        {
            get => _insertUri ?? typeof(T).Name.ToLower() + "/" + "insert";
            set => _insertUri = value;
        }

        public string UpdateUri
        {
            get => _updateUri ?? typeof(T).Name.ToLower() + "/" + "update";
            set => _updateUri = value;
        }

        public string DeleteUri
        {
            get => _deleteUri ?? typeof(T).Name.ToLower() + "/" + "delete";
            set => _deleteUri = value;
        }

        public string SyncUri
        {
            get => _syncUri ?? typeof(T).Name.ToLower() + "/" + "merge";
            set => _syncUri = value;
        }

        public string SyncDeletedUri
        {
            get => _syncDeletedUri ?? typeof(T).Name.ToLower() + "/" + "mergeDeleted";
            set => _syncDeletedUri = value;
        }
    }
}
