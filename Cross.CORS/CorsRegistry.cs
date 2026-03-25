namespace Cross.CORS;

/// <summary>
/// Static registry for global CORS configuration.
/// </summary>
public static class CorsRegistry
{
    /// <summary>
    /// Configuration key used in error messages.
    /// </summary>
    public static readonly string CorsConfigKey = "crossCORS";

    private static CorsConfig? _config;

    /// <summary>
    /// Registers the global CORS configuration. Must be called before handling requests when using the attribute without parameters.
    /// </summary>
    /// <param name="cors">The CORS configuration to use globally.</param>
    /// <exception cref="ArgumentNullException">Thrown when cors is null.</exception>
    public static void Register(CorsConfig cors)
    {
        ArgumentNullException.ThrowIfNull(cors);

        _config = cors;
    }

    internal static CorsConfig? GetConfig() => _config;
}
