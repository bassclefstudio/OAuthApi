using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Api.Google.Classroom
{
    public class Course : IIdentifiable<string>
    {
        public string Id { get; }

        public Course(string id)
        {
            Id = id;
        }
    }
}
