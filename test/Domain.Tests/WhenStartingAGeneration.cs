using Shouldly;

namespace Domain.Tests;

public class WhenStartingAGeneration
{
    [Fact]
    public void It_is_pending()
    {
        // Act
        var generation = new GivenAGeneration().Build();

        // Assert
        generation.Status.ShouldBe(GenerationStatus.Pending);
    }

    [Fact]
    public void It_gets_an_id_nobody_can_guess()
    {
        // Act
        var generation = new GivenAGeneration().Build();

        // Assert
        generation.Id.Length.ShouldBe(32);
    }

    [Fact]
    public void Each_generation_gets_its_own_id()
    {
        // Arrange
        var first = new GivenAGeneration().Build();

        // Act
        var second = new GivenAGeneration().Build();

        // Assert
        second.Id.ShouldNotBe(first.Id);
    }
}
