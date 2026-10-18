using Domain;

namespace Api.Models;

public record PunchResponse(
    IReadOnlyList<PunchCellResponse> Cells,
    PunchCellResponse Busiest,
    CodingWindowResponse Window,
    IReadOnlyList<HourShareResponse> HourShares,
    int HoursTouched,
    double Evening,
    double Weekend,
    double Combined)
{
    public static PunchResponse From(Punch punch) => new(
        punch.Cells.Select(PunchCellResponse.From).ToArray(),
        PunchCellResponse.From(punch.Busiest),
        new CodingWindowResponse(punch.CodingWindow.Start, punch.CodingWindow.Length, punch.CodingWindow.End),
        punch.HourShares.Select(h => new HourShareResponse(h.Hour, h.Share)).ToArray(),
        punch.HoursTouched,
        punch.Evening,
        punch.Weekend,
        punch.Combined);
}

public record PunchCellResponse(int Day, int Hour, int Commits)
{
    public static PunchCellResponse From(PunchCell cell) => new(cell.Day, cell.Hour, cell.Commits);
}

public record CodingWindowResponse(int Start, int Length, int End);

public record HourShareResponse(int Hour, double Share);
