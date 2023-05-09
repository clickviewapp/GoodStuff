namespace ClickView.GoodStuff.Repositories.Snowflake
{
    using Abstractions.Factories;
    using global::Snowflake.Data.Client;

    public interface ISnowflakeConnectionFactory : IConnectionFactory<SnowflakeDbConnection>
    {
    }
}