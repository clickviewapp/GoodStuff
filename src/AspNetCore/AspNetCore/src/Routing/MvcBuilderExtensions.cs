namespace ClickView.GoodStuff.AspNetCore.Routing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder UseKebabCaseParameterTransformer(this IMvcBuilder builder)
    {
        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer(true)));
        });

        return builder;
    }
}
