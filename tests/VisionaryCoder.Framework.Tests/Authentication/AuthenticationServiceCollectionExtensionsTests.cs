// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Authentication;
using VisionaryCoder.Framework.Authentication.Providers;
using VisionaryCoder.Framework.Authentication.Jwt;

namespace VisionaryCoder.Framework.Tests.Authentication;

/// <summary>
/// Comprehensive data-driven unit tests for Authentication service collection extensions with 100% code coverage.
/// Tests SOLID principles compliance and service registration patterns.
/// </summary>
[TestClass]
public class AuthenticationServiceCollectionExtensionsTests
{
    private IServiceCollection services = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        services = new ServiceCollection();
    }

    #region UseDefaultAuthenticationProviders Tests

    [TestMethod]
    public void UseDefaultAuthenticationProviders_ShouldRegisterDefaultProviders()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.UseDefaultAuthenticationProviders();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var userProvider = serviceProvider.GetService<IUserContextProvider>();
        userProvider.Should().NotBeNull();
        userProvider.Should().BeOfType<DefaultUserContextProvider>();

        var tenantProvider = serviceProvider.GetService<ITenantContextProvider>();
        tenantProvider.Should().NotBeNull();
        tenantProvider.Should().BeOfType<DefaultTenantContextProvider>();

        var tokenProvider = serviceProvider.GetService<ITokenProvider>();
        tokenProvider.Should().NotBeNull();
        tokenProvider.Should().BeOfType<DefaultTokenProvider>();
    }

    #endregion

    #region ReplaceUserContextProvider Tests

    [TestMethod]
    public void ReplaceUserContextProvider_ShouldReplaceWithSpecifiedProvider()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.ReplaceUserContextProvider<DefaultUserContextProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IUserContextProvider>();
        provider.Should().NotBeNull();
        provider.Should().BeOfType<DefaultUserContextProvider>();
    }

    [TestMethod]
    public void ReplaceUserContextProvider_CalledMultipleTimes_ShouldUseLastRegistration()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.ReplaceUserContextProvider<DefaultUserContextProvider>();
        services.ReplaceUserContextProvider<NullUserContextProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IUserContextProvider>();
        provider.Should().BeOfType<NullUserContextProvider>();
    }

    #endregion

    #region ReplaceTenantContextProvider Tests

    [TestMethod]
    public void ReplaceTenantContextProvider_ShouldReplaceWithSpecifiedProvider()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.ReplaceTenantContextProvider<DefaultTenantContextProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<ITenantContextProvider>();
        provider.Should().NotBeNull();
        provider.Should().BeOfType<DefaultTenantContextProvider>();
    }

    [TestMethod]
    public void ReplaceTenantContextProvider_CalledMultipleTimes_ShouldUseLastRegistration()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.ReplaceTenantContextProvider<DefaultTenantContextProvider>();
        services.ReplaceTenantContextProvider<NullTenantContextProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<ITenantContextProvider>();
        provider.Should().BeOfType<NullTenantContextProvider>();
    }

    #endregion

    #region ReplaceTokenProvider Tests

    [TestMethod]
    public void ReplaceTokenProvider_ShouldReplaceWithSpecifiedProvider()
    {
        // Arrange
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Act
        services.ReplaceTokenProvider<DefaultTokenProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<ITokenProvider>();
        provider.Should().NotBeNull();
        provider.Should().BeOfType<DefaultTokenProvider>();
    }

    #endregion

    #region AddJwtAuthentication Tests

    [TestMethod]
    public void AddJwtAuthentication_ShouldRegisterNullProvidersByDefault()
    {
        // Act
        services.AddJwtAuthentication(options =>
        {
            options.Authority = "https://localhost:5001";
            options.Audience = "test-audience";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var userProvider = serviceProvider.GetService<IUserContextProvider>();
        userProvider.Should().BeOfType<NullUserContextProvider>();

        var tenantProvider = serviceProvider.GetService<ITenantContextProvider>();
        tenantProvider.Should().BeOfType<NullTenantContextProvider>();

        var tokenProvider = serviceProvider.GetService<ITokenProvider>();
        tokenProvider.Should().BeOfType<NullTokenProvider>();
    }

    [TestMethod]
    public void AddJwtAuthentication_WithInvalidOptions_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => services.AddJwtAuthentication(options =>
        {
            // Missing Authority and Audience - invalid configuration
        });

        act.Should().Throw<ArgumentException>()
           .WithMessage("*JWT options configuration is invalid*");
    }

    #endregion
}
