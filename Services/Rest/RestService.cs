using xamarinrest.Models;
using xamarinrest.Services.Rest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using xamarinrest.Configuration;

namespace xamarinrest.Services
{
    public class RestService
    {
        public static string Url = "http://localhost:8080/";
        public static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<string> Authenticate( string username, string password, string uri )
        {
            var byteArray = Encoding.ASCII.GetBytes("mobileapp:eits2018");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic", Convert.ToBase64String( byteArray ) );
    
            StringContent restContent = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync( Url + uri + "?grant_type=password&username=" + username + "&password=" + password, restContent);

            if ( response.IsSuccessStatusCode )
            {
                String token = JObject.Parse(await response.Content.ReadAsStringAsync())["access_token"].ToString();
                SetOAuthToken(token);
                return token;
            }
            
            return null;              
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="token"></param>
        public static void SetOAuthToken( string token )
        {
            Prefs.putString("token", token);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
          
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
            return await _client.GetStringAsync(Url + uri);
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
