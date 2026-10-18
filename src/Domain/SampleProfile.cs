namespace Domain;

// Port of the front end's server/sample.ts. The random sequence, and so the call order below,
// must match it exactly: the handoff's expected values were taken from that sequence.
public static class SampleProfile
{
    public const string Handle = "sam-builds";
    private const int Seed = 20251124;
    private const int Weeks = 47;
    private static readonly DateOnly CutoffWeekStart = new(2025, 11, 23);
    private static readonly DateTimeOffset GeneratedAt = new(2026, 10, 18, 0, 0, 0, TimeSpan.Zero);

    private static readonly PerEra<double[]> DayWeight = new(
        [0.12, 1, 1, 1, 0.95, 0.85, 0.2],
        [0.75, 1, 1, 1, 1, 0.9, 0.85]);

    private static readonly PerEra<double[]> HourWeight = new(
        [0.01, 0, 0, 0, 0, 0, 0, 0.04, 0.25, 0.8, 1, 1, 0.45, 0.8, 1, 0.95, 0.85, 0.55, 0.16, 0.12, 0.14, 0.12, 0.06, 0.03],
        [0.38, 0.18, 0.04, 0, 0, 0.02, 0.12, 0.42, 0.62, 0.72, 0.8, 0.82, 0.5, 0.72, 0.78, 0.76, 0.7, 0.6, 0.5, 0.78, 0.92, 0.98, 0.9, 0.66]);

    private static readonly PerEra<int> Commits = new(641, 1608);
    private static readonly PerEra<int> LinesPerCommit = new(64, 142);
    private static readonly PerEra<double> DeleteRatio = new(0.31, 0.33);
    private static readonly PerEra<double> PunchShare = new(0.46, 0.84);

    private static readonly IReadOnlyList<Repo> Repos =
    [
        RepoWith("invoice-api", ("C#", 0.9), ("SQL", 0.07), ("Shell", 0.03)) with { Share = new(0.46, 0.14) },
        RepoWith("client-portal", ("TypeScript", 0.82), ("CSS", 0.12), ("HTML", 0.06)) with { Share = new(0.26, 0), PunchCardEra = Era.Pre, StartedIn = Era.Pre },
        RepoWith("ledger-sync", ("C#", 0.95), ("SQL", 0.05)) with { Share = new(0.14, 0), PunchCardEra = Era.Pre, StartedIn = Era.Pre },
        RepoWith("ops-scripts", ("Shell", 0.9), ("Python", 0.1)) with { Share = new(0.08, 0.02) },
        RepoWith("blog", ("JavaScript", 0.7), ("CSS", 0.2), ("HTML", 0.1)) with { Share = new(0.06, 0), PunchCardEra = Era.Pre },
        Started("agent-runner", 0.19, ("TypeScript", 0.92), ("CSS", 0.05), ("Shell", 0.03)),
        Started("snip-cast", 0.13, ("Python", 0.95), ("Shell", 0.05)),
        Started("queue-lens", 0.1, ("Go", 0.97), ("Shell", 0.03)),
        Started("status-board", 0.09, ("TypeScript", 0.85), ("CSS", 0.1), ("HTML", 0.05)),
        Started("env-loader", 0.08, ("C#", 1)),
        Started("thumb-rs", 0.07, ("Rust", 1)),
        Started("pace-coach", 0.06, ("Swift", 1)),
        Started("league-hall", 0.05, ("TypeScript", 0.8), ("CSS", 0.15), ("HTML", 0.05)),
        Started("chat-ops", 0.035, ("Python", 1)),
        Started("sql-reports", 0.035, ("SQL", 0.8), ("Python", 0.2)),
    ];

