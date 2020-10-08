namespace ClickView.GoodStuff.Repositories.MySql
{
    using MySqlConnector;

    public static class MySqlUtils
    {
        public static bool IsFailoverException(MySqlException mySqlException)
        {
            return mySqlException.Number == (int) MySqlErrorCode.UnableToConnectToHost ||
                   mySqlException.Number == (int) MySqlErrorCode.OptionPreventsStatement ||
                   mySqlException.Number == 0 && mySqlException.HResult == -2147467259;
        }
    }
}
