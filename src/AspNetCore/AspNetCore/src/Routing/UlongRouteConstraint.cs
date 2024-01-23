namespace ClickView.GoodStuff.AspNetCore.Routing;

using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Constrains a route parameter to represent only unsigned 64-bit integer values.
/// </summary>
public class UlongRouteConstraint : IRouteConstraint
{
    /// <inheritdoc />
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        ArgumentNullException.ThrowIfNull(routeKey);
        ArgumentNullException.ThrowIfNull(values);

        if (!values.TryGetValue(routeKey, out var value) || value == null)
            return false;

        switch (value)
        {
            case ulong:
            // If we have a long value, check that its not negative
            case long and > -1:
                return true;
            default:
            {
                var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                return CheckConstraintCore(valueString);
            }
        }
    }

    private static bool CheckConstraintCore(string? valueString) =>
        ulong.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
}
