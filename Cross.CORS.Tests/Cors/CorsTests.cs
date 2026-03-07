namespace Cross.CORS.Tests.Cors;

public class CorsTests
{
    [TearDown]
    public void TearDown()
    {
        // Reset static config between tests via reflection
        var field = typeof(CorsRegistry).GetField("_config", BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);
    }

    [Test]
    public void CorsConfigKey_IsAspNetWebApiCors()
    {
        CorsRegistry.CorsConfigKey.Should().Be("crossCORS");
    }

    [Test]
    public void Register_WithValidConfig_StoresConfig()
    {
        var config = new CorsConfig("https://example.com");
        CorsRegistry.Register(config);

        CorsRegistry.GetConfig().Should().Be(config);
    }

    [Test]
    public void Register_WithNull_Throws()
    {
        var act = () => CorsRegistry.Register(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("cors");
    }
}
