namespace ClickView.GoodStuff.AspNetCore.Tests
{
    using System;
    using Xunit;

    public class ApplicationInformationTests
    {
        [Fact]
        public void FromAssembly_Null_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ApplicationInformation.FromAssembly(null!));
        }
    }
}
