using Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Storage;

public static class ClassMapRegistrar
{
    private static readonly Lock Gate = new();
    private static bool _registered;

    public static void Register()
    {
        lock (Gate)
        {
            if (_registered) return;

            // Ignoring extra elements lets documents saved before a field was dropped still load,
            // such as a cut-off's old label.
            ConventionRegistry.Register(
                "pre-post",
                new ConventionPack
                {
                    new CamelCaseElementNameConvention(),
                    new EnumRepresentationConvention(BsonType.String),
                    new IgnoreExtraElementsConvention(true),
                },
                type => type.Namespace == typeof(Profile).Namespace);
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));
            BsonClassMap.RegisterClassMap<Profile>(map =>
            {
                map.AutoMap();
                map.MapIdMember(p => p.GitHubId);
                map.SetIgnoreExtraElements(true);
            });

            _registered = true;
        }
    }
}
