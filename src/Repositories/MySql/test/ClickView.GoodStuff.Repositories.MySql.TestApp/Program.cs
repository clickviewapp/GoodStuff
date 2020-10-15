namespace ClickView.GoodStuff.Repositories.MySql.TestApp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions.Factories;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class Program
    {
        private const string Host = "";
        private const string RootUser = "";
        private const string RootPwd = "";

        private static async Task Main(string[] args)
        {
            await SetupAsync();

            var services = new ServiceCollection();
            services.AddLogging(o => o.AddConsole());

            var connection = new MySqlConnectionOptions
            {
                Host = Host,
                Username = "testusr",
                Password = "testpw",
                Database = "test_db"
            };

            var connFactory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MySqlConnectionOptions>
            {
                Write = connection,
                Read = connection
            });

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            var testRepo = new TestRepo(connFactory, new MySqlRepositoryOptions
            {
                FailOverRetryCount = 10,
                LoggerFactory = loggerFactory
            });

            logger.LogInformation("Starting...");

            if (args.Contains("-write"))
            {
                while (true)
                {
                    logger.LogInformation("Writing...");

                    await testRepo.WriteAsync("INSERT IGNORE INTO `test_table` VALUES (1);");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            if (args.Contains("-read"))
            {
                while (true)
                {
                    logger.LogInformation("Reading...");

                    await testRepo.ReadAsync<int>("SELECT * FROM `test_table`;");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        private static async Task SetupAsync()
        {
            var connFactory = new MySqlConnectionFactory(new ConnectionFactoryOptions<MySqlConnectionOptions>
            {
                Write = new MySqlConnectionOptions
                {
                    Host = Host,
                    Username = RootUser,
                    Password = RootPwd
                }
            });

            var testRepo = new TestRepo(connFactory);

            await testRepo.WriteAsync("CREATE SCHEMA IF NOT EXISTS test_db; " +
                                      "CREATE TABLE IF NOT EXISTS `test_db`.`test_table` (id int PRIMARY KEY);" +
                                      "CREATE USER IF NOT EXISTS 'testusr'@'%' IDENTIFIED BY 'testpw';" +
                                      "GRANT INSERT,SELECT ON test_db.test_table TO 'testusr'@'%';" +
                                      "FLUSH PRIVILEGES;"
            );
        }
    }
}
