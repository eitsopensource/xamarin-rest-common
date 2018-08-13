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
            "&redirect_uri={1}";

        //request access token URI
        public static readonly string _requestUri =
            "https://graph.facebook.com/oauth/access_token" +
            "?client_id={0}" +
            "&redirect_uri={1}" +
            "&client_secret={2}" +
            "&code={3}";

        //request credentials URI 
        public static readonly string _userProfileUri =
            "https://graph.facebook.com/me?access_token=" +
            "?access_token={0}";


        //get login form URI
        public static string GetAuthRequestUri()
        {
            var authUri = new StringBuilder().AppendFormat( _authRequest, ClientId, RedirectUri ).ToString();
            return authUri;
        }

        //request access token URI
        public static string GetTokenRequestUri( string code )
        {
            var tokenUri = new StringBuilder().AppendFormat( _requestUri, code, ClientId, ClientSecret, RedirectUri ).ToString();
            return tokenUri;
        }

        //get access token method
        public static async Task<string> GetAccessToken( string uri )
        {
            var json = await RestService.GetAsync(uri);
            string accessToken = JsonConvert.DeserializeObject<JObject>(json).Value<string>("access_token");
            return accessToken;
        }

        //manage user profile infos
        public static async Task<string> GetUserProfile( string accessToken )
        {
            var userProfileUri = new StringBuilder().AppendFormat( _userProfileUri, accessToken ).ToString();
            var userJson = await RestService.GetAsync( userProfileUri );

            //var facebookProfile = JsonConvert.DeserializeObject<FacebookProfile>(userJson);
            return userJson;
        }

        //extract '?code=<hash>'
        private static string ExtractCodeFromUrl(string url)
        {
            if (url.Contains("code="))
            {
                List<string> attributes = new List<string>(url.Split('&'));
                var code = attributes.Find(s => s.Contains("code=")).Split('=')[1];

                return code;
            }

            return string.Empty;
        }

        //get webView Login FORM
        public static WebView GetFacebookAuthForm()
        {
            var webView = new WebView
            {
                Source = GetAuthRequestUri(),
                HeightRequest = 1
            };

            webView.Navigated += async (object sender, WebNavigatedEventArgs e) => {
                var code = ExtractCodeFromUrl(e.Url);

                if (code != "")
                {
                    string accessTokenPostUri = GetTokenRequestUri(code);
                    string accessToken = await GetAccessToken(accessTokenPostUri);

                    Debug.WriteLine(await GetUserProfile(accessToken));
                }
            };

            return webView;
        }
    }

    
}
