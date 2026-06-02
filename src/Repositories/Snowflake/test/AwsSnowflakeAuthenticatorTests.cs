namespace ClickView.GoodStuff.Repositories.Snowflake.Tests;

using Xunit;

public class AwsSnowflakeAuthenticatorTests
{
    [Fact]
    public void SetOptions_EmptyOptions_SetsAuthenticatorAndWorkloadIdentityProvider()
    {
        var options = new SnowflakeConnectionOptions();

        new AwsSnowflakeAuthenticator().SetOptions(options);

        Assert.Equal("workload_identity", options.Authenticator);
        Assert.Equal("aws", options.WorkloadIdentityProvider);
    }
}
