namespace Cross.CORS;

/// <summary>
/// Options for CORS configuration, typically bound from appsettings.json "Cors" section.
/// </summary>
/// <example>
/// <code>
/// // appsettings.json
/// // {
/// //   "Cors": {
/// //     "AllowOrigins": "https://example.com, https://trusted-app.com",
/// //     "AllowMethods": "GET, POST, PUT, DELETE, OPTIONS",
/// //     "AllowHeaders": "Content-Type, Authorization",
/// //     "AllowCredentials": true
/// //   }
/// // }
///
/// var corsOptions = new CorsOptions();
/// builder.Configuration.GetSection(CorsOptions.ConfigSection).Bind(corsOptions);
/// CorsRegistry.Register(corsOptions.ToCorsConfig());
/// </code>
/// </example>
public class CorsOptions
{
    /// <summary>
    /// Configuration section name for CORS options in appsettings.json.
    /// Use with <c>IConfiguration.GetSection(CorsOptions.ConfigSection)</c>.
    /// </summary>
    public const string ConfigSection = "Cors";

    /// <summary>
    /// Allowed origins (comma or space separated). Use "*" for all origins.
    /// </summary>
    public string AllowOrigins { get; set; } = "*";

    /// <summary>
    /// Allowed HTTP methods (comma separated).
    /// </summary>
    public string AllowMethods { get; set; } = "*";

    /// <summary>
    /// Allowed request headers (comma separated).
    /// </summary>
    public string AllowHeaders { get; set; } = "*";

    /// <summary>
    /// Whether to allow credentials (cookies, authorization headers).
    /// </summary>
    public bool AllowCredentials { get; set; }

    /// <summary>
    /// Converts options to CorsConfig for use with CorsRegistry.Register().
    /// </summary>
    public CorsConfig ToCorsConfig()
    {
        return new CorsConfig(
            allowOrigins: string.IsNullOrEmpty(AllowOrigins) ? "*" : AllowOrigins,
            allowMethods: string.IsNullOrEmpty(AllowMethods) ? "*" : AllowMethods,
            allowHeaders: string.IsNullOrEmpty(AllowHeaders) ? "*" : AllowHeaders,
            allowCredentials: AllowCredentials);
    }
}
