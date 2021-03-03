using System;

namespace RESTHTTPExtensions
{
    public class QueryNameAttribute : Attribute
    {
        public QueryNameAttribute(string name)
        {

            Name = name ?? throw new ArgumentException(nameof(name));
        }
        public string Name { get; }
    }
}
