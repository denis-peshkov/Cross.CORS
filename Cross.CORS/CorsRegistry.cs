namespace Cross.CORS;

public static class CorsRegistry
{
    public static readonly string CorsConfigKey = "crossCORS";

    private static CorsConfig? _config;

    public static void Register(CorsConfig cors)
    {
        ArgumentNullException.ThrowIfNull(cors);

        _config = cors;
    }

    internal static CorsConfig? GetConfig() => _config;
}
