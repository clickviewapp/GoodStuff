namespace ClickView.GoodStuff.AspNetCore.Authentication
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IEndpoint
    {
        Task ProcessAsync(HttpContext context, CancellationToken token = default);
    }
}
