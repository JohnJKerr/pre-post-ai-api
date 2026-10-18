using System.Text.Json.Serialization;

namespace External.GitHub;

internal record RepoResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("private")] bool Private,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

internal record ContributorResponse(
    [property: JsonPropertyName("author")] AuthorResponse? Author,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("weeks")] IReadOnlyList<ContributorWeekResponse> Weeks);

internal record AuthorResponse([property: JsonPropertyName("login")] string Login);

internal record ContributorWeekResponse(
    [property: JsonPropertyName("w")] long W,
    [property: JsonPropertyName("a")] int A,
    [property: JsonPropertyName("d")] int D,
    [property: JsonPropertyName("c")] int C);
