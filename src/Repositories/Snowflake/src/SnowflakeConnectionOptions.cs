namespace ClickView.GoodStuff.Repositories.Snowflake
{
    using Abstractions;

    public class SnowflakeConnectionOptions : RepositoryConnectionOptions
    {
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
    }
}