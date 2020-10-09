using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api.Google.Classroom
{
    public class Post : IIdentifiable<string>
    {
        public string Id { get; }

        public Post(string id)
        {
            Id = id;
        }
    }
}
