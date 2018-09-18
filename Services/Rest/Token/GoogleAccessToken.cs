using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xamarinrest.Services.Rest.Token
{
    public class GoogleAccessToken
    {
        //CONFIG Properties 
        private static readonly string ClientId = "443473907772-3e0labvjo8vlap2to61nh829qg5p8chg.apps.googleusercontent.com";
        public static readonly string ClientSecret = "kDhhgCrwQnjLU5fRD9prPL-w";
        public static readonly string RedirectUri = "https://localhost";

        //Google login form URI
        public static readonly string _authRequest =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            "?response_type=code" +
            "&scope=openid" +
            "&redirect_uri={0}" +
            "&client_id={1}";

        //request access token URI
        public static readonly string _requestUri =
            "https://www.googleapis.com/oauth2/v4/token" +
            "?code={0}" +
            "&client_id={1}" +
            "&client_secret={2}" +
            "&redirect_uri={3}" +
            "&grant_type=authorization_code";

        //request credentials URI 
        public static readonly string _userProfileUri =
            "https://www.googlepais.com/plus/v1/people/me" +
            "?access_token={0}";

        //get login form URI
        public static string GetAuthRequestUri()
        {
            var authUri = new StringBuilder().AppendFormat( _authRequest, RedirectUri, ClientId ).ToString();
            return authUri;
        }

        //request access token URI
        public static string GetTokenRequestUri( string code )
        {
            var tokenUri = new StringBuilder().AppendFormat( _requestUri, code, ClientId, ClientSecret, RedirectUri ).ToString();
            return tokenUri;
        }

        //get access token method
        public static void GetAccessToken( string uri )
        {
            RestService.GetAsync( uri,
                //onSuccess
                ( response, json ) => {
                    string accessToken = JsonConvert.DeserializeObject<JObject>(json).Value<string>("access_token");
                },

                //onFailure
                ( e ) => {

                });
        }

        //extract '?code=<hash>'
        public static string ExtractCodeFromUrl(string url)
        {
            if (url.Contains("code="))
            {
                List<string> attributes = new List<string>(url.Split('&'));
                var code = attributes.Find(s => s.Contains("code=")).Split('=')[1];

                return code;
            }

            return string.Empty;
        }
    }    
}
