using Api;
using Api.Authentication;
using Api.Generating;
using Domain.GitHub;
using Domain.Repositories;
using External.GitHub;
using MongoDB.Driver;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Storage;
using Storage.DataProtection;
using Storage.Repositories;

var builder = WebApplication.CreateBuilder(args);
var frontEnd = FrontEnd.From(builder.Configuration);

ClassMapRegistrar.Register();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["Mongo:ConnectionString"]));
builder.Services.AddSingleton(services =>
    services.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["Mongo:Database"]));
builder.Services.AddSingleton<IProfileRepository, MongoProfileRepository>();
builder.Services.AddSingleton<IGenerationRepository, MongoGenerationRepository>();
builder.Services.AddDataProtection().SetApplicationName("pre-post-ai-api").PersistKeysToMongo();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<GenerationQueue>();
builder.Services.AddHostedService<ReportGenerator>();
builder.Services.AddSingleton(GitHubOptions.Default);
builder.Services.AddHttpClient<IGitHubMetadata, GitHubMetadata>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("pre-post-ai");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    // TLS can end at a load balancer. Without this the OAuth redirect_uri would be http and GitHub
    // would reject it. Only proxies in ForwardedHeaders:KnownNetworks (and loopback) are trusted.
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    foreach (var network in builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ?? [])
    {
        options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
    }
});
builder.Services.AddSingleton(frontEnd);
builder.Services.AddCors(options => options.AddPolicy(FrontEnd.CorsPolicy, policy => policy
    .WithOrigins(frontEnd.Origin)
    .AllowCredentials()
    .WithHeaders("Content-Type")
    .WithMethods("GET", "POST")));
builder.Services.AddGitHubSignIn(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors(FrontEnd.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
