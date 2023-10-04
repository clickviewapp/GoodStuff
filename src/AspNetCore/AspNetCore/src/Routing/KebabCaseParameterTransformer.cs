namespace ClickView.GoodStuff.AspNetCore.Routing;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Parameter transformer which converts route parameters to kebab-case
/// </summary>
public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    private static readonly Regex ReplacerRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    /// <inheritdoc />
    public string? TransformOutbound(object? value)
    {
        var str = value?.ToString();

        if (str is null)
            return null;

        // Slugify value
        return ReplacerRegex.Replace(str, "$1-$2");
    }
}
