using Shouldly;

namespace Domain.Tests;

public class WhenBuildingTheSampleReport
{
    [Fact]
    public void Each_side_has_47_complete_weeks()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Weeks.ShouldBe(47);
    }

    [Fact]
    public void Pre_adds_40_459_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Added.ShouldBe(40_459);
    }

    [Fact]
    public void Pre_deletes_12_665_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Deleted.ShouldBe(12_665);
    }

    [Fact]
    public void Pre_has_638_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Commits.ShouldBe(638);
    }

    [Fact]
    public void Post_adds_226_832_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Added.ShouldBe(226_832);
    }

    [Fact]
    public void Post_deletes_74_833_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Deleted.ShouldBe(74_833);
    }

    [Fact]
    public void Post_has_1_604_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Commits.ShouldBe(1_604);
    }

    [Fact]
    public void Pre_has_5_projects()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.ProjectCount.ShouldBe(5);
    }

    [Fact]
    public void Pre_started_2_projects()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Started.ShouldBe(2);
    }

    [Fact]
    public void Post_has_12_projects()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.ProjectCount.ShouldBe(12);
    }

    [Fact]
    public void Post_started_10_projects()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Started.ShouldBe(10);
    }

    [Fact]
    public void Projects_are_ordered_by_lines_descending()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Projects.Select(p => p.Lines).ShouldBeInOrder(SortDirection.Descending);
    }

    [Fact]
    public void A_project_gets_its_share_of_the_window_s_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Projects.ShouldContain(new Project("invoice-api", 18_611));
    }

    [Fact]
    public void Pre_spans_from_2024_12_29()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Range.Start.ShouldBe(new DateOnly(2024, 12, 29));
    }

    [Fact]
    public void Pre_spans_to_2025_11_22()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Range.End.ShouldBe(new DateOnly(2025, 11, 22));
    }

    [Fact]
    public void Post_spans_from_2025_11_23()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Range.Start.ShouldBe(new DateOnly(2025, 11, 23));
    }

    [Fact]
    public void Post_spans_to_2026_10_17()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Range.End.ShouldBe(new DateOnly(2026, 10, 17));
    }

    [Fact]
    public void Every_week_of_the_window_is_kept()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Weekly.Count.ShouldBe(47);
    }

    [Fact]
    public void Pre_was_active_on_161_days()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.ActiveDays.ShouldBe(161);
    }

    [Fact]
    public void Pre_was_active_3_4_days_a_week()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.ActiveDaysPerWeek.ShouldBe(3.4);
    }

    [Fact]
    public void Post_was_active_on_277_days()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.ActiveDays.ShouldBe(277);
    }

    [Fact]
    public void Post_was_active_5_9_days_a_week()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.ActiveDaysPerWeek.ShouldBe(5.9);
    }

    [Fact]
    public void Pre_adds_63_lines_per_commit()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.PerCommit.ShouldBe(63);
    }

    [Fact]
    public void Post_adds_141_lines_per_commit()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.PerCommit.ShouldBe(141);
    }

    [Fact]
    public void Pre_uses_5_languages()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.UsedLanguages.Count.ShouldBe(5);
    }

    [Fact]
    public void Post_uses_8_languages()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.UsedLanguages.Count.ShouldBe(8);
    }

    [Fact]
    public void Markup_is_left_out_of_languages()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Languages.Select(l => l.Language).ShouldNotContain("CSS");
    }

    [Fact]
    public void Languages_are_ordered_by_share_descending()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Languages.Select(l => l.Share).ShouldBeInOrder(SortDirection.Descending);
    }

    [Fact]
    public void Pre_is_busiest_on_Tuesday_at_10()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        (report.Pre.Punch!.Busiest.Day, report.Pre.Punch.Busiest.Hour).ShouldBe((2, 10));
    }

    [Fact]
    public void Post_is_busiest_on_Tuesday_at_21()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        (report.Post.Punch!.Busiest.Day, report.Post.Punch.Busiest.Hour).ShouldBe((2, 21));
    }

    [Fact]
    public void Pre_codes_from_9_to_17()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Punch!.CodingWindow.ShouldBe(new CodingWindow(Start: 9, Length: 8));
    }

    [Fact]
    public void Post_codes_from_8_to_22()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Punch!.CodingWindow.ShouldBe(new CodingWindow(Start: 8, Length: 14));
    }

    [Fact]
    public void Pre_touches_15_hours_of_the_day()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Punch!.HoursTouched.ShouldBe(15);
    }

    [Fact]
    public void Post_touches_21_hours_of_the_day()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Punch!.HoursTouched.ShouldBe(21);
    }

    [Fact]
    public void Every_hour_of_the_day_has_a_share()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Punch!.HourShares.Count.ShouldBe(24);
    }

    [Fact]
    public void Pre_commits_7_1_percent_on_evenings()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Pre.Punch!.Evening).ShouldBe(7.1);
    }

    [Fact]
    public void Pre_commits_5_1_percent_on_weekends()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Pre.Punch!.Weekend).ShouldBe(5.1);
    }

    [Fact]
    public void Pre_commits_12_2_percent_out_of_hours()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Pre.Punch!.Combined).ShouldBe(12.2);
    }

    [Fact]
    public void Post_commits_30_6_percent_on_evenings()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Evening).ShouldBe(30.6);
    }

    [Fact]
    public void Post_commits_24_percent_on_weekends()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Weekend).ShouldBe(24.0);
    }

    [Fact]
    public void Post_commits_54_6_percent_out_of_hours()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Combined).ShouldBe(54.6);
    }

    [Fact]
    public void Pre_punch_card_covers_3_of_5_projects_and_46_percent_of_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Coverage.ShouldBe(new Coverage(Eligible: 3, Total: 5, Share: 0.46));
    }

    [Fact]
    public void Post_punch_card_covers_10_of_12_projects_and_84_percent_of_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Coverage.ShouldBe(new Coverage(Eligible: 10, Total: 12, Share: 0.84));
    }

    [Fact]
    public void Go_Python_Rust_and_Swift_are_new_languages()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.NewLanguages.ShouldBe(["Go", "Python", "Rust", "Swift"], ignoreOrder: true);
    }

    [Fact]
    public void Commits_grow_by_post_over_pre()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.CommitsX.ShouldBe(1_604 / 638d);
    }

    [Fact]
    public void Out_of_hours_grows_about_four_and_a_half_times()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.OutOfHoursX!.Value.ShouldBeInRange(4.45, 4.50);
    }

    [Fact]
    public void The_score_parts_are_20_9_20_and_20()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Parts.ShouldBe(new ScoreParts(Output: 20, Projects: 9, Languages: 20, Time: 20));
    }

    [Fact]
    public void The_score_is_69()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Total.ShouldBe(69);
    }

    [Fact]
    public void The_verdict_is_I_ve_been_assimilated()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Verdict.ShouldBe("I've been assimilated");
    }

    private static double Percent(double share) => Math.Round(share * 100, 1);
}
