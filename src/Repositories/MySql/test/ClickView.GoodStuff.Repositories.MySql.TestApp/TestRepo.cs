namespace ClickView.GoodStuff.Repositories.MySql.TestApp
{
    using System.Threading.Tasks;

    public class TestRepo : BaseMySqlRepository
    {
        public TestRepo(IMySqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public TestRepo(IMySqlConnectionFactory connectionFactory, MySqlRepositoryOptions options) : base(connectionFactory, options)
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
