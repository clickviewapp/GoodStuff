namespace ClickView.GoodStuff.IdGenerators.IdGen.Tests;

using System.Threading.Tasks;
using Abstractions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class IdGenGeneratorTests
{
    [Fact]
    public async Task CreateId_IsNotEmpty()
    {
        var generator = new IdGenGenerator(new NullLogger<IdGenGenerator>());

        var id = await generator.CreateAsync();

        Assert.NotEqual(IdLong.Empty, id);
    }
}
