using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace Microsoft.AspNetCore.Builder;

public static class PowCapServerApplicationBuilderExtensions
{
    public static IApplicationBuilder MapPowCapServer(this IApplicationBuilder app, string endpointPrefix = "/api/captcha")
    {
        app.UseEndpoints(endpoints =>
        {
            var trimmedPrefix = endpointPrefix?.Trim('/');
            var challengeEndpoint = string.IsNullOrEmpty(trimmedPrefix) ? "/challenge" : $"/{trimmedPrefix}/challenge";
            var redeemEndpoint = string.IsNullOrEmpty(trimmedPrefix) ? "/redeem" : $"/{trimmedPrefix}/redeem";

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
