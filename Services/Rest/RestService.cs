using xamarinrest.Models;
using xamarinrest.Services.Rest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xamarinrest.Services
{
    public class RestService
    {
        public static readonly string Url = "http://192.168.20.13:8080/";
        public static readonly HttpClient _client = new HttpClient();

        public static void Init()
        {
            //Obrigatório; Cria a configuração de REST para a entidade T especificada em <T>
            var pessoaHolder = new RestHolder<Pessoa> {
                SyncUri = "pessoa/merge?date=",
                SyncDeletedUri = "pessoa/mergeDeleted?date=",
                InsertUri = "pessoa/insert",
                UpdateUri = "pessoa/update",
                DeleteUri = "pessoa/delete/{0}"
            };
            var empresaHolder = new RestHolder<Empresa> {
                SyncUri = "empresa/merge?date=",
                InsertUri = "empresa/insert",
                UpdateUri = "empresa/update",
                DeleteUri = "empresa/delete/{0}"
            };
        }

        //generic method to send entity with rest HTTP PUT/POST
        public static async Task<T> Send<T>( string fullUri, Object entity ) where T : new ()
        {
            string content = JsonConvert.SerializeObject(entity);
            StringContent restContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync( fullUri, restContent );
            return JsonConvert.DeserializeObject<T>(response.Content.ToString());
        }

   
        //generic method to delete with rest HTTP DELETE
        public static async void Delete<T>( long id ) where T : new()
        {
            var holder = RestHolder<T>.instance;
            StringBuilder fullUri = new StringBuilder();
            fullUri = fullUri.AppendFormat(Url + holder.DeleteUri, id);
            await _client.DeleteAsync( fullUri.ToString() );
        }

        //generic method to rest HTTP GET
        public static async Task<string> GetAsync( string uri )
        {
            string content = await _client.GetStringAsync(Url + uri);
            return content;
        }
    }
}
