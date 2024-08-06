using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OilPriceTrendApi.Models;
using OilPriceTrendApi.Services;

/// <summary>
/// Unit tests for the CachingOilPriceServiceDecorator class.
/// </summary>
public class OilPriceServiceDecoratorCacheTests
{
    private readonly Mock<ILogger<OilPriceServiceDecoratorCache>> _loggerMock;
    private readonly Mock<IOilPriceService> _innerServiceMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly OilPriceServiceDecoratorCache _cachingService;

    /// <summary>
    /// Initializes a new instance of the CachingOilPriceServiceDecoratorTests class.
    /// </summary>
    public OilPriceServiceDecoratorCacheTests()
    {
        // Mock for the inner service
        _innerServiceMock = new Mock<IOilPriceService>();

        // Mock for the memory cache
        _cacheMock = new Mock<IMemoryCache>();

        // Mock for the configuration
        _configurationMock = new Mock<IConfiguration>();

        // Mock for the logger
        _loggerMock = new Mock<ILogger<OilPriceServiceDecoratorCache>>();

        // Configure the mock configuration to return the cache duration
        var configurationSectionMock = new Mock<IConfigurationSection>();
        configurationSectionMock.Setup(x => x.Value).Returns("86400000"); // 24 hours in milliseconds

        _configurationMock
            .Setup(x => x.GetSection("OilPriceTrendsConfig:CacheDurationInMilliseconds"))
            .Returns(configurationSectionMock.Object);

        // Initialize the caching service decorator
        _cachingService = new OilPriceServiceDecoratorCache(
            _loggerMock.Object,
            _innerServiceMock.Object,
            _cacheMock.Object,
            _configurationMock.Object
        );
    }

    /// <summary>
    /// Tests the GetOilPriceDataAsync method when the cache is missed and data is fetched from the inner service.
    /// </summary>
    [Fact]
    public async Task GetOilPriceDataAsync_CacheMiss_FetchesFromInnerService()
    {
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2020, 1, 5);

        // Mock cache entry creation
        var mockCacheEntry = new Mock<ICacheEntry>();
        _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry.Object);

        // Configure the mock inner service to return data
        _innerServiceMock.Setup(x => x.GetOilPriceDataAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<OilPriceTrendResponse>
            {
                new() { Date = "2020-01-01", Price = 50 },
                new() { Date = "2020-01-02", Price = 55 }
            });

        // Call the caching service decorator
        var result = (await _cachingService.GetOilPriceDataAsync(startDate, endDate)).ToList();

        // Assert that the result is not empty and contains the expected data
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);

        // Verify that the inner service was called once
        _innerServiceMock.Verify(x => x.GetOilPriceDataAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    /// <summary>
    /// Tests the GetOilPriceDataAsync method when data is already present in the cache.
    /// </summary>
    [Fact]
    public async Task GetOilPriceDataAsync_CacheHit_ReturnsCachedData()
    {
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2020, 1, 5);

        var cachedData = new List<OilPriceTrendResponse>
        {
            new() { Date = "2020-01-01", Price = 50 },
            new() { Date = "2020-01-02", Price = 55 }
        };

        // Configure the mock cache to simulate a cache hit
        object cacheEntry = cachedData;
        _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(true);

        // Call the caching service decorator
        var result = (await _cachingService.GetOilPriceDataAsync(startDate, endDate)).ToList();

        // Assert that the result is not empty and contains the expected data
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);

        // Verify that the inner service was never called
        _innerServiceMock.Verify(x => x.GetOilPriceDataAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }
}