using Domain;

namespace Api.Models;

public record WindowSummaryResponse(
    string Era,
    DateRangeResponse Range,
    int Weeks,
    IReadOnlyList<WeekResponse> Weekly,
    int Added,
    int Deleted,
    int Commits,
    int PerCommit,
    IReadOnlyList<ProjectResponse> Projects,
    int ProjectCount,
    int Started,
    int ActiveDays,
    double ActiveDaysPerWeek,
    IReadOnlyList<LanguageShareResponse> Languages,
    IReadOnlyList<string> UsedLanguages,
    CoverageResponse Coverage,
    PunchResponse? Punch)
{
    public static WindowSummaryResponse From(WindowSummary summary) => new(
        summary.Era == Domain.Era.Pre ? "pre" : "post",
        new DateRangeResponse(Dates.Format(summary.Range.Start), Dates.Format(summary.Range.End)),
        summary.Weeks,
        summary.Weekly.Select(w => new WeekResponse(Dates.Format(w.Start), w.Added, w.Deleted, w.Commits)).ToArray(),
        summary.Added,
        summary.Deleted,
        summary.Commits,
        summary.PerCommit,
        summary.Projects.Select(p => new ProjectResponse(p.Name, p.Lines)).ToArray(),
        summary.ProjectCount,
        summary.Started,
        summary.ActiveDays,
        summary.ActiveDaysPerWeek,
        summary.Languages.Select(l => new LanguageShareResponse(l.Language, l.Share)).ToArray(),
        summary.UsedLanguages,
        new CoverageResponse(summary.Coverage.Eligible, summary.Coverage.Total, summary.Coverage.Share),
        summary.Punch is { } punch ? PunchResponse.From(punch) : null);
}

public record DateRangeResponse(string Start, string End);

public record WeekResponse(string W, int A, int D, int C);

public record ProjectResponse(string? Name, int Lines);

public record LanguageShareResponse(string Language, double Share);

public record CoverageResponse(int Eligible, int Total, double Share);
