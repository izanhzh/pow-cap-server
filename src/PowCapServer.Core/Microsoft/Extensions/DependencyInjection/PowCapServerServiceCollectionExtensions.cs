using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PowCapServer;
using PowCapServer.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class PowCapServerServiceCollectionExtensions
{
    public static IServiceCollection AddPowCapServer(this IServiceCollection services, Action<PowCapServerOptions>? options = null)
    {
        services.AddDistributedMemoryCache();
        services.TryAddSingleton<ICaptchaService, DefaultCaptchaService>();
        services.TryAddSingleton<ICaptchaStore, DefaultCaptchaStore>();
        services.Configure<PowCapServerOptions>(opts => options?.Invoke(opts));
        return services;
    }
}
