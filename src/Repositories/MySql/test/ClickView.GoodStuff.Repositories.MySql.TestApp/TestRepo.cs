namespace ClickView.GoodStuff.Repositories.MySql.TestApp
{
    using System.Threading.Tasks;
    using Abstractions.Factories;
    using MySqlConnector;

    public class TestRepo : BaseMySqlRepository
    {
        public TestRepo(IConnectionFactory<MySqlConnection> connectionFactory) : base(connectionFactory)
        {
        }

        public TestRepo(IConnectionFactory<MySqlConnection> connectionFactory, MySqlRepositoryOptions options) : base(connectionFactory, options)
        {
        }

        public Task ReadAsync<T>(string sql)
        {
            return QueryAsync<T>(sql);
        }

        public Task WriteAsync(string sql)
        {
            return ExecuteAsync(sql);
        }
    }
}
