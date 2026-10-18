namespace Domain;

public record Report(string Handle, bool IsSample, DateTimeOffset GeneratedAt, Cutoff Cutoff, WindowSummary Pre, WindowSummary Post)
{
    public Shift Shift => Shift.Between(Pre, Post);

    public Score Score => Shift.Score();

    public int Weeks => Post.Weeks;

    public Scales Scales => Scales.Across(Pre, Post);

    public static Report From(Profile profile) => new(
        profile.Handle,
        profile.IsSample,
        profile.GeneratedAt,
        profile.Cutoff,
        WindowSummary.For(profile, Era.Pre),
        WindowSummary.For(profile, Era.Post));
}
