namespace Cross.CORS.Tests.Cors;

public class CorsOptionsTests
{
    [Test]
    public void ToCorsConfig_WithDefaults_ReturnsCorsConfig()
    {
        var options = new CorsOptions();

        var config = options.ToCorsConfig();

        config.AllowOrigins.Should().Be("*");
        config.AllowMethods.Should().Be("*");
        config.AllowHeaders.Should().Be("*");
        config.AllowCredentials.Should().BeFalse();
    }

    [Test]
    public void ToCorsConfig_WithValues_ReturnsCorsConfig()
    {
        var options = new CorsOptions
        {
            AllowOrigins = "https://a.com, https://b.com",
            AllowMethods = "GET, POST",
            AllowHeaders = "Content-Type",
            AllowCredentials = true
        };

        var config = options.ToCorsConfig();

        config.AllowOrigins.Should().Be("https://a.com, https://b.com");
        config.AllowMethods.Should().Be("GET, POST");
        config.AllowHeaders.Should().Be("Content-Type");
        config.AllowCredentials.Should().BeTrue();
    }

    [Test]
    public void ToCorsConfig_WithNullAllowOrigins_UsesWildcard()
    {
        var options = new CorsOptions { AllowOrigins = null! };

        var config = options.ToCorsConfig();

        config.AllowOrigins.Should().Be("*");
    }

    [Test]
    public void ToCorsConfig_WithEmptyAllowOrigins_UsesWildcard()
    {
        var options = new CorsOptions { AllowOrigins = string.Empty };

        var config = options.ToCorsConfig();

        config.AllowOrigins.Should().Be("*");
    }
}
