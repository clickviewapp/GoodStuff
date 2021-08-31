namespace ClickView.GoodStuff.AspNetCore.Authentication.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal interface IEndpoint
    {
        Task ProcessAsync(HttpContext context, CancellationToken token = default);
    }
}
