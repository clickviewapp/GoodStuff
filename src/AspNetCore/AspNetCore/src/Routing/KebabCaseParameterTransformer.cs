namespace ClickView.GoodStuff.AspNetCore.Routing;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Parameter transformer which converts route parameters to kebab-case
/// </summary>
public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    /// <inheritdoc />
    public string? TransformOutbound(object? value)
    {
        var str = value?.ToString();

        if (str is null)
            return null;

        // Slugify value
        return Regex.Replace(str, "([a-z])([A-Z])", "$1-$2");
    }
}
