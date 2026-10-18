using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.GitHub;

namespace External.GitHub;

public class GitHubMetadata(HttpClient http, GitHubOptions options) : IGitHubMetadata
{
    public async Task<GitHubActivity> Fetch(GitHubUser user, DateOnly from, DateOnly to)
    {
        var repos = await Get<RepoResponse[]>(user, "/user/repos?per_page=100") ?? [];
        var metadata = new List<RepoMetadata>();
        foreach (var repo in repos) metadata.Add(await MetadataFor(user, repo));

        return new GitHubActivity(metadata, []);
    }

    private async Task<RepoMetadata> MetadataFor(GitHubUser user, RepoResponse repo)
    {
        var contributors = await Get<ContributorResponse[]>(user, $"/repos/{repo.FullName}/stats/contributors") ?? [];
        var punchCard = await Get<int[][]>(user, $"/repos/{repo.FullName}/stats/punch_card") ?? [];
        var languages = await Get<Dictionary<string, long>>(user, $"/repos/{repo.FullName}/languages") ?? [];
        var author = contributors.FirstOrDefault(c => string.Equals(c.Author?.Login, user.Handle, StringComparison.OrdinalIgnoreCase));

        return new RepoMetadata(
            repo.Name,
            repo.Private,
            DateOnly.FromDateTime(repo.CreatedAt.UtcDateTime),
            languages,
            author?.Weeks.Select(WeekFrom).ToArray() ?? [],
            contributors.Sum(c => c.Total),
            punchCard.Select(cell => new PunchCell(cell[0], cell[1], cell[2])).ToArray());
    }

    private static Week WeekFrom(ContributorWeekResponse week) =>
        new(DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(week.W).UtcDateTime), week.A, week.D, week.C);

    private async Task<T?> Get<T>(GitHubUser user, string path)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}
