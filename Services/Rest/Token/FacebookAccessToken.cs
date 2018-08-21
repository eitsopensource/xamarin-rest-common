using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace xamarinrest.Services.Rest.Token
{
    public class FacebookAccessToken
    {
        //CONFIG Properties 
        public static readonly string ClientId = "1645746692200593";
        public static readonly string ClientSecret = "d7a10e078f3757ab83b447261a623c1e";
        public static readonly string RedirectUri = "https://localhost";

        //FB login form URI
        public static readonly string _authRequest =
            "https://www.facebook.com/dialog/oauth" +
            "?client_id={0}" +
            "&redirect_uri={1}" +
            "&display=popup" +
            "&response_type=token";

        //request credentials URI 
        public static readonly string _userProfileUri =
            "https://graph.facebook.com/me" +
            "?fields=name,email" +
            "&access_token={0}";

        //get login form URI
        public static string GetAuthRequestUri()
        {
            var authUri = RestService.Url + "login/facebook?mobileLogin=1" ;
               // new StringBuilder().AppendFormat( _authRequest, ClientId, RedirectUri ).ToString();
            return authUri;
        }

        //manage user profile infos
        public static async Task<string> GetUserProfile( string accessToken )
        {
            var userProfileUri = new StringBuilder().AppendFormat( _userProfileUri, accessToken ).ToString();
            var userJson = await RestService._client.GetStringAsync(userProfileUri);

            //var facebookProfile = JsonConvert.DeserializeObject<FacebookProfile>(userJson);
            return userJson;
        }

        //extract '#access_token=<hash>'
        private static string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token="))
            {
                List<string> attributes = new List<string>(url.Split('&'));
                var code = attributes.Find(s => s.Contains("access_token=")).Split('=')[1];

                return code;
            }

            return string.Empty;
        }

        //get webView Login FORM
        public static void GetFacebookAuthForm( WebView webview, Action<string> callback)
        {
            webview.Source = GetAuthRequestUri();
            webview.Navigated += async (object sender, WebNavigatedEventArgs e) => {
                var accessToken = ExtractAccessTokenFromUrl(e.Url);
                if (accessToken != "")
                {
                    callback.Invoke( accessToken );
                }
            };
        }
    }

    
}
