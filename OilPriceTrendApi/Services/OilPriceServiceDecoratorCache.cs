using Microsoft.Extensions.Caching.Memory;
using OilPriceTrendApi.Extensions;
using OilPriceTrendApi.Models;
using System.Reflection;

namespace OilPriceTrendApi.Services
{
    public class OilPriceServiceDecoratorCache(
        ILogger<OilPriceServiceDecoratorCache> logger,
        IOilPriceService inner,
        IMemoryCache cache,
        IConfiguration configuration) : IOilPriceService
    {
        private readonly ILogger<OilPriceServiceDecoratorCache> _logger = logger;
        private readonly IOilPriceService _inner = inner;
        private readonly IMemoryCache _cache = cache;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Retrieves oil price data within a date range, applying caching if configured.
        /// </summary>
        /// <param name="startDate">The start date for the data retrieval.</param>
        /// <param name="endDate">The end date for the data retrieval.</param>
        /// <returns>A collection of oil price data points.</returns>
        public async Task<IEnumerable<OilPriceTrendResponse>> GetOilPriceDataAsync(DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod()!.GetCurrentAsyncMethodName();
            _logger.LogInformation("Executing method [{MethodName}].", methodName);

            var cacheKey = "OilPriceData";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<OilPriceTrendResponse> cachedData))
            {
                _logger.LogInformation("Cache miss. Retrieving data from the inner service.");
                cachedData = await _inner.GetOilPriceDataAsync(DateTime.Today.AddYears(-200), DateTime.Today.AddYears(1)).ConfigureAwait(false);

                var cacheDuration = _configuration.GetValue<int>("OilPriceTrendsConfig:CacheDurationInMilliseconds");

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(cacheDuration));

                _cache.Set(cacheKey, cachedData, cacheEntryOptions);
                _logger.LogInformation("Data cached for {cacheDuration} milliseconds.", cacheDuration);
            }
            else
            {
                _logger.LogInformation("Cache hit. Retrieving data from cache.");
            }

            _logger.LogInformation("Exiting method [{MethodName}].", methodName);

            return cachedData
                .Where(d => DateTime.Parse(d.Date) >= startDate && DateTime.Parse(d.Date) <= endDate)
                .ToList();
        }
    }
}
