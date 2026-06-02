namespace ClickView.GoodStuff.Repositories.Snowflake.Tests;

using Xunit;

public class SnowflakeConnectionOptionsTests
{
    [Fact]
    public void GetConnectionString_Default_Empty()
    {
        var options = new SnowflakeConnectionOptions();

        var connString = options.GetConnectionString();

        Assert.Equal(string.Empty, connString);
    }

    [Fact]
    public void GetConnectionString_AllOptionsSet_Valid()
    {
        var options = new SnowflakeConnectionOptions
        {
            Host = "host",
            Account = "acc",
            Warehouse = "wh",
            Database = "db",
            Schema = "sch",
            User = "user",
            Password = "pass",
            Authenticator = "auth",
            WorkloadIdentityProvider = "aws",
            Token = "tok"
        };

        var connString = options.GetConnectionString();

        Assert.Equal(
            "host=host;" +
            "account=acc;" +
            "warehouse=wh;" +
            "db=db;" +
            "schema=sch;" +
            "user=user;" +
            "password=pass;" +
            "authenticator=auth;" +
            "workload_identity_provider=aws;" +
            "token=tok;",
            connString);
    }

    [Fact]
    public void PropertiesSet_AllOptionsSet_Valid()
    {
        var options = new SnowflakeConnectionOptions
        {
            Host = "host",
            Account = "acc",
            Warehouse = "wh",
            Database = "db",
            Schema = "sch",
            User = "user",
            Password = "pass",
            Authenticator = "auth",
            WorkloadIdentityProvider = "aws",
            Token = "tok"
        };

        Assert.Equal("host", options.Host);
        Assert.Equal("acc", options.Account);
        Assert.Equal("wh", options.Warehouse);
        Assert.Equal("db", options.Database);
        Assert.Equal("sch", options.Schema);
        Assert.Equal("user", options.User);
        Assert.Equal("pass", options.Password);
        Assert.Equal("auth", options.Authenticator);
        Assert.Equal("aws", options.WorkloadIdentityProvider);
        Assert.Equal("tok", options.Token);
    }

    [Fact]
    public void PropertiesSet_Default_Empty()
    {
        var options = new SnowflakeConnectionOptions();

        Assert.Null(options.Host);
        Assert.Null(options.Account);
        Assert.Null(options.Warehouse);
        Assert.Null(options.Database);
        Assert.Null(options.Schema);
        Assert.Null(options.User);
        Assert.Null(options.Password);
        Assert.Null(options.Authenticator);
        Assert.Null(options.WorkloadIdentityProvider);
        Assert.Null(options.Token);
    }

    [Fact]
    public void PropertiesSet_Null_Empty()
    {
        var options = new SnowflakeConnectionOptions
        {
            Host = null,
            Account = null,
            Warehouse = null,
            Database = null,
            Schema = null,
            User = null,
            Password = null,
            Authenticator = null,
            WorkloadIdentityProvider = null,
            Token = null
        };

        Assert.Null(options.Host);
        Assert.Null(options.Account);
        Assert.Null(options.Warehouse);
        Assert.Null(options.Database);
        Assert.Null(options.Schema);
        Assert.Null(options.User);
        Assert.Null(options.Password);
        Assert.Null(options.Authenticator);
        Assert.Null(options.WorkloadIdentityProvider);
        Assert.Null(options.Token);
    }

    [Fact]
    public void Constructor_WithAuthenticator_AppliesAuthenticator()
    {
        var options = new SnowflakeConnectionOptions(ExternalBrowserSnowflakeAuthenticator.Instance);

        Assert.Equal("externalbrowser", options.Authenticator);
    }

    [Fact]
    public void UseAuthenticator_WithAuthenticator_AppliesAuthenticator()
    {
        var options = new SnowflakeConnectionOptions();

        options.UseAuthenticator(ExternalBrowserSnowflakeAuthenticator.Instance);

        Assert.Equal("externalbrowser", options.Authenticator);
    }
}
