using Domain;

namespace Api.Models;

public record ShiftResponse(
    double? LinesX,
    double? ProjectsX,
    double? CommitsX,
    double? OutOfHoursX,
    IReadOnlyList<string> NewLanguages,
    double? HoursShift)
{
    public static ShiftResponse From(Shift shift) =>
        new(shift.LinesX, shift.ProjectsX, shift.CommitsX, shift.OutOfHoursX, shift.NewLanguages, shift.HoursShift);
}
