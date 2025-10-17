using FluentAssertions;

namespace VisionaryCoder.Framework.Tests;

/// <summary>
/// Unit tests for FrameworkConstants to ensure all constants have correct values.
/// </summary>
[TestClass]
public class FrameworkConstantsTests
{
    #region Main Constants Tests

    [TestMethod]
    public void Version_ShouldHaveCorrectValue()
    {
        // Assert
        FrameworkConstants.Version.Should().Be("1.0.0");
    }

    [TestMethod]
    public void ConfigurationSection_ShouldHaveCorrectValue()
    {
        // Assert
        FrameworkConstants.ConfigurationSection.Should().Be("VisionaryCoderFramework");
    }

    #endregion

    #region Timeouts Constants Tests

    [TestMethod]
    public void TimeoutsDefaults_ShouldHaveCorrectValues()
    {
        // Assert
        FrameworkConstants.Timeouts.DefaultHttpTimeoutSeconds.Should().Be(30);
        FrameworkConstants.Timeouts.DefaultDatabaseTimeoutSeconds.Should().Be(30);
        FrameworkConstants.Timeouts.DefaultCacheExpirationMinutes.Should().Be(15);
    }

    [TestMethod]
    public void TimeoutsConstants_ShouldBePositiveValues()
    {
        // Assert
        FrameworkConstants.Timeouts.DefaultHttpTimeoutSeconds.Should().BePositive();
        FrameworkConstants.Timeouts.DefaultDatabaseTimeoutSeconds.Should().BePositive();
        FrameworkConstants.Timeouts.DefaultCacheExpirationMinutes.Should().BePositive();
    }

    #endregion

    #region Headers Constants Tests

    [TestMethod]
    public void HeaderNames_ShouldHaveCorrectValues()
    {
        // Assert
        FrameworkConstants.Headers.CorrelationId.Should().Be("X-Correlation-ID");
        FrameworkConstants.Headers.RequestId.Should().Be("X-Request-ID");
        FrameworkConstants.Headers.UserContext.Should().Be("X-User-Context");
        FrameworkConstants.Headers.ApiVersion.Should().Be("Api-Version");
    }

    [TestMethod]
    public void HeaderNames_ShouldNotBeNullOrEmpty()
    {
        // Assert
        FrameworkConstants.Headers.CorrelationId.Should().NotBeNullOrWhiteSpace();
        FrameworkConstants.Headers.RequestId.Should().NotBeNullOrWhiteSpace();
        FrameworkConstants.Headers.UserContext.Should().NotBeNullOrWhiteSpace();
        FrameworkConstants.Headers.ApiVersion.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void HeaderNames_ShouldFollowHTTPHeaderConventions()
    {
        // Assert - Headers should contain hyphens and follow standard HTTP header naming
        FrameworkConstants.Headers.CorrelationId.Should().Contain("-");
        FrameworkConstants.Headers.RequestId.Should().Contain("-");
        FrameworkConstants.Headers.UserContext.Should().Contain("-");
        FrameworkConstants.Headers.ApiVersion.Should().Contain("-");
    }

    #endregion

    #region Logging Constants Tests

    [TestMethod]
    public void LoggingTemplate_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        // Assert
        FrameworkConstants.Logging.DefaultTemplate.Should().Be(expectedTemplate);
    }

    [TestMethod]
    public void LoggingPropertyNames_ShouldHaveCorrectValues()
    {
        // Assert
        FrameworkConstants.Logging.CorrelationIdProperty.Should().Be("CorrelationId");
        FrameworkConstants.Logging.RequestIdProperty.Should().Be("RequestId");
        FrameworkConstants.Logging.UserIdProperty.Should().Be("UserId");
    }

    [TestMethod]
    public void LoggingPropertyNames_ShouldNotBeNullOrEmpty()
    {
        // Assert
        FrameworkConstants.Logging.CorrelationIdProperty.Should().NotBeNullOrWhiteSpace();
        FrameworkConstants.Logging.RequestIdProperty.Should().NotBeNullOrWhiteSpace();
        FrameworkConstants.Logging.UserIdProperty.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void LoggingTemplate_ShouldContainRequiredPlaceholders()
    {
        // Arrange
        var template = FrameworkConstants.Logging.DefaultTemplate;

        // Assert - Template should contain standard structured logging placeholders
        template.Should().Contain("{Timestamp");
        template.Should().Contain("{Level");
        template.Should().Contain("{SourceContext}");
        template.Should().Contain("{Message");
        template.Should().Contain("{NewLine}");
        template.Should().Contain("{Exception}");
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void AllConstants_ShouldBeAccessible()
    {
        // This test ensures all nested classes and their constants are accessible
        // If compilation succeeds, the test passes

        // Main constants
        var version = FrameworkConstants.Version;
        var configSection = FrameworkConstants.ConfigurationSection;

        // Timeout constants
        var httpTimeout = FrameworkConstants.Timeouts.DefaultHttpTimeoutSeconds;
        var dbTimeout = FrameworkConstants.Timeouts.DefaultDatabaseTimeoutSeconds;
        var cacheTimeout = FrameworkConstants.Timeouts.DefaultCacheExpirationMinutes;

        // Header constants
        var correlationHeader = FrameworkConstants.Headers.CorrelationId;
        var requestHeader = FrameworkConstants.Headers.RequestId;
        var userHeader = FrameworkConstants.Headers.UserContext;
        var versionHeader = FrameworkConstants.Headers.ApiVersion;

        // Logging constants
        var template = FrameworkConstants.Logging.DefaultTemplate;
        var correlationProp = FrameworkConstants.Logging.CorrelationIdProperty;
        var requestProp = FrameworkConstants.Logging.RequestIdProperty;
        var userProp = FrameworkConstants.Logging.UserIdProperty;

        // Assert that all values are not null (compilation test)
        version.Should().NotBeNull();
        configSection.Should().NotBeNull();
        template.Should().NotBeNull();
        correlationHeader.Should().NotBeNull();
    }

    #endregion
}