    public static Profile Generate()
    {
        var random = new Mulberry32(Seed);
        var pre = WeeksFor(Era.Pre, random);
        var post = WeeksFor(Era.Post, random);
        var prePunchCard = PunchCardFor(Era.Pre, (int)Rounding.HalfUp(Commits.Pre * PunchShare.Pre), random);
        var postPunchCard = PunchCardFor(Era.Post, (int)Rounding.HalfUp(Commits.Post * PunchShare.Post), random);

        return new Profile(
            Handle,
            IsSample: true,
            GeneratedAt,
            Cutoff.Default,
            Repos,
            new PerEra<WindowActivity>(
                new WindowActivity(pre.Weeks, prePunchCard, pre.ActiveDays, PunchShare.Pre),
                new WindowActivity(post.Weeks, postPunchCard, post.ActiveDays, PunchShare.Post)));
    }

    private static (IReadOnlyList<Week> Weeks, int ActiveDays) WeeksFor(Era era, Mulberry32 random)
    {
        var daily = DailyCommits(era, random);
        var start = era == Era.Pre ? CutoffWeekStart.AddDays(-Weeks * 7) : CutoffWeekStart;
        var weeks = new List<Week>();
        for (var k = 0; k < Weeks; k++)
        {
            var commits = daily.Skip(k * 7).Take(7).Sum();
            var added = (int)Rounding.HalfUp(commits * LinesPerCommit[era] * random.Jitter(0.45));
            var deleted = (int)Rounding.HalfUp(added * DeleteRatio[era] * random.Jitter(0.3));
            weeks.Add(new Week(start.AddDays(k * 7), added, deleted, commits));
        }

        return (weeks, daily.Count(n => n > 0));
    }

    private static int[] DailyCommits(Era era, Mulberry32 random)
    {
        var raw = new double[Weeks * 7];
        for (var i = 0; i < raw.Length; i++)
        {
            var week = i / 7;
            var dayOfWeek = i % 7;
            var weekFactor = era == Era.Pre ? 1 : 0.55 + 0.75 * ((double)week / Math.Max(1, Weeks - 1));
            if (era == Era.Pre && week is 0 or 33 or 34) weekFactor = 0.1;
            if (era == Era.Post && week == 4) weekFactor = 0.35;
            var activeChance = (era == Era.Pre ? 0.62 : 0.86) * DayWeight[era][dayOfWeek] + 0.05;
            var active = random.Next() < activeChance;
            raw[i] = active ? weekFactor * DayWeight[era][dayOfWeek] * random.Jitter(0.6) : 0;
        }

        var sum = SumInOrder(raw);
        return raw.Select(v => (int)Rounding.HalfUp(v / sum * Commits[era])).ToArray();
    }

    private static IReadOnlyList<PunchCell> PunchCardFor(Era era, int total, Mulberry32 random)
    {
        var raw = new double[7 * 24];
        for (var i = 0; i < raw.Length; i++)
        {
            var weight = DayWeight[era][i / 24] * HourWeight[era][i % 24];
            raw[i] = weight > 0 ? weight * random.Jitter(0.35) : 0;
        }

        var sum = SumInOrder(raw);
        return raw.Select((v, i) => new PunchCell(i / 24, i % 24, (int)Rounding.HalfUp(v / sum * total))).ToArray();
    }

    private static double SumInOrder(double[] values)
    {
        var sum = 0d;
        foreach (var value in values) sum += value;
        return sum;
    }

    private static Repo RepoWith(string name, params (string Language, double Share)[] languages) =>
        new(name, IsPrivate: false, languages.ToDictionary(l => l.Language, l => l.Share), new PerEra<double>(0, 0), PunchCardEra: null, StartedIn: null);

    private static Repo Started(string name, double postShare, params (string Language, double Share)[] languages) =>
        RepoWith(name, languages) with { Share = new(0, postShare), PunchCardEra = Era.Post, StartedIn = Era.Post };

    private class Mulberry32(int seed)
    {
        private int _state = seed;

        public double Next()
        {
            unchecked
            {
                _state += 0x6d2b79f5;
                var t = (_state ^ (int)((uint)_state >> 15)) * (1 | _state);
                t = (t + (t ^ (int)((uint)t >> 7)) * (61 | t)) ^ t;
                return (uint)(t ^ (int)((uint)t >> 14)) / 4294967296d;
            }
        }

        public double Jitter(double spread) => 1 + (Next() * 2 - 1) * spread;
    }
}
