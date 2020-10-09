using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api
{
    public class ApiService : IDisposable
    {
        public IAccount<HttpRequestMessage> Account { get; set; }

        private HttpClient client;
        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                }
                return client;
            }
        }

        /// <summary>
        /// Creates an <see cref="ApiService"/> with authentication from a given <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The account provided for authentication.</param>
        public ApiService(IAccount<HttpRequestMessage> account)
        {
            Account = account;
        }

        /// <summary>
        /// Creates an <see cref="ApiService"/> with no authentication.
        /// </summary>
        public ApiService()
        {
            Account = null;
        }

        /// <summary>
        /// Sends a GET request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<JToken> GetAsync(string endpoint, object content, bool encodeData = true)
            => await GetAsync(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a GET request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<JToken> GetAsync(string endpoint, string content = null)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, endpoint);
            if (content != null)
            {
                message.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            Account?.Authenticate(message);
            var result = await Client.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return JToken.Parse(await result.Content.ReadAsStringAsync());
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                throw new ApiException(result.StatusCode, result.ReasonPhrase, await result.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// Sends a GET request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<T> GetAsync<T>(string endpoint, object content, bool encodeData = true)
            => await GetAsync<T>(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a GET request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<T> GetAsync<T>(string endpoint, string content = null)
        {
            var result = await GetAsync(endpoint, content);
            if (result != null)
            {
                return result.ToObject<T>();
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sends a POST request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<JToken> PostAsync(string endpoint, object content, bool encodeData = true)
            => await PostAsync(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a POST request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<JToken> PostAsync(string endpoint, string content = null)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (content != null)
            {
                message.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            Account?.Authenticate(message);
            var result = await Client.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return JToken.Parse(await result.Content.ReadAsStringAsync());
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                throw new ApiException(result.StatusCode, result.ReasonPhrase, await result.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// Sends a POST request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<T> PostAsync<T>(string endpoint, object content, bool encodeData = true)
            => await PostAsync<T>(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a GET request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<T> PostAsync<T>(string endpoint, string content = null)
        {
            var result = await PostAsync(endpoint, content);
            if (result != null)
            {
                return result.ToObject<T>();
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sends a DELETE request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<JToken> DeleteAsync(string endpoint, object content, bool encodeData = true)
            => await DeleteAsync(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a DELETE request to a given API and returns the result as a JSON object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<JToken> DeleteAsync(string endpoint, string content = null)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            if (content != null)
            {
                message.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            Account?.Authenticate(message);
            var result = await Client.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return JToken.Parse(await result.Content.ReadAsStringAsync());
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                throw new ApiException(result.StatusCode, result.ReasonPhrase, await result.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// Sends a DELETE request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a .NET object (serialized to JSON).</param>
        /// <param name="encodeData">A <see cref="bool"/> value indicating whether the JSON content should be URL-encoded.</param>
        public async Task<T> DeleteAsync<T>(string endpoint, object content, bool encodeData = true)
            => await DeleteAsync<T>(endpoint, encodeData ? JToken.FromObject(content).ToString() : Uri.EscapeDataString(JToken.FromObject(content).ToString()));

        /// <summary>
        /// Sends a DELETE request to a given API and returns the result as a .NET object.
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request.</param>
        /// <param name="content">Any content included in the request as a JSON object.</param>
        public async Task<T> DeleteAsync<T>(string endpoint, string content = null)
        {
            var result = await DeleteAsync(endpoint, content);
            if (result != null)
            {
                return result.ToObject<T>();
            }
            else
            {
                return default(T);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if(Client != null)
            {
                Client.Dispose();
            }
        }
    }
}
