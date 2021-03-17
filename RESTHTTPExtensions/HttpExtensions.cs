using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace RESTHTTPExtensions
{
    public static partial class HttpExtensions
    {
        private const string RouteParam = "Route_";
        public static string UrlFormat<T>(this string url, T value) where T : class
        {

            var paramsDictionary = CreateParamsDictionary(value);

            url = RouteFormats(url, paramsDictionary);
            var query = CreateQueryParams(paramsDictionary);

            
            if (string.IsNullOrWhiteSpace(query))
                return url;

            //TODO Find the Best Possible Solution
            return url.Contains("?") ? $"{url}&{query}" : $"{url}?{query}";
        }
        private static Dictionary<string, Param> CreateParamsDictionary<T>(T value)
        {
            var type = typeof(T);
            if (type.IsAnonymousType())
            {
                return CreateAnonymousTypeParamsDictionary(value);
            }
            else if (type.IsClass)
            {
                return CreateClassTypeParamsDictionary(value);
            }
            else
            {

                throw new NotSupportedException($"{nameof(type)} type not supported");
            }
        }
        private static Dictionary<string, Param> CreateAnonymousTypeParamsDictionary<T>(T value)
        {
            var properties = typeof(T).GetProperties();
            var paramDictionary = new Dictionary<string, Param>();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(value)?.ToString();
                if (property.Name.StartsWith(RouteParam, StringComparison.InvariantCultureIgnoreCase))
                {
                    var propertyName = property.Name[RouteParam.Length..];
                    if (string.IsNullOrWhiteSpace(propertyName))
                        throw new ArgumentOutOfRangeException("incorrect propertyname");

                    var routeParam = new Param(propertyName, propertyValue, ParamType.Route);
                    paramDictionary.Add(propertyName.ToLower(), routeParam);
                    continue;
                }

                var queryParam = new Param(property.Name, propertyValue, ParamType.Query);
                paramDictionary.Add(property.Name, queryParam);
            }

            return paramDictionary;
        }
        private static Dictionary<string, Param> CreateClassTypeParamsDictionary<T>(T value)
        {
            var properties = typeof(T).GetProperties();
            var paramDictionary = new Dictionary<string, Param>();
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
                    var propertyName = queryNameAttribute.Name ?? property.Name;
                    var routeParam = new Param(propertyName, propertyValue, ParamType.Query);
                    paramDictionary.Add(propertyName.ToLower(), routeParam);
                    continue;
                }

                if (queryNameAttribute != null)
                {
                    var propertyName = queryNameAttribute.Name ?? property.Name;
                    var queryParam = new Param(propertyName, propertyValue, ParamType.Query);
                    paramDictionary.Add(propertyName, queryParam);
                    continue;
                }

                if (disableDefaultBehaviourAttribute == null)
                {
                    var propertyName = queryNameAttribute.Name ?? property.Name;
                    var queryParam = new Param(propertyName, propertyValue, ParamType.Query);
                    paramDictionary.Add(property.Name, queryParam);
                }
                    
            }

            return paramDictionary;
        }
        private static string CreateQueryParams(Dictionary<string, Param> paramsDictionary)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryParam in paramsDictionary.Values)
            {
                if(queryParam.Type == ParamType.Query)
                   query.Add(queryParam.Name, queryParam.Value);
            }
            return query.ToString();
        }
        private static string RouteFormats(string url, Dictionary<string, Param> paramsDictionary)
        {
            Regex regex = new Regex(@"\{([^{ }]+)*\}");
            url = regex.Replace(url, delegate (Match match)
            {
                if (match.Groups.Count < 2)
                    return "";

                var key = match.Groups[1].Value.ToLower();
                if (paramsDictionary.Remove(key, out var param) && param.Type == ParamType.Route)
                {
                    return param.Value;
                }

                return match.Value;
            });

            return url;
        }
        private static bool IsAnonymousType(this Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }
    }
}
