using Shouldly;

namespace Domain.Tests;

public class WhenCommitsBunchAroundMidnight
{
    private readonly Report _report = Report.From(new GivenAProfile()
        .WithRepo(new GivenARepo("night-owl").WithShareOfLines(pre: 0, post: 1).WithPunchCardFor(Era.Post))
        .WithPunchCardAfterTheCutoff(new PunchCell(Day: 3, Hour: 23, Commits: 5), new PunchCell(Day: 3, Hour: 0, Commits: 4), new PunchCell(Day: 3, Hour: 12, Commits: 1))
        .Build());

    [Fact]
    public void The_coding_window_wraps_past_midnight() =>
        _report.Post.Punch!.CodingWindow.ShouldBe(new CodingWindow(Start: 23, Length: 2));
}
