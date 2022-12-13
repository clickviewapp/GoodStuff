namespace ClickView.GoodStuff.Clients.Braze.Models;

public class BrazeUserAlias
{
    public BrazeUserAlias(string aliasName, string aliasLabel)
    {
        AliasName = aliasName;
        AliasLabel = aliasLabel;
    }

    public string AliasName { get; set; }
    public string AliasLabel { get; set; }
}