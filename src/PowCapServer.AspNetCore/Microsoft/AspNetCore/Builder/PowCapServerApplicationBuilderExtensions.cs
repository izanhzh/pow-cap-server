using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace Microsoft.AspNetCore.Builder;

public static class PowCapServerApplicationBuilderExtensions
{
    private const string ErrorInvalidUseCase = "Invalid use case";
    private const string ErrorInvalidRequest = "Invalid request";

    private static readonly Regex ValidUseCasePattern = new(@"^[a-zA-Z0-9_-]{1,50}$", RegexOptions.Compiled);

    public static IApplicationBuilder MapPowCapServer(this IApplicationBuilder app, string endpointPrefix = "/api/captcha")
    {
        app.UseEndpoints(endpoints =>
        {
            var trimmedPrefix = endpointPrefix?.Trim('/');
            var challengeEndpoint = string.IsNullOrEmpty(trimmedPrefix) ? "/challenge" : $"/{trimmedPrefix}/challenge";
            var challengeEndpointWithUseCase = string.IsNullOrEmpty(trimmedPrefix) ? "/{useCase}/challenge" : $"/{trimmedPrefix}/{{useCase}}/challenge";
            var redeemEndpoint = string.IsNullOrEmpty(trimmedPrefix) ? "/redeem" : $"/{trimmedPrefix}/redeem";
            var redeemEndpointWithUseCase = string.IsNullOrEmpty(trimmedPrefix) ? "/{useCase}/redeem" : $"/{trimmedPrefix}/{{useCase}}/redeem";

            endpoints.MapPost(challengeEndpoint, async context =>
            {
                var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
                var challengeTokenInfo = await captchaService.CreateChallengeAsync().ConfigureAwait(false);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(challengeTokenInfo).ConfigureAwait(false);
            });

            endpoints.MapPost(challengeEndpointWithUseCase, async context =>
            {
                var useCase = context.Request.RouteValues["useCase"]?.ToString();
                if (useCase == null || !ValidUseCasePattern.IsMatch(useCase))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(ErrorInvalidUseCase).ConfigureAwait(false);
                    return;
                }

                var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
                var challengeTokenInfo = await captchaService.CreateChallengeAsync(useCase).ConfigureAwait(false);
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
                    await context.Response.WriteAsync(ErrorInvalidRequest).ConfigureAwait(false);
                    return;
                }

                var result = await captchaService.RedeemChallengeAsync(request).ConfigureAwait(false);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(result).ConfigureAwait(false);
            });

            endpoints.MapPost(redeemEndpointWithUseCase, async context =>
            {
                var useCase = context.Request.RouteValues["useCase"]?.ToString();
                if (useCase == null || !ValidUseCasePattern.IsMatch(useCase))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(ErrorInvalidUseCase).ConfigureAwait(false);
                    return;
                }

                var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
                var request = await context.Request.ReadFromJsonAsync<ChallengeSolution>().ConfigureAwait(false);
                if (request == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(ErrorInvalidRequest).ConfigureAwait(false);
                    return;
                }

                var result = await captchaService.RedeemChallengeAsync(useCase, request).ConfigureAwait(false);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(result).ConfigureAwait(false);
            });
        });

        return app;
    }
}
