using BassClefStudio.NET.Api.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api
{
    /// <summary>
    /// Represents an API with multiple <see cref="IApiEndpoint"/>s which requests can be sent to.
    /// </summary>
    /// <typeparam name="TIn">The type of input request sent to this API.</typeparam>
    public class ApiService<TIn>
    {
        /// <summary>
        /// A collection of <see cref="IApiEndpoint"/>s which the API includes.
        /// </summary>
        public IEnumerable<IApiEndpoint> Endpoints { get; }

        /// <summary>
        /// An optional <see cref="IAuthProvider{T}"/> which can authenticate requests.
        /// </summary>
        public IAuthProvider<TIn> AuthProvider { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ApiService{TIn}"/>.
        /// </summary>
        /// <param name="endPoints">A collection of <see cref="IApiEndpoint"/>s which the API includes.</param>
        public ApiService(IEnumerable<IApiEndpoint> endPoints)
        {
            Endpoints = endPoints;
        }

        /// <summary>
        /// Sends the given request to the <see cref="IApiEndpoint"/> in the <see cref="ApiService{TIn}"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of response to receive, and therefore the type of <see cref="IApiEndpoint{TIn, TOut}"/> which can handle the request.</typeparam>
        /// <param name="request">The <typeparamref name="TIn"/> request to send.</param>
        /// <param name="name">The name of the <see cref="IApiEndpoint"/> to send it to.</param>
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
