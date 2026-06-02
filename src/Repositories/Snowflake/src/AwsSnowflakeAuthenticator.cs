namespace ClickView.GoodStuff.Repositories.Snowflake;

public class AwsSnowflakeAuthenticator : ISnowflakeAuthenticator
{
    public static AwsSnowflakeAuthenticator Instance { get; } = new();

    public void SetOptions(SnowflakeConnectionOptions options)
    {
        options.Authenticator = "workload_identity";
        options.WorkloadIdentityProvider = "aws";
    }
}
