using Shouldly;

namespace Domain.Tests;

public class WhenBuildingTheSampleReport
{
    [Fact]
    public void Each_side_has_26_complete_weeks()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Weeks.ShouldBe(26);
    }

    [Fact]
    public void Pre_adds_38_306_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Added.ShouldBe(38_306);
    }

    [Fact]
    public void Pre_deletes_12_125_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Deleted.ShouldBe(12_125);
    }

    [Fact]
    public void Pre_has_638_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Commits.ShouldBe(639);
    }

    [Fact]
    public void Post_adds_232_608_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Added.ShouldBe(232_608);
    }

    [Fact]
    public void Post_deletes_72_522_lines()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Deleted.ShouldBe(72_522);
    }

    [Fact]
    public void Post_has_1_617_commits()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Commits.ShouldBe(1_617);
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
        report.Pre.Projects.ShouldContain(new Project("invoice-api", 17_621));
    }

    [Fact]
    public void Pre_spans_from_2024_12_29()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Range.Start.ShouldBe(new DateOnly(2025, 5, 25));
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
        report.Post.Range.Start.ShouldBe(new DateOnly(2026, 4, 19));
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
        report.Pre.Weekly.Count.ShouldBe(26);
    }

    [Fact]
    public void Pre_was_active_on_99_days()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.ActiveDays.ShouldBe(99);
    }

    [Fact]
    public void Pre_was_active_3_8_days_a_week()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.ActiveDaysPerWeek.ShouldBe(3.8);
    }

    [Fact]
    public void Post_was_active_on_155_days()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.ActiveDays.ShouldBe(155);
    }

    [Fact]
    public void Post_was_active_6_days_a_week()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.ActiveDaysPerWeek.ShouldBe(6);
    }

    [Fact]
    public void Pre_adds_63_lines_per_commit()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.PerCommit.ShouldBe(60);
    }

    [Fact]
    public void Post_adds_141_lines_per_commit()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.PerCommit.ShouldBe(144);
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
        (report.Pre.Punch!.Busiest.Day, report.Pre.Punch.Busiest.Hour).ShouldBe((1, 11));
    }

    [Fact]
    public void Post_is_busiest_on_Tuesday_at_21()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        (report.Post.Punch!.Busiest.Day, report.Post.Punch.Busiest.Hour).ShouldBe((4, 20));
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
        report.Post.Punch!.CodingWindow.ShouldBe(new CodingWindow(Start: 9, Length: 14));
    }

    [Fact]
    public void Pre_touches_15_hours_of_the_day()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Punch!.HoursTouched.ShouldBe(14);
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
        Percent(report.Pre.Punch!.Evening).ShouldBe(6.8);
    }

    [Fact]
    public void Pre_commits_5_1_percent_on_weekends()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Pre.Punch!.Weekend).ShouldBe(5.8);
    }

    [Fact]
    public void Pre_commits_12_2_percent_out_of_hours()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Pre.Punch!.Combined).ShouldBe(12.6);
    }

    [Fact]
    public void Post_commits_30_6_percent_on_evenings()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Evening).ShouldBe(30.5);
    }

    [Fact]
    public void Post_commits_24_percent_on_weekends()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Weekend).ShouldBe(24.7);
    }

    [Fact]
    public void Post_commits_54_6_percent_out_of_hours()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        Percent(report.Post.Punch!.Combined).ShouldBe(55.2);
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
        report.Shift.CommitsX.ShouldBe(1_617 / 639d);
    }

    [Fact]
    public void Out_of_hours_grows_about_four_and_a_third_times()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.OutOfHoursX!.Value.ShouldBeInRange(4.35, 4.42);
    }

    [Fact]
    public void The_score_parts_are_20_9_20_and_16()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Parts.ShouldBe(new ScoreParts(Output: 20, Projects: 9, Languages: 20, Time: 16));
    }

    [Fact]
    public void The_score_is_65()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Total.ShouldBe(65);
    }

    [Fact]
    public void The_verdict_is_I_m_a_machine_literally()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Verdict.ShouldBe("I'm a machine (literally)");
    }

    private static double Percent(double share) => Math.Round(share * 100, 1);
}
