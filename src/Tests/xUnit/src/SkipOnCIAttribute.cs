namespace ClickView.GoodStuff.Tests.xUnit
{
    using System;
    using McMaster.Extensions.Xunit;

    /// <summary>
    /// Skip test if running on CI
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SkipOnCIAttribute : Attribute, ITestCondition
    {
        private readonly string _reason;

        public SkipOnCIAttribute(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            _reason = reason;
        }

        public bool IsMet => !IsOnCI();

        public string SkipReason => "This test is skipped on CI. " + _reason;

        public static bool IsOnCI() => IsOnTeamCity() || IsOnAppVeyor();
        public static bool IsOnTeamCity() => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME"));
        public static bool IsOnAppVeyor() => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR"));
    }
}
