using BassClefStudio.NET.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.OAuth.Credentials
{
    public class OAuthAccount : IAccount<HttpRequestMessage>
    {
        public string UserId { get; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public void Authenticate(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public OAuthAccount(string username, string accessToken, string refreshToken = null)
        {
            UserId = username;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
