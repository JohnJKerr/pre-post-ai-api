using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Storage.Repositories;

public class MongoProfileRepository(IMongoDatabase database) : IProfileRepository
{
    private static readonly Collation CaseInsensitive = new("en", strength: CollationStrength.Secondary);

    private readonly IMongoCollection<Profile> _profiles = database.GetCollection<Profile>("profiles");

    public async Task Save(Profile profile)
    {
        await EnsureHandleIndex();
        await _profiles.ReplaceOneAsync(
            p => p.Handle == profile.Handle,
            profile,
            new ReplaceOptions { IsUpsert = true, Collation = CaseInsensitive });
    }

    public async Task<Profile?> GetByHandle(string handle) =>
        await _profiles.Find(p => p.Handle == handle, new FindOptions { Collation = CaseInsensitive }).FirstOrDefaultAsync();

    private Task EnsureHandleIndex() => _profiles.Indexes.CreateOneAsync(new CreateIndexModel<Profile>(
        Builders<Profile>.IndexKeys.Ascending(p => p.Handle),
        new CreateIndexOptions { Unique = true, Collation = CaseInsensitive }));
}
