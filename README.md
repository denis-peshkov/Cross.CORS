[![License](https://img.shields.io/github/license/denis-peshkov/Cross.CORS)](LICENSE)
[![GitHub Release Date](https://img.shields.io/github/release-date/denis-peshkov/Cross.CORS?label=released)](https://github.com/denis-peshkov/Cross.CORS/releases)
[![NuGetVersion](https://img.shields.io/nuget/v/Cross.CORS.svg)](https://nuget.org/packages/Cross.CORS/)
[![NugetDownloads](https://img.shields.io/nuget/dt/Cross.CORS.svg)](https://nuget.org/packages/Cross.CORS/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Cross.CORS&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Cross.CORS)
[![issues](https://img.shields.io/github/issues/denis-peshkov/Cross.CORS)](https://github.com/denis-peshkov/Cross.CORS/issues)
[![.NET PR](https://github.com/denis-peshkov/Cross.CORS/actions/workflows/dotnet.yml/badge.svg?event=pull_request)](https://github.com/denis-peshkov/Cross.CORS/actions/workflows/dotnet.yml)

![Size](https://img.shields.io/github/repo-size/denis-peshkov/Cross.CORS)
[![GitHub contributors](https://img.shields.io/github/contributors/denis-peshkov/Cross.CORS)](https://github.com/denis-peshkov/Cross.CORS/contributors)
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/denis-peshkov/Cross.CORS/latest?label=new+commits)](https://github.com/denis-peshkov/Cross.CORS/commits/master)
![Activity](https://img.shields.io/github/commit-activity/w/denis-peshkov/Cross.CORS)
![Activity](https://img.shields.io/github/commit-activity/m/denis-peshkov/Cross.CORS)
![Activity](https://img.shields.io/github/commit-activity/y/denis-peshkov/Cross.CORS)

# Cross.CORS

ASP.NET Core library for configuring Cross-Origin Resource Sharing (CORS) via the `[AllowCrossOriginResourceSharing]` attribute and global `CorsConfig` configuration.

**Supported frameworks:** .NET 6, .NET 7, .NET 8, .NET 9, .NET 10

## Install NuGet package

Install the _Cross.CORS_ [NuGet package](https://www.nuget.org/packages/Cross.CORS/) into your .NET project:

```powershell
Install-Package Cross.CORS
```

or

```bash
dotnet add package Cross.CORS
```

## Library classes

| Class | Description |
|-------|-------------|
| `CorsConfig` | CORS configuration (origins, methods, headers, credentials) |
| `CorsOptions` | Options for binding from appsettings.json; use `CorsOptions.ConfigSection` ("Cors") for `GetSection()` |
| `CorsRegistry` | Static class for global configuration registration |
| `AllowCrossOriginResourceSharingAttribute` | Attribute for controllers and actions with support for global and local configuration |

## Registration and configuration

### Option 1: Global configuration

Call `CorsRegistry.Register()` at application startup (e.g. in `Program.cs` or `Startup`):

```csharp
using Cross.CORS;

var builder = WebApplication.CreateBuilder(args);

// Register global CORS configuration
CorsRegistry.Register(new CorsConfig(
    allowOrigins: "https://example.com",
    allowMethods: "GET, POST, PUT, DELETE, OPTIONS",
    allowHeaders: "Content-Type, Authorization, X-Requested-With",
    allowCredentials: true));

var app = builder.Build();
// ...
```

When using global configuration, the `[AllowCrossOriginResourceSharing]` attribute without parameters adds response headers from this configuration.

### Option 2: Configuration from appsettings.json

Configuration section name: `CorsOptions.ConfigSection` (value: `"Cors"`).

```json
{
  "Cors": {
    "AllowOrigins": "https://example.com, https://trusted-app.com",
    "AllowMethods": "GET, POST, PUT, DELETE, OPTIONS",
    "AllowHeaders": "Content-Type, Authorization",
    "AllowCredentials": true
  }
}
```

**With CorsOptions class:**

```csharp
var corsOptions = new CorsOptions();
builder.Configuration.GetSection(CorsOptions.ConfigSection).Bind(corsOptions);
CorsRegistry.Register(corsOptions.ToCorsConfig());
```

**Without CorsOptions (manual):**

```csharp
var corsSection = builder.Configuration.GetSection(CorsOptions.ConfigSection);
CorsRegistry.Register(new CorsConfig(
    allowOrigins: corsSection["AllowOrigins"] ?? "*",
    allowMethods: corsSection["AllowMethods"] ?? "*",
    allowHeaders: corsSection["AllowHeaders"] ?? "*",
    allowCredentials: corsSection.GetValue<bool>("AllowCredentials")));
```

### Option 3: CorsConfig default parameters

```csharp
// All origins, methods, headers; no credentials
CorsRegistry.Register(new CorsConfig(
    allowOrigins: "*",
    allowMethods: "*",
    allowHeaders: "*",
    allowCredentials: false));
```

## Usage

### On controller (global configuration)

```csharp
using Cross.CORS;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[AllowCrossOriginResourceSharing]  // uses CorsRegistry.Register()
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "Product1", "Product2" });
}
```

### On specific action (local configuration)

```csharp
using Cross.CORS;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [AllowCrossOriginResourceSharing(
        allowOrigins: "https://example.com",
        allowMethods: "GET",
        allowHeaders: "Content-Type")]
    public IActionResult Get() => Ok(new[] { "Product1", "Product2" });

    [HttpPost]
    [AllowCrossOriginResourceSharing(
        allowOrigins: "https://trusted-app.com",
        allowMethods: "POST, OPTIONS",
        allowHeaders: "Content-Type, Authorization",
        allowCredentials: true)]
    public IActionResult Create([FromBody] Product product) => Ok(product);
}
```

### Mixed usage

```csharp
using Cross.CORS;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[AllowCrossOriginResourceSharing]  // global for all actions
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();  // global configuration

    [HttpPost]
    [AllowCrossOriginResourceSharing(
        allowOrigins: "https://special-client.com",
        allowMethods: "POST",
        allowHeaders: "*",
        allowCredentials: true)]  // local overrides global
    public IActionResult Create([FromBody] Product product) => Ok(product);
}
```

### Using with UseCors

When using the standard `app.UseCors()`, the `[AllowCrossOriginResourceSharing]` attribute adds CORS headers to the response after the action executes. This is useful for:

- fine-grained CORS configuration on individual controllers/actions;
- different rules for different origins on the same API;
- cases where header control at the action filter level is needed.

```csharp
// Program.cs
app.UseCors("Default");  // or another policy
app.UseAuthorization();
app.MapControllers();
```

## Important notes

1. **Required configuration:** When using the attribute without parameters, you must call `CorsRegistry.Register()` before handling requests. Otherwise, an `InvalidOperationException` will be thrown.

2. **Priority:** Local configuration (via attribute parameters) takes precedence over global configuration.

3. **AllowCredentials:** When `allowCredentials: true`, the `allowOrigins` value must not be `*` — specify a concrete origin.

4. **Order:** The attribute adds headers in `OnActionExecuted`, so they are added to the response after the action executes.

5. **Multiple origins:** The `allowOrigins` parameter supports multiple URLs separated by comma or space (e.g. `"https://a.com, https://b.com"` or `"https://a.com https://b.com"`). When the request includes an `Origin` header, the response echoes it if it matches the allowed list; otherwise the first allowed origin or `*` is used.
