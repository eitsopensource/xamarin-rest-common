using xamarinrest.Models;
using xamarinrest.Services.Rest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace xamarinrest.Services
{
    public class RestService
    {
        public static string Url = "http://localhost:8080/";
        public static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Autentica o usuário na Uri definida
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<T> Authenticate<T>( string username, string password, string uri ) where T : new()
        {
            var byteArray = Encoding.ASCII.GetBytes( username + ":" + password );
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic", Convert.ToBase64String( byteArray ) );

            HttpResponseMessage response = await _client.GetAsync( Url + uri );
            HttpContent content = response.Content;

            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);

        }

        /// <summary>
        /// Generic method to send entity with rest HTTP PUT/POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<T> Send<T>( string uri, Object entity ) where T : new ()
        {
            string content = JsonConvert.SerializeObject(entity);
            StringContent restContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync( Url + uri, restContent );
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>( result );
        }

        /// <summary>
        /// Generic method to delete with rest HTTP DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public static async void Delete<T>( long id ) where T : new()
        {
            var holder = RestHolder<T>.instance;
            StringBuilder fullUri = new StringBuilder();
            fullUri = fullUri.AppendFormat(Url + holder.DeleteUri, id);
            await _client.DeleteAsync( fullUri.ToString() );
        }

        /// <summary>
        /// Generic method to rest HTTP GET
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync( string uri )
        {
            string content = await _client.GetStringAsync(Url + uri);
            return content;
        }
        
        /// <summary>
        /// Generic method to rest HTTP GET
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<T> GetEntityAsync<T>( string uri ) where T : new()
        {
            HttpResponseMessage response = await _client.GetAsync(Url + uri);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
