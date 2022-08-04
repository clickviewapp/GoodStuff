namespace ClickView.GoodStuff.IdGenerators.IdGen;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using global::IdGen;
using Microsoft.Extensions.Logging;

/// <summary>
/// An Id Generator that generates `<see cref="IdLong"/> ids
/// </summary>
public class IdGenGenerator : Abstractions.IIdGenerator<IdLong>
{
    private readonly IdGenerator _idGenerator;

    /// <summary>
    /// Creates a new instance of <see cref="IdGenGenerator"/>
    /// </summary>
    /// <param name="logger"></param>
    public IdGenGenerator(ILogger<IdGenGenerator> logger)
    {
        var defaultOptions = new IdGeneratorOptions();
        var generatorId = GeneratorIdSource.GetId(defaultOptions.IdStructure.GeneratorIdBits);

        _idGenerator = new IdGenerator(generatorId, defaultOptions);

        logger.LogDebug("GeneratorId: {GeneratorId}", _idGenerator.Id);
    }

    /// <inheritdoc />
    public Task<IdLong> CreateAsync(CancellationToken token = default)
    {
        var id = _idGenerator.CreateId();

        return Task.FromResult(new IdLong(id));
    }
}
