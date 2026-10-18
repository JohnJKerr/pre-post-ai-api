using System.Text.Json.Serialization;

namespace External.GitHub;

internal record RepoResponse(
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("private")] bool Private,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("pushed_at")] DateTimeOffset? PushedAt);

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

internal record GraphQlResponse<T>([property: JsonPropertyName("data")] T? Data);

internal record CalendarData([property: JsonPropertyName("viewer")] CalendarViewer Viewer);

internal record CalendarViewer([property: JsonPropertyName("contributionsCollection")] ContributionsCollection ContributionsCollection);

internal record ContributionsCollection([property: JsonPropertyName("contributionCalendar")] ContributionCalendar ContributionCalendar);

internal record ContributionCalendar([property: JsonPropertyName("weeks")] IReadOnlyList<CalendarWeek> Weeks);

internal record CalendarWeek([property: JsonPropertyName("contributionDays")] IReadOnlyList<ContributionDay> ContributionDays);

internal record ContributionDay(
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("contributionCount")] int ContributionCount);
