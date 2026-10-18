using Domain.GitHub;

namespace Domain.Tests;

internal class GivenAGitHubRepo
{
    private readonly Dictionary<string, long> _languageBytes = [];
    private readonly List<Week> _authorWeeks = [];
    private readonly List<PunchCell> _punchCard = [];
    private bool _isPrivate;
    private DateOnly _createdOn = new(2020, 1, 1);
    private int _commitsByOthers;

    public GivenAGitHubRepo Private()
    {
        _isPrivate = true;
        return this;
    }

    public GivenAGitHubRepo CreatedOn(DateOnly date)
    {
        _createdOn = date;
        return this;
    }

    public GivenAGitHubRepo Writing(string language, long bytes)
    {
        _languageBytes[language] = bytes;
        return this;
    }

    public GivenAGitHubRepo WithAuthorWeek(DateOnly weekStart, int added, int commits)
    {
        _authorWeeks.Add(new Week(weekStart, added, added / 4, commits));
        return this;
    }

    public GivenAGitHubRepo WithCommitsByOthers(int commits)
    {
        _commitsByOthers = commits;
        return this;
    }

    public GivenAGitHubRepo WithPunchCell(int day, int hour, int commits)
    {
        _punchCard.Add(new PunchCell(day, hour, commits));
        return this;
    }

    public RepoMetadata Build() => new(
        _isPrivate,
        _createdOn,
        _languageBytes,
        _authorWeeks,
        _authorWeeks.Sum(w => w.Commits) + _commitsByOthers,
        _punchCard);
}
