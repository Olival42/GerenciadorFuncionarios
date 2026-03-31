using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using GerenciadorFuncionarios.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

public class RateLimitIntegrationTests
{
    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    var dbDescriptors = services
                        .Where(d => d.ServiceType == typeof(AppDbContext))
                        .ToList();

                    foreach (var d in dbDescriptors)
                        services.Remove(d);

                    var optionsDescriptors = services
                        .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                        .ToList();

                    foreach (var d in optionsDescriptors)
                        services.Remove(d);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });

                builder.Configure(app =>
                {
                    app.Map("/test", testApp =>
                    {
                        testApp.UseRouting();
                        testApp.UseRateLimiter();

                        testApp.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/global", () => "ok");
                            endpoints.MapGet("/auth", () => "ok")
                                     .RequireRateLimiting("Auth");
                        });
                    });
                });
            });
    }

    [Fact]
    public async Task GlobalRateLimit_Should_Return_429_When_LimitExceeded()
    {
        var factory = CreateFactory();
        var client = factory.CreateClient();

        HttpResponseMessage lastResponse = null!;
        for (int i = 0; i < 101; i++)
        {
            lastResponse = await client.GetAsync("/test/global");
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
    }

    [Fact]
    public async Task AuthRateLimit_Should_Return_429_When_LimitExceeded()
    {
        var factory = CreateFactory();
        var client = factory.CreateClient();

        HttpResponseMessage lastResponse = null!;
        for (int i = 0; i < 11; i++)
        {
            lastResponse = await client.GetAsync("/test/auth");
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
    }
}