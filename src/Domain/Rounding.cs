namespace Domain;

internal static class Rounding
{
    // Matches the reference implementation's Math.round: halves round towards positive infinity.
    public static double HalfUp(double value) => Math.Floor(value + 0.5);
}
