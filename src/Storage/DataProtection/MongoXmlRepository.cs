using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Storage.DataProtection;

// Keeps the Data Protection key ring, which encrypts the session cookie, beside the data, so
// sessions survive a restart and work across instances.
public class MongoXmlRepository(IMongoDatabase database) : IXmlRepository
{
    private readonly IMongoCollection<BsonDocument> _keys = database.GetCollection<BsonDocument>("dataProtectionKeys");

    public IReadOnlyCollection<XElement> GetAllElements() =>
        _keys.Find(FilterDefinition<BsonDocument>.Empty).ToList().Select(key => XElement.Parse(key["xml"].AsString)).ToArray();

    public void StoreElement(XElement element, string friendlyName) =>
        _keys.InsertOne(new BsonDocument { ["friendlyName"] = friendlyName, ["xml"] = element.ToString(SaveOptions.DisableFormatting) });
}
