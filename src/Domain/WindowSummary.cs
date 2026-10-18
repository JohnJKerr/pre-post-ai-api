namespace Domain;

public record WindowSummary(Era Era, int Added, int Deleted, int Commits)
{
    public static WindowSummary For(Profile profile, Era era)
    {
        var weeks = profile.Activity[era].Weeks;
        return new WindowSummary(era, weeks.Sum(w => w.Added), weeks.Sum(w => w.Deleted), weeks.Sum(w => w.Commits));
    }
}
