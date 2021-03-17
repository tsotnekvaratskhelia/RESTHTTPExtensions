using System;

namespace RESTHTTPExtensions
{
    internal class Param
    {
        public Param(string paramName, string paramValue, ParamType type)
        {
            Name = paramName ?? throw new ArgumentNullException(nameof(paramName));
            Value = paramValue ?? throw new ArgumentNullException(nameof(paramValue));
            Type = type;
        }

        public string Name { get; }
        public string Value { get; }
        public ParamType Type { get; }
    }
    internal enum ParamType
    {
        Route,
        Query
    }
}
