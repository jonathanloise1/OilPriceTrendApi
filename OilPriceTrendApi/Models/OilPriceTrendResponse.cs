namespace OilPriceTrendApi.Models
{
    /// <summary>
    /// Represents the response containing the oil price trend for a specific date.
    /// </summary>
    public class OilPriceTrendResponse
    {
        /// <summary>
        /// Gets or sets the date in ISO 8601 format.
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// Gets or sets the oil price on the given date.
        /// </summary>
        public double? Price { get; set; }
    }
}