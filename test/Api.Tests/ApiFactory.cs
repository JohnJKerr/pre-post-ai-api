using Api.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Domain.GitHub;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.MongoDb;

namespace Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly DateTimeOffset Now = new(2026, 10, 18, 9, 0, 0, TimeSpan.Zero);

    private readonly MongoDbContainer _mongo = new MongoDbBuilder().WithImage("mongo:8").Build();

    public Task InitializeAsync() => _mongo.StartAsync();

    public new Task DisposeAsync() => _mongo.DisposeAsync().AsTask();

    internal HttpClient CreateClientFor(IApiUser apiUser, IGitHubMetadata? gitHub = null) =>
        WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(apiUser);
            services.AddSingleton(gitHub ?? new FakeGitHub());
        })).CreateClient();

    internal HttpClient CreateBrowserSigningInWithGitHub() =>
        WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(TimeProvider.System);
                services.PostConfigure<OAuthOptions>(GitHubSignIn.Scheme, options => options.Backchannel = new HttpClient(new FakeGitHubOAuth()));
            }))
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Mongo:ConnectionString", _mongo.GetConnectionString());
        builder.UseSetting("Mongo:Database", "pre-post-tests");
        builder.UseSetting("GitHub:ClientId", "test-client-id");
        builder.UseSetting("GitHub:ClientSecret", "test-client-secret");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(Now));
            services.AddSingleton<IGitHubMetadata>(new FakeGitHub());
        });
    }
}
