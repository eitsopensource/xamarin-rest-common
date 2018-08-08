using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace xamarinrest.Models
{
    public class AbstractEntity : INotifyPropertyChanged
    {
        protected internal long? _id = null;
        protected internal DateTime? _created = null;

        [JsonProperty("id")] //This maps the element title of your web service to your model
        [PrimaryKey, Column("id")]
        public long? Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(); //This notifies the View or ViewModel that the value that a property in the Model has changed and the View needs to be updated.
            }
        }

        [JsonProperty("created")]
        [Column("created")]
        public DateTime? Created
        {
            get => _created;
            set => _created = value;
        }

        //This is how you create your OnPropertyChanged() method
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
