using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Api
{
    public interface IApiEndpoint
    {
        string Name { get; }
    }

    public interface IApiEndpoint<TIn, TOut> : IApiEndpoint where TIn : IApiRequest
    {
        Task<TOut> SendRequestAsync(TIn request);
    }
}
