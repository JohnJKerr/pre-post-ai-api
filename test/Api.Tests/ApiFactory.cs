using Api.Authentication;
using Api.Generating;
using Domain;
using Domain.GitHub;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Extensions.Logging;
using Testcontainers.MongoDb;

namespace Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly DateTimeOffset Now = new(2026, 10, 18, 9, 0, 0, TimeSpan.Zero);
    public const string FrontEndOrigin = "https://shift.example";

    // mongo:8 is 8.3, which exits on GitHub-hosted Ubuntu: uname -r is 7.0.0, and
    // that falls in the rseq range it refuses (SERVER-131779). The runner kernel
    // is already past the bug. glibc owning rseq is what lets mongod start there,
    // so the readiness wait is not left hanging for an hour.
    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
        .WithImage("mongo:8")
        .WithEnvironment("GLIBC_TUNABLES", "glibc.pthread.rseq=1")
        .Build();

    public Task InitializeAsync() => _mongo.StartAsync();

    public new Task DisposeAsync() => _mongo.DisposeAsync().AsTask();

    internal async Task<string> Saved(Generation generation)
    {
        await Services.GetRequiredService<IGenerationRepository>().Save(generation);
        return generation.Id;
    }

    // The browser keeps real cookies, so the server's clock is the real one: the OAuth correlation
    // cookie would otherwise expire at the fake time.
    internal HttpClient CreateBrowserSigningInWithGitHub(IGitHubMetadata? gitHub = null, FakeGitHubOAuth? gitHubOAuth = null,
        ILogger<ReportGenerator>? generationLog = null) =>
        WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(TimeProvider.System);
                services.AddSingleton(gitHub ?? new FakeGitHub());
                if (generationLog is not null) services.AddSingleton(generationLog);
                services.PostConfigure<OAuthOptions>(GitHubSignIn.Scheme, options => options.Backchannel = new HttpClient(gitHubOAuth ?? new FakeGitHubOAuth()));
            }))
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Mongo:ConnectionString", _mongo.GetConnectionString());
        builder.UseSetting("Mongo:Database", "pre-post-tests");
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
