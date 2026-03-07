namespace Cross.CORS;

public class CorsConfig
{
    private static readonly char[] AllowedOriginsSeparators = { ',', ' ' };

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

    public string AllowOrigins { get; set; }

    public string AllowMethods { get; set; }

    public string AllowHeaders { get; set; }

    public bool AllowCredentials { get; set; }

    /// <summary>
    /// Parses AllowOrigins into a list of origins. Supports comma or space as separators.
    /// </summary>
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
