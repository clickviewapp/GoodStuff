namespace ClickView.GoodStuff.Tests.xUnit
{
    using System;
    using Abstractions;
    using McMaster.Extensions.Xunit;

    /// <summary>
    /// Skip test if running on CI
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    // ReSharper disable once InconsistentNaming
    public class SkipOnCIAttribute : Attribute, ITestCondition
    {
        private readonly string _reason;

        public SkipOnCIAttribute(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            _reason = reason;
        }

        public bool IsMet => !BuildEnvironmentHelper.IsBuildEnvironment();

        public string SkipReason => "This test is skipped on CI. " + _reason;
    }
}
