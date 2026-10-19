using System.Net;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Shouldly;

namespace Api.Tests;

// Cut-offs used to be saved with a label such as "Claude Opus 4.5". The front end names them now.
[Collection(ApiCollection.Name)]
public class WhenLoadingDocumentsSavedWithACutoffLabel(ApiFactory factory)
{
    [Fact]
    public async Task The_generation_is_found()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().Build());
        await LabelTheCutoff("generations", new BsonDocument("_id", id));

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/generations/{id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task The_report_is_found()
    {
        // Arrange
        var profile = new GivenAProfile().For("labelled-dev").Build();
        await factory.Services.GetRequiredService<IProfileRepository>().Save(profile);
        await LabelTheCutoff("profiles", new BsonDocument("_id", profile.GitHubId));

        // Act
        var response = await factory.CreateClient().GetAsync("/api/reports/labelled-dev");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private Task LabelTheCutoff(string collection, BsonDocument filter) =>
        factory.Services.GetRequiredService<IMongoDatabase>()
            .GetCollection<BsonDocument>(collection)
            .UpdateOneAsync(filter, Builders<BsonDocument>.Update.Set("cutoff.label", "Claude Opus 4.5"));
}
