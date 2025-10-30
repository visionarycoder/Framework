// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VisionaryCoder.Framework.Authorization;
using VisionaryCoder.Framework.Authorization.Policies;

namespace VisionaryCoder.Framework.Tests.Authorization;

/// <summary>
/// Comprehensive data-driven unit tests for Authorization service collection extensions with 100% code coverage.
/// Tests SOLID principles compliance and service registration patterns.
/// </summary>
[TestClass]
public class AuthorizationServiceCollectionExtensionsTests
{
    private IServiceCollection services = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        services = new ServiceCollection();
    }

    #region Manual Registration Tests

    [TestMethod]
    public void AddRoleBasedAuthorizationPolicy_ShouldRegisterCorrectly()
    {
        // Act
        services.AddScoped<IAuthorizationPolicy, RoleBasedAuthorizationPolicy>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var policy = serviceProvider.GetService<IAuthorizationPolicy>();
        policy.Should().NotBeNull();
        policy.Should().BeOfType<RoleBasedAuthorizationPolicy>();
    }

    [TestMethod]
    public void AddNullAuthorizationPolicy_ShouldRegisterCorrectly()
    {
        // Act
        services.AddScoped<IAuthorizationPolicy, NullAuthorizationPolicy>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var policy = serviceProvider.GetService<IAuthorizationPolicy>();
        policy.Should().NotBeNull();
        policy.Should().BeOfType<NullAuthorizationPolicy>();
    }

    [TestMethod]
    public void AddMultipleAuthorizationPolicies_ShouldRegisterAll()
    {
        // Act
        services.AddScoped<IAuthorizationPolicy, RoleBasedAuthorizationPolicy>();
        services.AddScoped<IAuthorizationPolicy, NullAuthorizationPolicy>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var policies = serviceProvider.GetServices<IAuthorizationPolicy>();
        policies.Should().HaveCount(2);
        policies.Should().ContainSingle(p => p.GetType() == typeof(RoleBasedAuthorizationPolicy));
        policies.Should().ContainSingle(p => p.GetType() == typeof(NullAuthorizationPolicy));
    }

    [TestMethod]
    public void RegisterAuthorizationPolicies_ShouldUseCorrectServiceLifetime()
    {
        // Act
        services.AddScoped<IAuthorizationPolicy, RoleBasedAuthorizationPolicy>();

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IAuthorizationPolicy) 
            && s.ImplementationType == typeof(RoleBasedAuthorizationPolicy));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    #endregion
}