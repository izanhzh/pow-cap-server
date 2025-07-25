using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PowCapServer;
using PowCapServer.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class PowCapServerServiceCollectionExtensions
{
    public static IServiceCollection AddPowCapServer(this IServiceCollection services)
    {
        return services
            .AddPowCapServer(options =>
            {
                options.ChallengeCount = 50;
                options.ChallengeSize = 32;
                options.ChallengeDifficulty = 4;
                options.ChallengeTokenExpiresMs = 600000;
                options.CaptchaTokenExpiresMs = 1200000;
            });
    }

    public static IServiceCollection AddPowCapServer(this IServiceCollection services, Action<PowCapServerOptions> options)
    {
        services.AddDistributedMemoryCache();
        services.TryAddSingleton<ICaptchaService, DefaultCaptchaService>();
        services.TryAddSingleton<ICaptchaStore, DefaultCaptchaStore>();
        services.Configure(options);
        return services;
    }
}
