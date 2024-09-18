namespace ClickView.GoodStuff.AspNetCore.Routing;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Parameter transformer which converts route parameters to kebab-case
/// </summary>
public partial class KebabCaseParameterTransformer(bool lowercase = false) : IOutboundParameterTransformer
{
    private static readonly Regex ReplacerRegex = KebabRegex();

    /// <inheritdoc />
    public string? TransformOutbound(object? value)
    {
        var str = value?.ToString();

        if (str is null)
            return null;

        // Slugify value
        var kebabCase = ReplacerRegex.Replace(str, "$1-$2");

        // ToLower?
        return lowercase ? kebabCase.ToLower() : kebabCase;
    }

    [GeneratedRegex("([a-z])([A-Z])", RegexOptions.Compiled)]
    private static partial Regex KebabRegex();
}
