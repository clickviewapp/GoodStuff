namespace ClickView.GoodStuff.Repositories.MySql
{
    using Abstractions.Factories;
    using MySqlConnector;

    public interface IMySqlConnectionFactory : IConnectionFactory<MySqlConnection>
    {
    }
}
