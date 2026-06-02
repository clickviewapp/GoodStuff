namespace ClickView.GoodStuff.Repositories.Snowflake.Tests;

using Xunit;

public class ProgrammaticAccessTokenSnowflakeAuthenticatorTests
{
    [Fact]
    public void SetOptions_EmptyOptions_SetsAuthenticatorAndToken()
    {
        var options = new SnowflakeConnectionOptions();

        new ProgrammaticAccessTokenSnowflakeAuthenticator("my-token").SetOptions(options);

        Assert.Equal("programmatic_access_token", options.Authenticator);
        Assert.Equal("my-token", options.Token);
    }
}
