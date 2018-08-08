using Newtonsoft.Json;
using SQLite;

namespace xamarinrest.Models
{
    public class Empresa : AbstractEntity
    {
        private string _nome;
        private string _cnpj;

        [JsonProperty("nome")] //This maps the element title of your web service to your model
        [MaxLength(144), Column("nome")]
        public string Nome
        {
            get => _nome;
            set
            {
                _nome = value;
                OnPropertyChanged(); //This notifies the View or ViewModel that the value that a property in the Model has changed and the View needs to be updated.
            }
        }

        [JsonProperty("cnpj")] //This maps the element title of your web service to your model
        [MaxLength(144), Column("cnpj")]
        public string Cnpj
        {
            get => _cnpj;
            set
            {
                _cnpj = value;
                OnPropertyChanged(); //This notifies the View or ViewModel that the value that a property in the Model has changed and the View needs to be updated.
            }
        }
    }
}
