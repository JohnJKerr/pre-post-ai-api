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
    public const string FrontEndOrigin = "https://shift.example";
    private const string Database = "pre-post-tests";

    private readonly MongoDbContainer _mongo = new MongoDbBuilder().WithImage("mongo:8").Build();

    public Task InitializeAsync() => _mongo.StartAsync();

    public new Task DisposeAsync() => _mongo.DisposeAsync().AsTask();

    // Clients send the front end's Origin, as a browser on the front end would.
    internal HttpClient CreateClientFor(IApiUser apiUser, IGitHubMetadata? gitHub = null, TimeProvider? clock = null) =>
        FromTheFrontEnd(WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            if (clock is not null) services.AddSingleton(clock);
            services.AddSingleton(apiUser);
            services.AddSingleton(gitHub ?? new FakeGitHub());
        })).CreateClient());

    // The browser keeps real cookies, so the server's clock must start at the real time.
    internal HttpClient CreateBrowserSigningInWithGitHub(
        IGitHubMetadata? gitHub = null, TimeProvider? clock = null, FakeGitHubOAuth? gitHubOAuth = null, string database = Database) =>
        FromTheFrontEnd(WithWebHostBuilder(builder => builder.UseSetting("Mongo:Database", database).ConfigureTestServices(services =>
            {
                services.AddSingleton(clock ?? TimeProvider.System);
                services.AddSingleton(gitHub ?? new FakeGitHub());
                services.PostConfigure<OAuthOptions>(GitHubSignIn.Scheme, options => options.Backchannel = new HttpClient(gitHubOAuth ?? new FakeGitHubOAuth()));
            }))
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") }));

    private static HttpClient FromTheFrontEnd(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Origin", FrontEndOrigin);
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Mongo:ConnectionString", _mongo.GetConnectionString());
        builder.UseSetting("Mongo:Database", Database);
        builder.UseSetting("FrontEnd:Origin", FrontEndOrigin);
        builder.UseSetting("GitHub:AppSlug", "pre-post-test");
        builder.UseSetting("GitHub:ClientId", "test-client-id");
        builder.UseSetting("GitHub:ClientSecret", "test-client-secret");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(Now));
            services.AddSingleton<IGitHubMetadata>(new FakeGitHub());
        });
    }
}
