namespace Cross.CORS;

/// <summary>
/// Action filter attribute that adds CORS headers to the response.
/// Supports global configuration via <see cref="CorsRegistry.Register"/> or local configuration via constructor parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllowCrossOriginResourceSharingAttribute : ActionFilterAttribute
{
    private const string AllowOriginHeader = "Access-Control-Allow-Origin";
    private const string AllowMethodsHeader = "Access-Control-Allow-Methods";
    private const string AllowHeadersHeader = "Access-Control-Allow-Headers";
    private const string AllowCredentialsHeader = "Access-Control-Allow-Credentials";

    private readonly string? _allowOrigins;
    private readonly string? _allowMethods;
    private readonly string? _allowHeaders;
    private readonly bool _allowCredentials;
    private readonly bool _overrideGlobals;

    /// <summary>
    /// Initializes the attribute to use global configuration from <see cref="CorsRegistry.Register"/>.
    /// </summary>
    public AllowCrossOriginResourceSharingAttribute() => _overrideGlobals = false;

    /// <summary>
    /// Initializes the attribute with local CORS configuration.
    /// </summary>
    /// <param name="allowOrigins">Allowed origins (comma or space separated). Use "*" for all origins.</param>
    /// <param name="allowMethods">Allowed HTTP methods (comma separated). Default is "*".</param>
    /// <param name="allowHeaders">Allowed request headers (comma separated). Default is "*".</param>
    /// <param name="allowCredentials">Whether to allow credentials. Default is false.</param>
    /// <exception cref="ArgumentException">Thrown when allowOrigins, allowMethods, or allowHeaders is null or empty.</exception>
    public AllowCrossOriginResourceSharingAttribute(
        string allowOrigins,
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

        _allowOrigins = allowOrigins;
        _allowMethods = allowMethods;
        _allowHeaders = allowHeaders;
        _allowCredentials = allowCredentials;
        _overrideGlobals = true;
    }

    /// <inheritdoc />
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Response.HasStarted)
        {
            base.OnActionExecuted(context);
            return;
        }

        string allowOrigin;
        string allowMethods;
        string allowHeaders;
        bool allowCredentials;

        CorsConfig config;
        if (!_overrideGlobals && CorsRegistry.GetConfig() is { } globalConfig)
        {
            config = globalConfig;
        }
        else if (_overrideGlobals && !string.IsNullOrEmpty(_allowOrigins) && !string.IsNullOrEmpty(_allowMethods) && !string.IsNullOrEmpty(_allowHeaders))
        {
            config = new CorsConfig(_allowOrigins, _allowMethods, _allowHeaders, _allowCredentials);
        }
        else
        {
            throw new InvalidOperationException(
                $"No {CorsRegistry.CorsConfigKey} configuration found. Please configure either globally via CorsRegistry.Register(), or locally via the Attribute constructor.");
        }

        allowMethods = config.AllowMethods;
        allowHeaders = config.AllowHeaders;
        allowCredentials = config.AllowCredentials;

        var requestOrigin = context.HttpContext.Request.Headers["Origin"].FirstOrDefault();
        allowOrigin = config.GetEffectiveOrigin(requestOrigin);

        var headers = context.HttpContext.Response.Headers;
        headers[AllowOriginHeader] = allowOrigin;
        headers[AllowMethodsHeader] = allowMethods;
        headers[AllowHeadersHeader] = allowHeaders;
        headers[AllowCredentialsHeader] = allowCredentials.ToString().ToLowerInvariant();

        base.OnActionExecuted(context);
    }
}
