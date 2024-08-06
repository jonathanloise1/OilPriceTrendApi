using FluentValidation;

namespace OilPriceTrendApi.Models.Validators
{
    /// <summary>
    /// Validator for GetOilPriceTrendParams.
    /// </summary>
    public class GetOilPriceTrendParamsValidator : AbstractValidator<GetOilPriceTrendParams>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetOilPriceTrendParamsValidator"/> class.
        /// </summary>
        public GetOilPriceTrendParamsValidator()
        {
            RuleFor(x => x.StartDateISO8601)
                .NotNull().WithMessage("StartDateISO8601 is required.")
                .Must(BeAValidDate).WithMessage("StartDateISO8601 must be a valid date with only year, month, and day.");

            RuleFor(x => x.EndDateISO8601)
                .NotNull().WithMessage("EndDateISO8601 is required.")
                .Must(BeAValidDate).WithMessage("EndDateISO8601 must be a valid date with only year, month, and day.");
        }

        /// <summary>
        /// Validates if the provided date has only year, month, and day components.
        /// </summary>
        /// <param name="date">The date to validate.</param>
        /// <returns>True if the date is valid; otherwise, false.</returns>
        private bool BeAValidDate(DateTime? date)
        {
            if (!date.HasValue) return false;
            return date.Value.Hour == 0 && date.Value.Minute == 0 && date.Value.Second == 0 && date.Value.Millisecond == 0;
        }
    }
}