using Shouldly;

namespace Domain.Tests;

public class WhenAGenerationIsPendingForTooLong
{
    [Fact]
    public void It_is_still_pending_within_30_minutes()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var current = generation.AsOf(GivenAGeneration.StartedAt.AddMinutes(29));

        // Assert
        current.Status.ShouldBe(GenerationStatus.Pending);
    }

    [Fact]
    public void After_30_minutes_it_has_failed()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var current = generation.AsOf(GivenAGeneration.StartedAt.AddMinutes(31));

        // Assert
        current.Status.ShouldBe(GenerationStatus.Failed);
    }

    [Fact]
    public void It_says_it_was_interrupted()
    {
        // Arrange
        var generation = new GivenAGeneration().Build();

        // Act
        var current = generation.AsOf(GivenAGeneration.StartedAt.AddMinutes(31));

        // Assert
        current.Message.ShouldBe("Something interrupted your report. Sign in to try again.");
    }

    [Fact]
    public void A_finished_generation_is_left_as_it_is()
    {
        // Arrange
        var generation = new GivenAGeneration().Build().Complete(GivenAGeneration.FinishedAt);

        // Act
        var current = generation.AsOf(GivenAGeneration.StartedAt.AddDays(1));

        // Assert
        current.Status.ShouldBe(GenerationStatus.Ready);
    }
}
