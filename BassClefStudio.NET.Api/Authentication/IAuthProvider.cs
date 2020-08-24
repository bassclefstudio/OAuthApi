using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api.Authentication
{
    /// <summary>
    /// Represents a service that can provide authentication to an request.
    /// </summary>
    /// <typeparam name="T">The types of requests this <see cref="IAuthProvider{T}"/> supports.</typeparam>
    public interface IAuthProvider<in T>
    {
        void Authenticate(T request);
    }
}
