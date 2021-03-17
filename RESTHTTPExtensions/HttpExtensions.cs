using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace RESTHTTPExtensions
{
    public static class HttpExtensions
    {
        public static string UrlFormat<T>(this string url, T value)
        {

            var paramsDictionary = CreateParamsDictionary(value);

            url = RouteFormats(url, paramsDictionary);
            var query = CreateQueryParams(paramsDictionary);

            //TODO Find the Best Possible Solution
            return url.Contains("?") ? $"{url}&{query}" : $"{url}?{query}";
        }
        private static Dictionary<string, string> CreateParamsDictionary<T>(T value)
        {
            var properties = typeof(T).GetProperties();
            var paramDictionary = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(value)?.ToString();
                if (propertyValue == null)
                    continue;

                var pathParamAttribute = (PathParamAttribute)Attribute.GetCustomAttribute(property, typeof(PathParamAttribute));
                var queryNameAttribute = (QueryNameAttribute)Attribute.GetCustomAttribute(property, typeof(QueryNameAttribute));
                var disableDefaultBehaviourAttribute = (QueryNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisableDefaultBehaviour));

                if (pathParamAttribute != null)
                {
                    paramDictionary.Add(pathParamAttribute.ParamName ?? property.Name, propertyValue);
                    continue;
                }

                if (queryNameAttribute != null)
                {
                    paramDictionary.Add(queryNameAttribute.Name ?? property.Name, propertyValue);
                    continue;
                }

                if (disableDefaultBehaviourAttribute == null)
                    paramDictionary.Add(property.Name, propertyValue);
            }

            return paramDictionary;
        }
        private static string RouteFormats(string url, Dictionary<string, string> paramsDictionary)
        {
            Regex regex = new Regex(@"\{([^{ }]+)*\}");
            url = regex.Replace(url, delegate (Match match) {
                if (match.Groups.Count < 2)
                    return "";

                var key = match.Groups[1].Value;
                if (paramsDictionary.Remove(key, out var value))
                {
                    return value;
                }

                return match.Value;
            });

            return url;
        }
        public static string CreateQueryParams(Dictionary<string, string> paramsDictionary)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryParam in paramsDictionary)
            {
                query.Add(queryParam.Key, queryParam.Value);
            }
            return query.ToString();
        }
    }
}
