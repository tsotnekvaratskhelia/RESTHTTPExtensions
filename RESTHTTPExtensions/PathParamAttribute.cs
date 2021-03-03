using System;

namespace RESTHTTPExtensions
{
    public class PathParamAttribute : Attribute
    {
        public PathParamAttribute(string paramName)
        {
            ParamName = paramName ?? throw new ArgumentException(nameof(paramName));
        }
        public string ParamName { get; }
    }
}
