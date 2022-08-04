namespace ClickView.GoodStuff.IdGenerators.IdGen;

using System;
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
        // Define our own new epoch
        var epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Create default options
        var defaultOptions = new IdGeneratorOptions(IdStructure.Default, new DefaultTimeSource(epoch));

        // Create our generator id
        var generatorId = GeneratorIdSource.GetId(defaultOptions.IdStructure.GeneratorIdBits);

        // Log for debug purposes
        logger.LogDebug("GeneratorId: {GeneratorId}", generatorId);

        _idGenerator = new IdGenerator(generatorId, defaultOptions);
    }

    /// <inheritdoc />
    public Task<IdLong> CreateAsync(CancellationToken token = default)
    {
        var id = _idGenerator.CreateId();

        return Task.FromResult(new IdLong(id));
    }
}
