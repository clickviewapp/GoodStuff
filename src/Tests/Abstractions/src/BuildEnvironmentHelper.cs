namespace ClickView.GoodStuff.Tests.Abstractions;

using System;

/// <summary>
/// Helper class for determining the build environment
/// </summary>
public static class BuildEnvironmentHelper
{
    /// <summary>
    /// Checks to see if the current environment is any known build server
    /// </summary>
    /// <returns></returns>
    public static bool IsBuildEnvironment() => IsOnTeamCity() || IsOnAppVeyor();

    /// <summary>
    /// Checks to see if the current environment is TeamCity
    /// </summary>
    /// <returns></returns>
    public static bool IsOnTeamCity() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME"));

    /// <summary>
    /// Checks to see if the current environment is AppVeyor
    /// </summary>
    /// <returns></returns>
    public static bool IsOnAppVeyor() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR"));
}
