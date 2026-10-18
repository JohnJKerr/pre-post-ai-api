using Domain.Repositories;
using MongoDB.Driver;
using Storage;
using Storage.Repositories;

var builder = WebApplication.CreateBuilder(args);

ClassMapRegistrar.Register();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["Mongo:ConnectionString"]));
builder.Services.AddSingleton(services =>
    services.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["Mongo:Database"]));
builder.Services.AddSingleton<IProfileRepository, MongoProfileRepository>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
