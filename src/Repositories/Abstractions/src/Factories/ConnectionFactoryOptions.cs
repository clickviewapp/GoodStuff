namespace ClickView.GoodStuff.Repositories.Abstractions.Factories
{
    public class ConnectionFactoryOptions<TOptions> where TOptions : RepositoryConnectionOptions
    {
        public TOptions Read { get; set; }
        public TOptions Write { get; set; }
    }
}
