using Api;
using Api.Authentication;
using Domain.GitHub;
using Domain.Repositories;
using External.GitHub;
using MongoDB.Driver;
using Microsoft.AspNetCore.DataProtection;
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
builder.Services.AddDataProtection().SetApplicationName("pre-post-ai-api").PersistKeysToMongo();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiUser, ApiUser>();
builder.Services.AddSingleton(GitHubOptions.Default);
builder.Services.AddHttpClient<IGitHubMetadata, GitHubMetadata>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("pre-post-ai");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
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

app.UseHttpsRedirection();
app.UseCors(FrontEnd.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
