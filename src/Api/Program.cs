using Api;
using Api.Authentication;
using Domain.GitHub;
using Domain.Repositories;
using External.GitHub;
using MongoDB.Driver;
using Storage;
using Storage.Repositories;

var builder = WebApplication.CreateBuilder(args);

ClassMapRegistrar.Register();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["Mongo:ConnectionString"]));
builder.Services.AddSingleton(services =>
    services.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["Mongo:Database"]));
builder.Services.AddSingleton<IProfileRepository, MongoProfileRepository>();
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
builder.Services.AddGitHubSignIn(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
