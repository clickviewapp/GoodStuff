namespace ClickView.GoodStuff.IdGenerators.IdGen.Tests;

using Xunit;

public class GeneratorIdSourceTests
{
    [Fact]
    public void GetId_IsInBounds()
    {
        var id = GeneratorIdSource.GetId(10);

        Assert.True(id > 0, "Generator is less than 1");
        Assert.True(id < 1023, "Generator is greater than 1023");
    }

    [Fact]
    public void GetId_MatchExpectedNetworkValue()
    {
        var ipAddress = GeneratorIdSource.GetPrivateIpV4();

        Assert.NotNull(ipAddress);

        // Use the string representation for my little bird brain
        var lastOctet = int.Parse(ipAddress.ToString().Split('.')[3]);

        var id = GeneratorIdSource.GetId(8);

        Assert.Equal(lastOctet, id);
    }
}
