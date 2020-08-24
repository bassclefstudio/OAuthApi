using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api
{
    /// <summary>
    /// Represents an identifiable location in an API.
    /// </summary>
    public interface IApiEndpoint
    {
        /// <summary>
        /// The name of this <see cref="IApiEndpoint"/>.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents an <see cref="IApiEndpoint"/> with a strongly-typed method that can accept requests.
    /// </summary>
    /// <typeparam name="TIn">The type of input this <see cref="IApiEndpoint"/> receives.</typeparam>
    /// <typeparam name="TOut">The type of output from this <see cref="IApiEndpoint"/>.</typeparam>
    public interface IApiEndpoint<TIn, TOut> : IApiEndpoint
    {
        /// <summary>
        /// Sends a <typeparamref name="TIn"/> request to the <see cref="IApiEndpoint{TIn, TOut}"/> and returns a <typeparamref name="TOut"/> object.
        /// </summary>
        /// <param name="request">The request to send.</param>
        Task<TOut> SendRequestAsync(TIn request);
    }
}
