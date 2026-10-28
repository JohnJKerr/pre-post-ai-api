namespace Domain;

public record CodingWindow(int Start, int Length)
{
    private const double Target = 0.8;
    private const double FloatTolerance = 1e-9;

    public int End => (Start + Length) % 24;

    public static CodingWindow Holding80PercentOf(IReadOnlyList<HourShare> hourShares)
    {
        for (var length = 1; length <= 24; length++)
            if (FirstWindowOfLength(hourShares, length) is { } window) return window;

        return new CodingWindow(0, 24);
    }

    private static CodingWindow? FirstWindowOfLength(IReadOnlyList<HourShare> hourShares, int length)
    {
        for (var start = 0; start < 24; start++)
            if (ShareFrom(hourShares, start, length) >= Target - FloatTolerance)
                return new CodingWindow(start, length);

        return null;
    }

    private static double ShareFrom(IReadOnlyList<HourShare> hourShares, int start, int length)
    {
        var share = 0d;
        for (var k = 0; k < length; k++) share += hourShares[(start + k) % 24].Share;
        return share;
    }
}
