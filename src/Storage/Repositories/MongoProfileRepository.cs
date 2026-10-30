using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Storage.Repositories;

// Profiles are keyed by GitHub user id, which survives a rename. A handle can briefly belong to two
// profiles, when someone takes a renamed user's old name before that user generates again; the
// most recently generated profile holds it.
public class MongoProfileRepository(IMongoDatabase database) : IProfileRepository
{
    private static readonly Collation CaseInsensitive = new("en", strength: CollationStrength.Secondary);

    private readonly IMongoCollection<Profile> _profiles = database.GetCollection<Profile>("profiles");

    public async Task Save(Profile profile)
    {
        await EnsureHandleIndex();
        await _profiles.ReplaceOneAsync(p => p.GitHubId == profile.GitHubId, profile, new ReplaceOptions { IsUpsert = true });
    }

    public async Task<Profile?> GetByHandle(string handle) =>
        await _profiles
            .Find(p => p.Handle == handle, new FindOptions { Collation = CaseInsensitive })
            .SortByDescending(p => p.GeneratedAt)
            .FirstOrDefaultAsync();

    public Task<long> Count() => _profiles.CountDocumentsAsync(FilterDefinition<Profile>.Empty);

    private Task EnsureHandleIndex() => _profiles.Indexes.CreateOneAsync(new CreateIndexModel<Profile>(
        Builders<Profile>.IndexKeys.Ascending(p => p.Handle).Descending(p => p.GeneratedAt),
        new CreateIndexOptions { Name = "handle_generatedAt", Collation = CaseInsensitive }));
}
