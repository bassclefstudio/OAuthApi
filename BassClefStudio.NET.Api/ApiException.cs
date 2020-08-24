using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BassClefStudio.NET.Api
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException() { }
        public ApiException(HttpStatusCode errorCode, string message, string data) : base($"The API call returned the following error:\r\nError code {errorCode}: {message}\r\n{data}") { }
    }
}
