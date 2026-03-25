namespace Cross.CORS;

/// <summary>
/// CORS configuration (origins, methods, headers, credentials).
/// </summary>
public class CorsConfig
{
    private static readonly char[] AllowedOriginsSeparators = { ',', ' ' };

    /// <summary>
    /// Creates a new CORS configuration.
    /// </summary>
    /// <param name="allowOrigins">Allowed origins (comma or space separated). Use "*" for all origins.</param>
    /// <param name="allowMethods">Allowed HTTP methods (comma separated).</param>
    /// <param name="allowHeaders">Allowed request headers (comma separated).</param>
    /// <param name="allowCredentials">Whether to allow credentials (cookies, authorization headers).</param>
    /// <exception cref="ArgumentException">Thrown when allowOrigins, allowMethods, or allowHeaders is null or empty.</exception>
    public CorsConfig(
        string allowOrigins = "*",
        string allowMethods = "*",
        string allowHeaders = "*",
        bool allowCredentials = false)
    {
        if (string.IsNullOrEmpty(allowOrigins))
        {
            throw new ArgumentException("Argument cannot be null or empty.", nameof(allowOrigins));
        }

        if (string.IsNullOrEmpty(allowMethods))
        {
            throw new ArgumentException("Argument cannot be null or empty.", nameof(allowMethods));
        }

        if (string.IsNullOrEmpty(allowHeaders))
        {
            throw new ArgumentException("Argument cannot be null or empty.", nameof(allowHeaders));
        }

        AllowOrigins = allowOrigins;
        AllowMethods = allowMethods;
        AllowHeaders = allowHeaders;
        AllowCredentials = allowCredentials;
    }

    /// <summary>
    /// Allowed origins (comma or space separated). Use "*" for all origins.
    /// </summary>
    public string AllowOrigins { get; set; }

    /// <summary>
    /// Allowed HTTP methods (comma separated).
    /// </summary>
    public string AllowMethods { get; set; }

    /// <summary>
    /// Allowed request headers (comma separated).
    /// </summary>
    public string AllowHeaders { get; set; }

    /// <summary>
    /// Whether to allow credentials (cookies, authorization headers).
    /// </summary>
    public bool AllowCredentials { get; set; }

    /// <summary>
    /// Parses AllowOrigins into a list of origins. Supports comma or space as separators.
    /// </summary>
    /// <returns>List of trimmed origin strings.</returns>
    public IReadOnlyList<string> GetAllowedOrigins()
    {
        return AllowOrigins
            .Split(AllowedOriginsSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length > 0)
            .ToArray();
    }

    /// <summary>
    /// Returns the effective origin for the Access-Control-Allow-Origin header.
    /// If requestOrigin is in the allowed list, echoes it; otherwise returns "*" or the first allowed origin.
    /// </summary>
    /// <param name="requestOrigin">The Origin header value from the request, or null.</param>
    /// <returns>The origin to set in Access-Control-Allow-Origin response header.</returns>
    public string GetEffectiveOrigin(string? requestOrigin)
    {
        var origins = GetAllowedOrigins();
        if (origins.Count == 0)
        {
            return "*";
        }

        if (origins.Count == 1 && origins[0] == "*")
        {
            return "*";
        }

        var trimmedRequestOrigin = requestOrigin?.Trim();
        if (!string.IsNullOrEmpty(trimmedRequestOrigin) && origins.Contains(trimmedRequestOrigin, StringComparer.OrdinalIgnoreCase))
        {
            return trimmedRequestOrigin!;
        }

        return origins[0];
    }
}
