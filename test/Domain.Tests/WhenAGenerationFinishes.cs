using Shouldly;

namespace Domain.Tests;

public class WhenAGenerationFinishes
{
    [Fact]
    public void A_completed_generation_is_ready()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var completed = generation.Complete(GivenAGeneration.FinishedAt);

        // Assert
        completed.Status.ShouldBe(GenerationStatus.Ready);
    }

    [Fact]
    public void A_generation_with_no_commits_to_compare_is_empty()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var finished = generation.FindNothingToCompare(GivenAGeneration.FinishedAt);

        // Assert
        finished.Status.ShouldBe(GenerationStatus.Empty);
    }

    [Fact]
    public void A_failed_generation_is_failed()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var failed = generation.Fail("GitHub is down.", GivenAGeneration.FinishedAt);

        // Assert
        failed.Status.ShouldBe(GenerationStatus.Failed);
    }

    [Fact]
    public void A_failed_generation_says_why()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var failed = generation.Fail("GitHub is down.", GivenAGeneration.FinishedAt);

        // Assert
        failed.Message.ShouldBe("GitHub is down.");
    }

    [Fact]
    public void A_completed_generation_knows_how_long_it_took()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var completed = generation.Complete(GivenAGeneration.StartedAt.AddSeconds(37));

        // Assert
        completed.Took.ShouldBe(TimeSpan.FromSeconds(37));
    }

    [Fact]
    public void An_empty_generation_knows_how_long_it_took()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var finished = generation.FindNothingToCompare(GivenAGeneration.StartedAt.AddSeconds(12));

        // Assert
        finished.Took.ShouldBe(TimeSpan.FromSeconds(12));
    }

    [Fact]
    public void A_failed_generation_knows_how_long_it_took()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var failed = generation.Fail("GitHub is down.", GivenAGeneration.StartedAt.AddSeconds(5));

        // Assert
        failed.Took.ShouldBe(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void A_pending_generation_has_taken_no_time_yet()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var took = generation.Took;

        // Assert
        took.ShouldBeNull();
    }
}
