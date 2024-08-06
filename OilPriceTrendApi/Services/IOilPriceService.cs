using OilPriceTrendApi.Models;

namespace OilPriceTrendApi.Services
{
    public interface IOilPriceService
    {
        Task<IEnumerable<OilPriceTrendResponse>> GetOilPriceDataAsync(DateTime startDate, DateTime endDate);
    }
}