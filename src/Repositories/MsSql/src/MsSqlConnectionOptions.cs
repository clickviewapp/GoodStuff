namespace ClickView.GoodStuff.Repositories.MsSql
{
    using Abstractions;

    public class MsSqlConnectionOptions : RepositoryConnectionOptions
    {
        public MsSqlConnectionOptions()
        {
            // Set some sane defaults
            Host = "localhost";
            SetParameter("Trusted_Connection", "True");
            SetParameter("TrustServerCertificate", "True");
            SetParameter("Encrypt", "True");
            SetParameter("Integrated Security", "False");
        }

        /// <summary>
        /// Specifies the name of the database in use for the connection being established
        /// </summary>
        public string? Database
        {
            set => SetParameter("Database", value);
            get => GetParameter("Database");
        }

        /// <summary>
        /// Specifies the User ID to be used when connecting with SQL Server Authentication
        /// </summary>
        public string? Username
        {
            set => SetParameter("User ID", value);
            get => GetParameter("User ID");
        }

        /// <summary>
        /// Specifies the password associated with the User ID to be used when connecting with SQL Server Authentication
        /// </summary>
        public string? Password
        {
            set => SetParameter("Password", value);
            get => GetParameter("Password");
        }

        /// <summary>
        /// The SQL Server instance to connect to
        /// </summary>
        public override string? Host
        {
            set => SetParameter("Server", value);
            get => GetParameter("Server");
        }
    }
}
