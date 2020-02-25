namespace ClickView.GoodStuff.Repositories.MySql
{
    using Abstractions.Factories;
    using global::MySql.Data.MySqlClient;

    public interface IMySqlConnectionFactory : IConnectionFactory<MySqlConnection>
    {
    }
}
