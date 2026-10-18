using System.Globalization;

namespace Api.Models;

internal static class Dates
{
    public static string Format(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
