using System.Globalization;
using Domain;

namespace Api.Models;

public record ReportResponse(
    string Handle,
    bool IsSample,
    string GeneratedAt,
    CutoffResponse Cutoff,
    int Weeks,
    WindowSummaryResponse Pre,
    WindowSummaryResponse Post,
    ShiftResponse Shift,
    ScoreResponse Score,
    ScalesResponse Scales)
{
    public static ReportResponse From(Report report) => new(
        report.Handle,
        report.IsSample,
        report.GeneratedAt.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
        new CutoffResponse(Dates.Format(report.Cutoff.Date)),
        report.Weeks,
        WindowSummaryResponse.From(report.Pre),
        WindowSummaryResponse.From(report.Post),
        ShiftResponse.From(report.Shift),
        ScoreResponse.From(report.Score),
        new ScalesResponse(report.Scales.WeekMax, report.Scales.ProjectMax, report.Scales.CellMax, report.Scales.HourMax));
}

public record CutoffResponse(string Date);

public record ScalesResponse(int WeekMax, int ProjectMax, int CellMax, double HourMax);
