namespace ClickView.GoodStuff.AspNetCore.VersionPage
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class VersionPageOptions
    {
        /// <summary>
        /// Gets or sets a delegate used to write the response.
        /// </summary>
        public Func<HttpContext, ApplicationInformation, Task>? ResponseWriter { get; set; } =
            VersionPageResponseWriters.WriteMinimalPlaintext;
    }
}
