using OilPriceTrendApi.Extensions;
using OilPriceTrendApi.Models;
using System.Reflection;
using System.Text.Json;

namespace OilPriceTrendApi.Services
{
    public class OilPriceService(
        ILogger<OilPriceService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : IOilPriceService
    {
        private readonly ILogger<OilPriceService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Retrieves oil price data within a date range.
        /// </summary>
        /// <param name="startDate">The start date for the data retrieval.</param>
        /// <param name="endDate">The end date for the data retrieval.</param>
        /// <returns>A collection of oil price data points.</returns>
        public async Task<IEnumerable<OilPriceTrendResponse>> GetOilPriceDataAsync(DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod()!.GetCurrentAsyncMethodName();
            _logger.LogInformation("Executing method [{MethodName}].", methodName);

            ValidateDates(startDate, endDate);

            var clientName = _configuration["OilPriceTrendsConfig:HttpClientName"]!;
            var client = _httpClientFactory.CreateClient(clientName);

            _logger.LogInformation("Sending request to {Url}.", client.BaseAddress);
            var response = await client.GetStringAsync(string.Empty).ConfigureAwait(false);
            _logger.LogInformation("Received response from {Url}.", client.BaseAddress);

            var data = (JsonSerializer.Deserialize<IEnumerable<OilPriceTrendResponse>>(response) ?? []).ToList();
            
            data = data
                .Where(d => DateTime.Parse(d.Date) >= startDate && DateTime.Parse(d.Date) <= endDate)
                .ToList();

            _logger.LogInformation("Exiting method [{MethodName}].", methodName);

            return data;
        }

        /// <summary>
        /// Validates the start and end dates.
        /// </summary>
        /// <param name="startDate">The start date to validate.</param>
        /// <param name="endDate">The end date to validate.</param>
        private void ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("StartDate must be less than or equal to EndDate.");
            }

            if (startDate == default || endDate == default)
            {
                throw new ArgumentException("StartDate and EndDate must be valid dates.");
            }
        }
    }
}
