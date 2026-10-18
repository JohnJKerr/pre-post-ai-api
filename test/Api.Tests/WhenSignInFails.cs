using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSignInFails(ApiFactory factory)
{
    [Fact]
    public async Task They_return_to_the_front_end_s_home_page_told_it_didn_t_finish()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.GetAsync("/api/auth/callback?code=github-code&state=forged");

        // Assert
        callback.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/?error=sign-in-failed");
    }
}
