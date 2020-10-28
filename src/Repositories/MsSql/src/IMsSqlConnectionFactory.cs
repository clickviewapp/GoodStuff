namespace ClickView.GoodStuff.Repositories.MsSql
{
    using System.Data.SqlClient;
    using Abstractions.Factories;

    public interface IMsSqlConnectionFactory : IConnectionFactory<SqlConnection>
    {
    }
}
