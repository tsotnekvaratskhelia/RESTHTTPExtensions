using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RESTHTTPExtensions.Tests
{
    public class HttpExtensionAnonymousObjectTypeTests
    {
        [Fact]
        public void UrlFormat_AnonymousObjectType_Should_Use_For_Route_Param_Test()
        {
            var testObject = new
            {
                route_Id = 1,
            };

            var url = "https://example.com/user/{id}".UrlFormat(testObject);
            url.Should().BeEquivalentTo("https://example.com/user/1");
        }

        [Fact]
        public void UrlFormat_AnonymousObjectType_Should_Use_For_Query_Param_Test()
        {
            var testObject = new
            {
                Id = 1,
            };

            var url = "https://example.com/user".UrlFormat(testObject);
            url.Should().BeEquivalentTo("https://example.com/user?id=1");
        }

        [Fact]
        public void UrlFormat_AnonymousObjectType_Should_Use_For_Query_And_Route_Param_Test()
        {
            var testObject = new
            {
                Route_Id = 1,
                Name = "test"
            };

            var url = "https://example.com/user/{id}".UrlFormat(testObject);
            url.Should().BeEquivalentTo("https://example.com/user/1?Name=test");
        }
    }
}
