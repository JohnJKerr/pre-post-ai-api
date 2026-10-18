using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningInWithAnInvalidCutoff(ApiFactory factory)
{
    [Fact]
    public async Task A_date_that_doesn_t_exist_is_sent_back_to_the_front_end()
    {
        // Arrange
        var browser = ABrowser();

        // Act
        var response = await browser.GetAsync("/api/auth/sign-in?cutoff=2025-02-30");

        // Assert
        response.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/generate?error=invalid-cutoff");
    }

    [Fact]
    public async Task A_cutoff_without_a_full_week_since_is_sent_back_to_the_front_end()
    {
        // Arrange
        var browser = ABrowser();

        // Act
        var response = await browser.GetAsync("/api/auth/sign-in?cutoff=2026-10-18");

        // Assert
        response.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/generate?error=cutoff-too-recent");
    }

    private HttpClient ABrowser() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });
}
