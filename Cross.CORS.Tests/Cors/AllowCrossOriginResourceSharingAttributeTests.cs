using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cross.CORS.Tests.Cors;

public class AllowCrossOriginResourceSharingAttributeTests
{
    [TearDown]
    public void TearDown()
    {
        var field = typeof(CorsRegistry).GetField("_config",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);
    }

    [Test]
    public async Task OnActionExecuted_WithLocalConfig_AddsCorsHeaders()
    {
        using var host = await CreateHostAsync();

        var client = host.GetTestClient();
        var response = await client.GetAsync("/test/local");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("Access-Control-Allow-Origin").First().Should().Be("https://local.com");
        response.Headers.GetValues("Access-Control-Allow-Methods").First().Should().Be("GET");
        response.Headers.GetValues("Access-Control-Allow-Headers").First().Should().Be("Content-Type");
        response.Headers.GetValues("Access-Control-Allow-Credentials").First().Should().Be("true");
    }

    [Test]
    public async Task OnActionExecuted_WithGlobalConfig_AddsCorsHeaders()
    {
        CorsRegistry.Register(new CorsConfig(
            allowOrigins: "https://global.com",
            allowMethods: "GET, POST",
            allowHeaders: "*",
            allowCredentials: false));

        using var host = await CreateHostAsync();

        var client = host.GetTestClient();
        var response = await client.GetAsync("/test/global");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("Access-Control-Allow-Origin").First().Should().Be("https://global.com");
        response.Headers.GetValues("Access-Control-Allow-Methods").First().Should().Be("GET, POST");
        response.Headers.GetValues("Access-Control-Allow-Headers").First().Should().Be("*");
        response.Headers.GetValues("Access-Control-Allow-Credentials").First().Should().Be("false");
    }

    [Test]
    public async Task OnActionExecuted_WithMultipleOrigins_EchoesRequestOrigin()
    {
        CorsRegistry.Register(new CorsConfig(
            allowOrigins: "https://a.com, https://b.com, https://c.com",
            allowMethods: "GET",
            allowHeaders: "*",
            allowCredentials: false));

        using var host = await CreateHostAsync();

        var response = await host.GetTestServer()
            .CreateRequest("/test/global")
            .AddHeader("Origin", "https://b.com")
            .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("Access-Control-Allow-Origin").First().Should().Be("https://b.com");
    }

    [Test]
    public async Task OnActionExecuted_WithNoConfig_Throws()
    {
        using var host = CreateHostSync();

        await host.StartAsync();
        var client = host.GetTestClient();

        var act = async () => await client.GetAsync("/test/global");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No {CorsRegistry.CorsConfigKey} configuration found*");
    }

    private static async Task<IHost> CreateHostAsync()
    {
        var host = new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(services =>
                {
                    services.AddControllers()
                        .AddApplicationPart(typeof(TestController).Assembly);
                });
                web.Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                });
            })
            .Build();

        await host.StartAsync();
        return host;
    }

    private static IHost CreateHostSync()
    {
        return new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(services =>
                {
                    services.AddControllers()
                        .AddApplicationPart(typeof(TestController).Assembly);
                });
                web.Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                });
            })
            .Build();
    }
}

[Route("test")]
public class TestController : ControllerBase
{
    [HttpGet("local")]
    [AllowCrossOriginResourceSharing(
        allowOrigins: "https://local.com",
        allowMethods: "GET",
        allowHeaders: "Content-Type",
        allowCredentials: true)]
    public IActionResult Local() => Ok();

    [HttpGet("global")]
    [AllowCrossOriginResourceSharing]
    public IActionResult Global() => Ok();
}
