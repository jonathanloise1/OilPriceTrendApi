namespace OilPriceTrendApi.Models
{
    /// <summary>
    /// Represents the parameters for getting the oil price trend.
    /// </summary>
    public class GetOilPriceTrendParams
    {
        /// <summary>
        /// Gets or sets the starting date of the period in ISO 8601 format.
        /// </summary>
        public DateTime? StartDateISO8601 { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the period in ISO 8601 format.
        /// </summary>
        public DateTime? EndDateISO8601 { get; set; }
    }
}