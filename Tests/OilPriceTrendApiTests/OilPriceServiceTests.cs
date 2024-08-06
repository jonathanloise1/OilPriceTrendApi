using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OilPriceTrendApi.Models;
using OilPriceTrendApi.Services;
using System.Text.Json;

/// <summary>
/// Unit tests for the OilPriceService class.
/// </summary>
public class OilPriceServiceTests
{
    // Mock for the ILogger
    private readonly Mock<ILogger<OilPriceService>> _loggerMock;

    // Mock for the IHttpClientFactory
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    // Mock for the IConfiguration
    private readonly Mock<IConfiguration> _configurationMock;

    // Instance of the OilPriceService to be tested
    private readonly OilPriceService _oilPriceService;

    // Mock for the HttpMessageHandler to simulate HTTP responses
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    /// <summary>
    /// Initializes a new instance of the OilPriceServiceTests class.
    /// Sets up the mocks and initializes the service to be tested.
    /// </summary>
    public OilPriceServiceTests()
    {
        // Initialize the mocks
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<OilPriceService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        // Create an HttpClient using the mocked HttpMessageHandler
        var client = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://example.com")
        };

        // Setup the mock to return the HttpClient
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

        // Setup the configuration mock to return the HttpClient name
        _configurationMock.SetupGet(x => x["OilPriceTrendsConfig:HttpClientName"]).Returns("OilPriceClient");

        // Initialize the OilPriceService with the mocks
        _oilPriceService = new OilPriceService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);
    }

    /// <summary>
    /// Tests the GetOilPriceDataAsync method to ensure it validates dates correctly.
    /// </summary>
    [Fact]
    public async Task GetOilPriceDataAsync_ValidatesDates()
    {
        var startDate = new DateTime(2020, 1, 1);
        var endDate = new DateTime(2020, 1, 5);

        // Setup the mock HttpMessageHandler to return a successful response with data
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new List<OilPriceTrendResponse>
                {
                    new() { Date = "2020-01-01", Price = 50 },
                    new() { Date = "2020-01-02", Price = 55 }
                }))
            });

        // Call the method to test
        var result = await _oilPriceService.GetOilPriceDataAsync(startDate, endDate);

        // Assert that the result is not empty and contains the expected number of elements
        Assert.NotEmpty(result);
        Assert.Equal(2, ((List<OilPriceTrendResponse>)result).Count);
    }

    /// <summary>
    /// Tests the GetOilPriceDataAsync method to ensure it throws an exception for invalid dates.
    /// </summary>
    [Fact]
    public async Task GetOilPriceDataAsync_InvalidDates_ThrowsArgumentException()
    {
        var startDate = new DateTime(2020, 1, 5);
        var endDate = new DateTime(2020, 1, 1);

        // Call the method with invalid dates and assert that it throws an ArgumentException
        await Assert.ThrowsAsync<ArgumentException>(() => _oilPriceService.GetOilPriceDataAsync(startDate, endDate));
    }
}