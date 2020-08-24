using BassClefStudio.NET.Api.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api
{
    public class ApiService<TIn> where TIn : IApiRequest
    {
        public IEnumerable<IApiEndpoint> Endpoints { get; }
        public IAuthProvider<TIn> AuthProvider { get; private set; }

        public ApiService(IEnumerable<IApiEndpoint> endPoints)
        {
            Endpoints = endPoints;
        }

        public void SignIn(IAuthProvider<TIn> auth) => AuthProvider = auth;

        public void SignOut() => AuthProvider = null;

        public async Task<TOut> SendRequestAsync<TOut>(TIn request, string name)
        {
            var endpoint = Endpoints.OfType<IApiEndpoint<TIn, TOut>>().FirstOrDefault(a => a.Name == name);
            if (endpoint != null)
            {
                if (AuthProvider != null)
                {
                    AuthProvider.Authenticate(request);
                }
                return await endpoint.SendRequestAsync(request);
            }
            else
            {
                return default(TOut);
            }
        }
    }
}
