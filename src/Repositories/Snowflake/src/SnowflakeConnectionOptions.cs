namespace ClickView.GoodStuff.Repositories.Snowflake;

using Abstractions;

public class SnowflakeConnectionOptions : RepositoryConnectionOptions
{
    public SnowflakeConnectionOptions()
    {
    }

    public SnowflakeConnectionOptions(ISnowflakeAuthenticator authenticator)
    {
        UseAuthenticator(authenticator);
    }

    /// <summary>
    /// The full account name which might include additional segments that identify the region and
    /// cloud platform where your account is hosted
    /// </summary>
    public string? Account
    {
        set => SetParameter("account", value);
        get => GetParameter("account");
    }

    /// <summary>
    /// The name of the warehouse to use
    /// </summary>
    public string? Warehouse
    {
        set => SetParameter("warehouse", value);
        get => GetParameter("warehouse");
    }

    /// <summary>
    /// The database to use
    /// </summary>
    public string? Database
    {
        set => SetParameter("db", value);
        get => GetParameter("db");
    }

    /// <summary>
    /// The schema to use
    /// </summary>
    public string? Schema
    {
        set => SetParameter("schema", value);
        get => GetParameter("schema");
    }

    /// <summary>
    /// The Snowflake user to use
    /// </summary>
    public string? User
    {
        set => SetParameter("user", value);
        get => GetParameter("user");
    }

    /// <summary>
    /// The password for the Snowflake user
    /// </summary>
    public string? Password
    {
        set => SetParameter("password", value);
        get => GetParameter("password");
    }

    public string? Authenticator
    {
        set => SetParameter("authenticator", value);
        get => GetParameter("authenticator");
    }

    /// <summary>
    /// Used to set the name of the cloud provider when authenticator=workload_identity
    /// </summary>
    public string? WorkloadIdentityProvider
    {
        set => SetParameter("workload_identity_provider", value);
        get => GetParameter("workload_identity_provider");
    }

    /// <summary>
    /// Used to set the programmatic access token (PAT) when authenticator=programmatic_access_token
    /// </summary>
    public string? Token
    {
        set => SetParameter("token", value);
        get => GetParameter("token");
    }

    public void UseAuthenticator(ISnowflakeAuthenticator authenticator)
    {
        authenticator.SetOptions(this);
    }
}
