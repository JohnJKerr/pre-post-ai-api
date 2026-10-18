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

            ConventionRegistry.Register(
                "pre-post",
                new ConventionPack { new CamelCaseElementNameConvention(), new EnumRepresentationConvention(BsonType.String) },
                type => type.Namespace == typeof(Profile).Namespace);
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));
            BsonClassMap.RegisterClassMap<Profile>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            _registered = true;
        }
    }
}
