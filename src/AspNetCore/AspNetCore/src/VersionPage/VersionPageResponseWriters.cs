namespace ClickView.GoodStuff.AspNetCore.VersionPage
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal static class VersionPageResponseWriters
    {
        internal static Task WriteMinimalPlaintext(HttpContext httpContext, ApplicationInformation information)
        {
            httpContext.Response.ContentType = "text/plain; charset=utf-8";

            // https://www.ietf.org/rfc/rfc2046.txt (4.1.1)
            // new line as crlf
            const string newLine = "\r\n";

            var response = information.Name + newLine + "Version: " + information.Version;

            return httpContext.Response.WriteAsync(response);
        }
    }
}
