namespace ClickView.GoodStuff.Repositories.MsSql
{
    using Abstractions.Factories;
    using Microsoft.Data.SqlClient;

    public interface IMsSqlConnectionFactory : IConnectionFactory<SqlConnection>;
}
