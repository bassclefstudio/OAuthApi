using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api.Auth
{
    public interface ICredentialService<T> where T : IAccount
    {
        void SaveCredentials(T account);
        bool RefreshCredentials(T account);
        bool RemoveCredentials(T account);
    }
}
