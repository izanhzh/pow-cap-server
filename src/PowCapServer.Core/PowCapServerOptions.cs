using System.Collections.Generic;

namespace PowCapServer;

public class PowCapServerOptions
{
    /// <summary>
    /// Default configuration for not specified PoW Captcha types
    /// </summary>
    public PowCapConfig Default { get; set; } = new PowCapConfig();

    /// <summary>
    /// Dictionary of configurations for different types of PoW Captcha
    /// </summary>
    public Dictionary<string, PowCapConfig>? TypeConfigs { get; set; }

    /// <summary>
    /// Gets the configuration for a specific type of PoW Captcha.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public PowCapConfig? GetPowCapConfig(string? type)
    {
        if (TypeConfigs == null || type == null || !TypeConfigs.TryGetValue(type, out var config))
        {
            return null;
        }
        return config;
    }
}
