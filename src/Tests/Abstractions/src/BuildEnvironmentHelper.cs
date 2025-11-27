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
    public static bool IsBuildEnvironment() => IsOnTeamCity() || IsOnAppVeyor() || IsOnGitHubActions() || IsOnGitLabCI();

    /// <summary>
    /// Checks to see if the current environment is TeamCity
    /// </summary>
    /// <returns></returns>
    public static bool IsOnTeamCity() => HasEnvironmentVariable("TEAMCITY_PROJECT_NAME");

    /// <summary>
    /// Checks to see if the current environment is AppVeyor
    /// </summary>
    /// <returns></returns>
    public static bool IsOnAppVeyor() => HasEnvironmentVariable("APPVEYOR");

    /// <summary>
    /// Checks to see if the current environment is GitHub actions
    /// </summary>
    /// <returns></returns>
    public static bool IsOnGitHubActions() => HasEnvironmentVariable("GITHUB_ACTIONS");

    /// <summary>
    /// Checks to see if the current environment is GitLab CI
    /// </summary>
    /// <returns></returns>
    public static bool IsOnGitLabCI() => HasEnvironmentVariable("GITLAB_CI");

    private static bool HasEnvironmentVariable(string name) =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name));
}
