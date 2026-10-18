using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Storage.DataProtection;

public static class DataProtectionBuilderExtensions
{
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder)
    {
        builder.Services
            .AddOptions<KeyManagementOptions>()
            .Configure<IMongoDatabase>((options, database) => options.XmlRepository = new MongoXmlRepository(database));
        return builder;
    }
}
