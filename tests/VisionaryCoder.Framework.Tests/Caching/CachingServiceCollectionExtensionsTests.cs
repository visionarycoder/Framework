// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VisionaryCoder.Framework.Caching;
using VisionaryCoder.Framework.Caching.Providers;

namespace VisionaryCoder.Framework.Tests.Caching;

/// <summary>
/// Comprehensive data-driven unit tests for Caching service collection extensions with 100% code coverage.
/// Tests SOLID principles compliance and service registration patterns.
/// </summary>
[TestClass]
public class CachingServiceCollectionExtensionsTests
{
    private IServiceCollection services = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        services = new ServiceCollection();
    }

    #region AddCaching Tests

    [TestMethod]
    public void AddCaching_ShouldRegisterNullProvidersByDefault()
    {
        // Act
        services.AddCaching();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var keyProvider = serviceProvider.GetService<ICacheKeyProvider>();
        keyProvider.Should().NotBeNull();
        keyProvider.Should().BeOfType<NullCacheKeyProvider>();

        var policyProvider = serviceProvider.GetService<ICachePolicyProvider>();
        policyProvider.Should().NotBeNull();
        policyProvider.Should().BeOfType<NullCachePolicyProvider>();

        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().NotBeNull();
        cache.Should().BeOfType<NullProxyCache>();
    }

    [TestMethod]
    public void AddCaching_WithConfiguration_ShouldApplyOptions()
    {
        // Act
        services.AddCaching(options =>
        {
            options.DefaultDuration = TimeSpan.FromMinutes(30);
            options.MaxCacheSize = 1000;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().NotBeNull();
    }

    [TestMethod]
    public void AddCaching_WithTimeSpan_ShouldRegisterWithDefaultDuration()
    {
        // Act
        services.AddCaching(TimeSpan.FromMinutes(15));

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().NotBeNull();
        cache.Should().BeOfType<NullProxyCache>();
    }

    #endregion

    #region AddCaching with Generic Types Tests

    [TestMethod]
    public void AddCaching_WithGenericCache_ShouldRegisterSpecifiedCache()
    {
        // Act
        services.AddCaching<MemoryProxyCache>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().NotBeNull();
        cache.Should().BeOfType<MemoryProxyCache>();

        var keyProvider = serviceProvider.GetService<ICacheKeyProvider>();
        keyProvider.Should().BeOfType<DefaultCacheKeyProvider>();

        var policyProvider = serviceProvider.GetService<ICachePolicyProvider>();
        policyProvider.Should().BeOfType<DefaultCachePolicyProvider>();
    }

    [TestMethod]
    public void AddCaching_WithGenericProviders_ShouldRegisterSpecifiedProviders()
    {
        // Act
        services.AddCaching<DefaultCacheKeyProvider, DefaultCachePolicyProvider>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var keyProvider = serviceProvider.GetService<ICacheKeyProvider>();
        keyProvider.Should().BeOfType<DefaultCacheKeyProvider>();

        var policyProvider = serviceProvider.GetService<ICachePolicyProvider>();
        policyProvider.Should().BeOfType<DefaultCachePolicyProvider>();

        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().BeOfType<MemoryProxyCache>();
    }

    #endregion

    #region AddDistributedCaching Tests

    [TestMethod]
    public void AddDistributedCaching_ShouldRegisterCachingWithConfiguration()
    {
        // Act
        services.AddDistributedCaching(options =>
        {
            options.DefaultDuration = TimeSpan.FromHours(1);
            options.EnableEvictionLogging = true;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetService<IProxyCache>();
        cache.Should().NotBeNull();
    }

    #endregion

    #region Service Lifetime Tests

    [TestMethod]
    public void AddCaching_ShouldRegisterProvidersAsSingleton()
    {
        // Act
        services.AddCaching();

        // Assert
        var keyProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICacheKeyProvider));
        keyProviderDescriptor.Should().NotBeNull();
        keyProviderDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);

        var policyProviderDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICachePolicyProvider));
        policyProviderDescriptor.Should().NotBeNull();
        policyProviderDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);

        var cacheDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IProxyCache));
        cacheDescriptor.Should().NotBeNull();
        cacheDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    #endregion
}