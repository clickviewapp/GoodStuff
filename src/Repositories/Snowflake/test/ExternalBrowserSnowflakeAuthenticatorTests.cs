namespace ClickView.GoodStuff.Repositories.Snowflake.Tests;

using Xunit;

public class ExternalBrowserSnowflakeAuthenticatorTests
{
    [Fact]
    public void SetOptions_EmptyOptions_SetsAuthenticatorA()
    {
        var options = new SnowflakeConnectionOptions();

        new ExternalBrowserSnowflakeAuthenticator().SetOptions(options);

        Assert.Equal("externalbrowser", options.Authenticator);
    }
}
