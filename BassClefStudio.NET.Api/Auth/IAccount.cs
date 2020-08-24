using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api
{
    public interface IAccount
    {
        string UserId { get; }
    }

    public interface IAccount<T> : IAccount
    {
        void Authenticate(T request);
    }
}
