using AutoFixture;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace MinimalApi.Tests;

internal class ApiCustomizations : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() =>
        {
            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Scheme = "https";
            httpRequest.Host = new HostString("testhost");
            httpRequest.PathBase = new PathString("/api");

            return httpRequest;
        });
    }
}
