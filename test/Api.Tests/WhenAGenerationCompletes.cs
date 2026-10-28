using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenAGenerationCompletes(ApiFactory factory)
{
    [Fact]
    public async Task Its_fetch_assembly_and_save_times_are_logged()
    {
        // Arrange
        var log = new GivenAGenerationLog();
        var browser = factory.CreateBrowserSigningInWithGitHub(
            gitHubOAuth: FakeGitHubOAuth.SigningInAs("timed-dev", 10_701),
            generationLog: log);

        // Act
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Assert
        log.Entries.ShouldContain(entry =>
            entry.Any(p => p.Key == "FetchMs") &&
            entry.Any(p => p.Key == "AssemblyMs") &&
            entry.Any(p => p.Key == "SaveMs"));
    }
}
