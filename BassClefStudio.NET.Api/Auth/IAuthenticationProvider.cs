using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api.Auth
{
    public interface IAuthenticationProvider<T> where T : IAccount
    {
        /// <summary>
        /// Attempts to use <see cref="IAuthenticationProvider{T}"/> to sign in and create an <see cref="IAccount"/> for the current session.
        /// </summary>
        Task<T> TrySignIn();

        /// <summary>
        /// Attempts to refresh the credentials of an existing user <see cref="IAccount"/>.
        /// </summary>
        /// <param name="account">The <typeparamref name="T"/> account to update.</param>
        Task<bool> TryRefreshCredentials(T account);

        /// <summary>
        /// Signs out the <see cref="IAccount"/> from the <see cref="IAuthenticationProvider{T}"/> so that it can no longer authenticate requests.
        /// </summary>
        /// <param name="account">The <typeparamref name="T"/> account to sign out of.</param>
        Task SignOut(T account);
    }
}
