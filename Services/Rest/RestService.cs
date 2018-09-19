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
using xamarinrest.Services.Rest.Exceptions;
using System.Text.RegularExpressions;

namespace xamarinrest.Services
{
    public class RestService
    {
        public static string Url = "http://127.0.0.1:8080/";
        public static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static void Authenticate( string username, string password, string uri, Action<string> onSuccess, Action<RestException> onFailure = null )
        {
            var byteArray = Encoding.ASCII.GetBytes("mobileapp:eits2018");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Basic", Convert.ToBase64String( byteArray ) );
    
            string completeUri = uri + "?grant_type=password&username=" + username + "&password=" + password;
            PostAsync( completeUri, null, 
                //onSuccess
                ( response, json ) => {
                    String token = JObject.Parse( json )["access_token"].ToString();
                    SetOAuthToken(token);
                    onSuccess.Invoke( token );
                },

                //onFailure
                ( exception ) => {
                    if( onFailure != null ) onFailure.Invoke( exception );
                });          
        }

        /// <summary>
        ///  SetOAuthToken
        /// </summary>
        /// <param name="token"></param>
        public static void SetOAuthToken( string token )
        {
            Prefs.putString( "token", token );
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", token );
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
        /// Generic 'Get' RESTful API implementation
        /// </summary>
        /// <typeparam name="T">Class to deserialize response result</typeparam>
        /// <param name="uri">RESTful API Path</param>
        /// <param name="onSuccess">Function to invoke on HTTP status 200 (ok)</param>
        /// <param name="onFailure">Function to invoke on HTTP status not 200 (failure)</param>
        /// <returns></returns>
        public static void Get<T>( string uri, Action<T> onSuccess, Action<RestException> onFailure = null )
        {            
            GetAsync( uri,
                //onSuccess
                ( response, json ) => {
                    onSuccess.Invoke( JsonConvert.DeserializeObject<T>( json ) );
                },

                //onFailure
                ( exception ) => {
                    if ( onFailure != null ) onFailure.Invoke( exception );
                });
        }

        /// <summary>
        /// Generic 'Post' RESTful API implementation
        /// </summary>
        /// <typeparam name="T">Class to deserialize response result</typeparam>
        /// <param name="uri">RESTful API Path</param>
        /// <param name="onSuccess">Function to invoke on HTTP status 200 (ok)</param>
        /// <param name="onFailure">Function to invoke on HTTP status not 200 (failure)</param>
        /// <returns></returns>
        public static void Send<T>( string uri, T entity, Action<T> onSuccess, Action<RestException> onFailure ) where T : new ()
        {
            PostAsync( uri, entity,
                //onSuccess
                ( response, json ) => {
                    onSuccess.Invoke( JsonConvert.DeserializeObject<T>( json, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects } ) );
                },

                //onFailure
                ( exception ) => {
                    if ( onFailure != null ) onFailure.Invoke( exception );
                });
        }

        /// <summary>
        /// Exception safe method for GET ASYNC
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async void GetAsync( string uri, Action<HttpResponseMessage, string> onSuccess, Action<RestException> onFailure = null )
        {           
            try
            {
                HttpResponseMessage response = await _client.GetAsync( Url + uri );
                var json = await response.Content.ReadAsStringAsync();

                if ( response.IsSuccessStatusCode )
                {
                    onSuccess.Invoke( response, SanitizeJSON( json ) );
                }
                else
                {
                    var exceptionResult = JsonConvert.DeserializeObject<RestException>( json );
                    if ( onFailure != null ) onFailure.Invoke( exceptionResult );
                }
            }
            catch ( Exception e )
            {
                var json = JsonConvert.SerializeObject( e.GetBaseException() );
                Console.WriteLine( e.Message + "\n" + e.GetBaseException().Message );

                var exceptionResult = JsonConvert.DeserializeObject<RestException>( json );
                if ( onFailure != null ) onFailure.Invoke( exceptionResult );
            }
        }

        /// <summary>
        /// Exception safe method for POST ASYNC
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="restContent"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFailure"></param>
        private static async void PostAsync( string uri, object entity, Action<HttpResponseMessage, string> onSuccess, Action<RestException> onFailure = null )
        {
            try
            {
                string content = entity != null ? JsonConvert.SerializeObject( entity ) : "";
                StringContent restContent = new StringContent( content, Encoding.UTF8, "application/json" );

                HttpResponseMessage response = await _client.PostAsync( Url + uri, restContent );
                var json = await response.Content.ReadAsStringAsync();

                if ( response.IsSuccessStatusCode )
                {
                    onSuccess.Invoke( response, SanitizeJSON( json ) );
                }
                else
                {
                    var exceptionResult = JsonConvert.DeserializeObject<RestException>( json );
                    if (onFailure != null) onFailure.Invoke( exceptionResult );
                }
            }
            catch( Exception e )
            {
                var json = JsonConvert.SerializeObject( e.GetBaseException() );
                Console.WriteLine( e.StackTrace );

                var exceptionResult = JsonConvert.DeserializeObject<RestException>( json );
                if ( onFailure != null ) onFailure.Invoke( exceptionResult );
            }
        }

        public static string SanitizeJSON( string originalJSONFromJava )
        {
            // Get ID right from Jackson to JSON.Net
            string pattern = "\"@id\":" + "\"(\\S{8}-\\S{4}-\\S{4}-\\S{4}-\\S{12})\"";
            string replacement = "\"$id\":\"$1\"";
            Regex rgx = new Regex(pattern);
            string output = rgx.Replace(originalJSONFromJava, replacement);

            // Convert Jackson reference in array
            //pattern = @",(\d+)";
            //replacement = @",{""$ref"":""$1""}";
            //rgx = new Regex(pattern);
            //output = rgx.Replace(output, replacement);

            // Convert single Jackson reference to ref
            pattern = "\"(?!PastaRepositorio)\\w+\":" + "\"(\\S{8}-\\S{4}-\\S{4}-\\S{4}-\\S{12})\"";
            replacement = "\"$ref\":" + "\"$1\"";
            rgx = new Regex(pattern);
            output = rgx.Replace(output, replacement);

            return output;
        }
    }
}
