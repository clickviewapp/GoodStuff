namespace ClickView.GoodStuff.Repositories.Snowflake;

public class ProgrammaticAccessTokenSnowflakeAuthenticator : ISnowflakeAuthenticator
{
    private readonly string _token;

    public ProgrammaticAccessTokenSnowflakeAuthenticator(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        _token = token;
    }

    public void SetOptions(SnowflakeConnectionOptions options)
    {
        options.Authenticator = "programmatic_access_token";
        options.Token = _token;
    }
}
