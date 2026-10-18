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
        var completed = generation.Complete();

        // Assert
        completed.Status.ShouldBe(GenerationStatus.Ready);
    }

    [Fact]
    public void A_generation_with_no_commits_to_compare_is_empty()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var finished = generation.FindNothingToCompare();

        // Assert
        finished.Status.ShouldBe(GenerationStatus.Empty);
    }

    [Fact]
    public void A_failed_generation_is_failed()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var failed = generation.Fail("GitHub is down.");

        // Assert
        failed.Status.ShouldBe(GenerationStatus.Failed);
    }

    [Fact]
    public void A_failed_generation_says_why()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var failed = generation.Fail("GitHub is down.");

        // Assert
        failed.Message.ShouldBe("GitHub is down.");
    }
}
