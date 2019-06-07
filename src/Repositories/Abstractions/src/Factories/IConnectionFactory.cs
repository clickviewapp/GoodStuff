namespace ClickView.GoodStuff.Repositories.Abstractions.Factories
{
    public interface IConnectionFactory<out TConnection>
    {
        TConnection GetReadConnection();
        TConnection GetWriteConnection();
    }
}
