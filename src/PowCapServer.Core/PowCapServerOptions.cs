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
    /// <param name="type">The type of PoW Captcha for which the configuration is requested. If null, no specific type is queried.</param>
    /// <returns>
    /// Returns the configuration for the specified type if it exists in <see cref="TypeConfigs"/>. 
    /// Returns null if <see cref="TypeConfigs"/> is null, the specified type is null, or the type is not found in the dictionary.
    /// </returns>
    public PowCapConfig? GetPowCapConfig(string? type)
    {
        if (TypeConfigs == null || type == null || !TypeConfigs.TryGetValue(type, out var config))
        {
            return null;
        }
        return config;
    }
}
