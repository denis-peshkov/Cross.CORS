namespace Cross.CORS.Tests.Cors;

public class CorsConfigTests
{
    [Test]
    public void Constructor_WithValidParams_SetsProperties()
    {
        var config = new CorsConfig(
            allowOrigins: "https://example.com",
            allowMethods: "GET, POST",
            allowHeaders: "Content-Type",
            allowCredentials: true);

        config.AllowOrigins.Should().Be("https://example.com");
        config.AllowMethods.Should().Be("GET, POST");
        config.AllowHeaders.Should().Be("Content-Type");
        config.AllowCredentials.Should().BeTrue();
    }

    [Test]
    public void Constructor_WithDefaults_UsesWildcards()
    {
        var config = new CorsConfig();

        config.AllowOrigins.Should().Be("*");
        config.AllowMethods.Should().Be("*");
        config.AllowHeaders.Should().Be("*");
        config.AllowCredentials.Should().BeFalse();
    }

    [Test]
    public void Constructor_WithNullAllowOrigin_Throws()
    {
        var act = () => new CorsConfig(allowOrigins: null!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("allowOrigins");
    }

    [Test]
    public void Constructor_WithEmptyAllowOrigin_Throws()
    {
        var act = () => new CorsConfig(allowOrigins: string.Empty);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("allowOrigins");
    }

    [Test]
    public void Constructor_WithNullAllowMethods_Throws()
    {
        var act = () => new CorsConfig(allowOrigins: "*", allowMethods: null!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("allowMethods");
    }

    [Test]
    public void Constructor_WithNullAllowHeaders_Throws()
    {
        var act = () => new CorsConfig(allowOrigins: "*", allowHeaders: null!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("allowHeaders");
    }

    [Test]
    public void GetAllowedOrigins_WithCommaSeparated_ReturnsList()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com, https://b.com");

        var origins = config.GetAllowedOrigins();

        origins.Should().BeEquivalentTo("https://a.com", "https://b.com");
    }

    [Test]
    public void GetAllowedOrigins_WithSpaceSeparated_ReturnsList()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com https://b.com");

        var origins = config.GetAllowedOrigins();

        origins.Should().BeEquivalentTo("https://a.com", "https://b.com");
    }

    [Test]
    public void GetAllowedOrigins_WithMixedSeparators_ReturnsList()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com, https://b.com https://c.com");

        var origins = config.GetAllowedOrigins();

        origins.Should().BeEquivalentTo("https://a.com", "https://b.com", "https://c.com");
    }

    [Test]
    public void GetEffectiveOrigin_WhenRequestOriginInList_ReturnsRequestOrigin()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com, https://b.com");

        var result = config.GetEffectiveOrigin("https://b.com");

        result.Should().Be("https://b.com");
    }

    [Test]
    public void GetEffectiveOrigin_WhenRequestOriginNotInList_ReturnsFirst()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com, https://b.com");

        var result = config.GetEffectiveOrigin("https://unknown.com");

        result.Should().Be("https://a.com");
    }

    [Test]
    public void GetEffectiveOrigin_WhenRequestOriginHasSpaces_TrimsAndMatches()
    {
        var config = new CorsConfig(allowOrigins: "https://a.com, https://b.com");

        var result = config.GetEffectiveOrigin("  https://b.com  ");

        result.Should().Be("https://b.com");
    }

    [Test]
    public void GetEffectiveOrigin_WhenWildcard_ReturnsWildcard()
    {
        var config = new CorsConfig(allowOrigins: "*");

        var result = config.GetEffectiveOrigin("https://any.com");

        result.Should().Be("*");
    }
}
