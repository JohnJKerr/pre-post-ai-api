using System.Text.Json.Nodes;

namespace Api.Tests;

internal static class JsonDifference
{
    public static string? Between(JsonNode? expected, JsonNode? actual, string path = "$") => (expected, actual) switch
    {
        (null, null) => null,
        (null, _) or (_, null) => $"{path}: expected {expected?.ToJsonString() ?? "null"} but was {actual?.ToJsonString() ?? "null"}",
        (JsonObject e, JsonObject a) => BetweenObjects(e, a, path),
        (JsonArray e, JsonArray a) => BetweenArrays(e, a, path),
        (JsonValue e, JsonValue a) => BetweenValues(e, a, path),
        _ => $"{path}: expected {expected.ToJsonString()} but was {actual.ToJsonString()}",
    };

    private static string? BetweenObjects(JsonObject expected, JsonObject actual, string path)
    {
        var keys = expected.Select(p => p.Key).Union(actual.Select(p => p.Key));
        foreach (var key in keys)
        {
            if (!expected.ContainsKey(key)) return $"{path}.{key}: unexpected property";
            if (!actual.ContainsKey(key)) return $"{path}.{key}: missing property";
            if (Between(expected[key], actual[key], $"{path}.{key}") is { } difference) return difference;
        }

        return null;
    }

    private static string? BetweenArrays(JsonArray expected, JsonArray actual, string path)
    {
        if (expected.Count != actual.Count) return $"{path}: expected {expected.Count} items but was {actual.Count}";
        for (var i = 0; i < expected.Count; i++)
            if (Between(expected[i], actual[i], $"{path}[{i}]") is { } difference)
                return difference;

        return null;
    }

    private static string? BetweenValues(JsonValue expected, JsonValue actual, string path)
    {
        var bothNumbers = expected.TryGetValue<double>(out var e) & actual.TryGetValue<double>(out var a);
        var same = bothNumbers ? e == a : expected.ToJsonString() == actual.ToJsonString();
        return same ? null : $"{path}: expected {expected.ToJsonString()} but was {actual.ToJsonString()}";
    }
}
