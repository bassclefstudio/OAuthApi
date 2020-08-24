using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api.Authentication
{
    public interface IAuthProvider<T> where T : IApiRequest
    {
        void Authenticate(T request);
    }
}
