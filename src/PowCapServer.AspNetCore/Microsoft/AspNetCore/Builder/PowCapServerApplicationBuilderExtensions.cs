using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PowCapServer;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace Microsoft.AspNetCore.Builder;

public static class PowCapServerApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePowCapServerEndpoints(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            var endpointPrefix = app.ApplicationServices.GetRequiredService<IOptions<PowCapServerOptions>>().Value.EndpointPrefix.Trim('/');
            var challengeEndpoint = $"/{endpointPrefix}/challenge";
            var redeemEndpoint = $"/{endpointPrefix}/redeem";

            endpoints.MapPost(challengeEndpoint, async context =>
            {
                var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
                var challengeTokenInfo = await captchaService.CreateChallengeAsync().ConfigureAwait(false);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(challengeTokenInfo).ConfigureAwait(false);
            });

            endpoints.MapPost(redeemEndpoint, async context =>
            {
                var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
                var request = await context.Request.ReadFromJsonAsync<ChallengeSolution>().ConfigureAwait(false);
                if (request == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid request").ConfigureAwait(false);
                    return;
                }

                var result = await captchaService.RedeemChallengeAsync(request).ConfigureAwait(false);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(result).ConfigureAwait(false);
            });
        });

        return app;
    }
}
