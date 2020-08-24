using BassClefStudio.NET.Api;
using BassClefStudio.NET.Api.Auth;
using BassClefStudio.UWP.OAuth.Credentials;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BassClefStudio.UWP.OAuth
{
    public class OAuthAuthenticationProvider : IAuthenticationProvider<OAuthAccount>
    {
        public string OAuthEndpoint { get; }
        public string TokenEndpoint { get; }
        public string RedirectParameter { get; }
        public string ClientID { get; }
        public string[] Scopes { get; }
        public string ResponseType { get; }
        private HttpClient Client { get; }
        public string[] Claims { get; }
        public string MeEndpoint { get; }
        public string NameProperty { get; }
        public string EmailProperty { get; }

        public OAuthAuthenticationProvider(string oauthEndpoint, string tokenEndpoint, string redirect, string clientId, string[] scopes, string response, string[] claims, string me, string name, string email)
        {
            OAuthEndpoint = oauthEndpoint;
            TokenEndpoint = tokenEndpoint;
            RedirectParameter = redirect;
            ClientID = clientId;
            Scopes = scopes;
            ResponseType = response;
            Claims = claims;

            MeEndpoint = me;
            NameProperty = name;
            EmailProperty = email;

            Client = new HttpClient();
        }

        /// <summary>
        /// Attempts to retreive user information for a given <see cref="OAuthAccount"/> using the provided URIs.
        /// </summary>
        /// <param name="account">The <see cref="OAuthAccount"/> to update.</param>
        public async Task GetUserInfo(OAuthAccount account)
        {
            var apiService = new ApiService(account);
            var json = await apiService.GetAsync(MeEndpoint) as JObject;

            if (json.ContainsKey(NameProperty))
            {
                var name = json[NameProperty];
                if (name.Type == JTokenType.String)
                {
                    account.Name = (string)name.ToObject(typeof(string));
                }
            }

            if (json.ContainsKey(EmailProperty))
            {
                var email = json[EmailProperty];
                if (email.Type == JTokenType.String)
                {
                    account.Email = (string)email.ToObject(typeof(string));
                }
            }
        }

        /// <inheritdoc/>
        public async Task SignOut(OAuthAccount account)
        {

        }

        /// <inheritdoc/>
        public async Task<OAuthAccount> TrySignIn()
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Stores the state and code_verifier values into local settings.
            // Member variables of this class may not be present when the app is resumed with the
            // authorization response, so LocalSettings can be used to persist any needed values.
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["state"] = state;
            localSettings.Values["code_verifier"] = code_verifier;

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type={7}&scope={6}&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                OAuthEndpoint,
                Uri.EscapeDataString(RedirectParameter),
                ClientID,
                state,
                code_challenge,
                code_challenge_method,
                string.Join(" ", Scopes),
                ResponseType);

            WebAuthenticationResult result = null;
            result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(authorizationRequest), new Uri(RedirectParameter));

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var query = new Uri(result.ResponseData).Query;
                Dictionary<string, string> queryDictionary =
                        query.Substring(1).Split('&')
                             .ToDictionary(c => c.Split('=')[0],
                                           c => Uri.UnescapeDataString(c.Split('=')[1]));
                if (queryDictionary.ContainsKey("access_token") && queryDictionary.ContainsKey("id_token"))
                {
                    queryDictionary.TryGetValue("access_token", out var a);
                    queryDictionary.TryGetValue("id_token", out var jwt);

                    var securityHandler = new JwtSecurityTokenHandler();
                    var jwtToken = securityHandler.ReadJwtToken(jwt);
                    string id = string.Join(":", jwtToken.Claims.Where(c => Claims.Contains(c.Type)).OrderBy(c => c.Type).Select(t => t.Value));

                    if (queryDictionary.ContainsKey("refresh_token"))
                    {
                        queryDictionary.TryGetValue("refresh_token", out var r);
                        return new OAuthAccount(id, a, r);
                    }
                    else
                    {
                        return new OAuthAccount(id, a);
                    }
                }
                else
                {
                    var tokens = await ContinueAuth(new Uri(result.ResponseData));
                    if (tokens == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (tokens.ContainsKey("access_token") && tokens.ContainsKey("id_token"))
                        {
                            var a = (string)tokens["access_token"].ToObject(typeof(string));
                            var jwt = (string)tokens["id_token"].ToObject(typeof(string));

                            var securityHandler = new JwtSecurityTokenHandler();
                            var jwtToken = securityHandler.ReadJwtToken(jwt);
                            string id = string.Join(":", jwtToken.Claims.Where(c => Claims.Contains(c.Type)).OrderBy(c => c.Type).Select(t => t.Value));

                            if (tokens.ContainsKey("refresh_token"))
                            {
                                var r = (string)tokens["refresh_token"].ToObject(typeof(string));
                                return new OAuthAccount(id, a, refreshToken: r);
                            }
                            else
                            {
                                return new OAuthAccount(id, a);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> TryRefreshCredentials(OAuthAccount account)
        {
            string tokenRequestBody = $"client_id={ClientID}&refresh_token={account.RefreshToken}&grant_type=refresh_token";

            StringContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //Debug.WriteLine(Environment.NewLine + "Exchanging code for tokens...");
            HttpResponseMessage response;
            response = await Client.PostAsync(TokenEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                JObject tokens;
                try
                {
                    tokens = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (tokens.ContainsKey("access_token"))
                    {
                        account.AccessToken = (string)tokens["access_token"].ToObject(typeof(string));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Processes the OAuth 2.0 Authorization Response.
        /// </summary>
        private async Task<JObject> ContinueAuth(Uri authorizationResponse)
        {
            string queryString = authorizationResponse.Query;
            //Debug.WriteLine("MainPage received authorizationResponse: " + authorizationResponse);

            // Parses URI params into a dictionary
            // ref: http://stackoverflow.com/a/11957114/72176
            Dictionary<string, string> queryStringParams =
                    queryString.Substring(1).Split('&')
                         .ToDictionary(c => c.Split('=')[0],
                                       c => Uri.UnescapeDataString(c.Split('=')[1]));

            if (queryStringParams.ContainsKey("error"))
            {
                //Debug.WriteLine(String.Format("OAuth authorization error: {0}.", queryStringParams["error"]));
                return null;
            }

            if (!queryStringParams.ContainsKey("code")
                || !queryStringParams.ContainsKey("state"))
            {
                //Debug.WriteLine("Malformed authorization response. " + queryString);
                return null;
            }

            // Gets the Authorization code & state
            string code = queryStringParams["code"];
            string incoming_state = queryStringParams["state"];

            // Retrieves the expected 'state' value from local settings (saved when the request was made).
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string expected_state = (string)localSettings.Values["state"];

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization
            if (incoming_state != expected_state)
            {
                //Debug.WriteLine(String.Format("Received request with invalid state ({0})", incoming_state));
                return null;
            }

            // Resets expected state value to avoid a replay attack.
            localSettings.Values["state"] = null;

            // Authorization Code is now ready to use!
            //Debug.WriteLine(Environment.NewLine + "Authorization code: " + code);

            string code_verifier = (string)localSettings.Values["code_verifier"];
            return await performCodeExchangeAsync(code, code_verifier);
        }

        private async Task<JObject> performCodeExchangeAsync(string code, string code_verifier)
        {
            // Builds the Token request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(RedirectParameter),
                ClientID,
                code_verifier
                );
            StringContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //Debug.WriteLine(Environment.NewLine + "Exchanging code for tokens...");
            HttpResponseMessage response = await Client.PostAsync(TokenEndpoint, content);
            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    //Debug.WriteLine("Authorization code exchange failed.");
                    return null;
                }

                return JObject.Parse(responseString);
            }
            else
            {
                return null;
            }
        }
        private static string randomDataBase64url(uint length)
        {
            IBuffer buffer = CryptographicBuffer.GenerateRandom(length);
            return base64urlencodeNoPadding(buffer);
        }
        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputStirng"></param>
        /// <returns></returns>
        private static IBuffer sha256(string inputStirng)
        {
            HashAlgorithmProvider sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(inputStirng, BinaryStringEncoding.Utf8);
            return sha.HashData(buff);
        }
        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string base64urlencodeNoPadding(IBuffer buffer)
        {
            string base64 = CryptographicBuffer.EncodeToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
