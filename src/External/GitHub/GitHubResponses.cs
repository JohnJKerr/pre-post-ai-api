using System.Text.Json.Serialization;

namespace External.GitHub;

internal record GraphQlResponse<T>(
    [property: JsonPropertyName("data")] T? Data,
    [property: JsonPropertyName("errors")] IReadOnlyList<GraphQlError>? Errors);

internal record GraphQlError([property: JsonPropertyName("message")] string Message);

internal record PageInfo(
    [property: JsonPropertyName("hasNextPage")] bool HasNextPage,
    [property: JsonPropertyName("endCursor")] string? EndCursor);

internal record ReposData([property: JsonPropertyName("viewer")] ReposViewer Viewer);

internal record ReposViewer(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("repositories")] RepoConnection Repositories);

internal record RepoConnection(
    [property: JsonPropertyName("pageInfo")] PageInfo PageInfo,
    [property: JsonPropertyName("nodes")] IReadOnlyList<RepoNode> Nodes);

internal record RepoNode(
    [property: JsonPropertyName("owner")] RepoOwner Owner,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("isPrivate")] bool IsPrivate,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("pushedAt")] DateTimeOffset? PushedAt,
    [property: JsonPropertyName("languages")] LanguageConnection Languages);

internal record RepoOwner([property: JsonPropertyName("login")] string Login);

internal record LanguageConnection([property: JsonPropertyName("edges")] IReadOnlyList<LanguageEdge> Edges);

internal record LanguageEdge(
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("node")] LanguageNode Node);

internal record LanguageNode([property: JsonPropertyName("name")] string Name);

internal record HistoryData([property: JsonPropertyName("repository")] HistoryRepository? Repository);

internal record HistoryRepository([property: JsonPropertyName("defaultBranchRef")] BranchRef? DefaultBranchRef);

internal record BranchRef([property: JsonPropertyName("target")] CommitHistories? Target);

internal record CommitHistories(
    [property: JsonPropertyName("all")] CommitCount All,
    [property: JsonPropertyName("mine")] CommitConnection Mine);

internal record CommitCount([property: JsonPropertyName("totalCount")] int TotalCount);

internal record CommitConnection(
    [property: JsonPropertyName("pageInfo")] PageInfo PageInfo,
    [property: JsonPropertyName("nodes")] IReadOnlyList<CommitNode> Nodes);

internal record CommitNode(
    [property: JsonPropertyName("author")] CommitAuthor Author,
    [property: JsonPropertyName("additions")] int Additions,
    [property: JsonPropertyName("deletions")] int Deletions);

internal record CommitAuthor([property: JsonPropertyName("date")] DateTimeOffset Date);

internal record CalendarData([property: JsonPropertyName("viewer")] CalendarViewer Viewer);

internal record CalendarViewer([property: JsonPropertyName("contributionsCollection")] ContributionsCollection ContributionsCollection);

internal record ContributionsCollection([property: JsonPropertyName("contributionCalendar")] ContributionCalendar ContributionCalendar);

internal record ContributionCalendar([property: JsonPropertyName("weeks")] IReadOnlyList<CalendarWeek> Weeks);

internal record CalendarWeek([property: JsonPropertyName("contributionDays")] IReadOnlyList<ContributionDay> ContributionDays);

internal record ContributionDay(
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("contributionCount")] int ContributionCount);
