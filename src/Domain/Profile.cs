using Domain.GitHub;

namespace Domain;

public record Profile(
    long GitHubId,
    string Handle,
    bool IsSample,
    DateTimeOffset GeneratedAt,
    Cutoff Cutoff,
    IReadOnlyList<Repo> Repos,
    PerEra<WindowActivity> Activity)
{
    public bool HasCommits => Activity.Pre.Weeks.Concat(Activity.Post.Weeks).Any(w => w.Commits > 0);

    public static Profile Assemble(long gitHubId, string handle, Cutoff cutoff, GitHubActivity activity, DateTimeOffset now)
    {
        var post = Window.After(cutoff.Date, DateOnly.FromDateTime(now.UtcDateTime));
        var assembly = new ProfileAssembly(activity, new PerEra<Window>(post.Preceding(), post));

        return new Profile(
            gitHubId,
            handle,
            IsSample: false,
            now,
            cutoff,
            assembly.Repos(),
            new PerEra<WindowActivity>(assembly.ActivityIn(Era.Pre), assembly.ActivityIn(Era.Post)));
    }
}
