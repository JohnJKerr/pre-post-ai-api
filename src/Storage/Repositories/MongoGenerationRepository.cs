using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Storage.Repositories;

public class MongoGenerationRepository(IMongoDatabase database) : IGenerationRepository
{
    private readonly IMongoCollection<Generation> _generations = database.GetCollection<Generation>("generations");

    public Task Save(Generation generation) =>
        _generations.ReplaceOneAsync(g => g.Id == generation.Id, generation, new ReplaceOptions { IsUpsert = true });

    public async Task<Generation?> GetById(string id) =>
        await _generations.Find(g => g.Id == id).FirstOrDefaultAsync();
}
