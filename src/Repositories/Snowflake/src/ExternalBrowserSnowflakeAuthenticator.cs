namespace ClickView.GoodStuff.Repositories.Snowflake;

public class ExternalBrowserSnowflakeAuthenticator : ISnowflakeAuthenticator
{
    public static ExternalBrowserSnowflakeAuthenticator Instance { get; } = new();

    public void SetOptions(SnowflakeConnectionOptions options)
    {
        options.Authenticator = "externalbrowser";
    }
}
