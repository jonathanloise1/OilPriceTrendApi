using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OilPriceTrendApi.Extensions;
using OilPriceTrendApi.Models;
using OilPriceTrendApi.Services;
using System.Reflection;

namespace OilPriceTrendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OilPriceController(
        ILogger<OilPriceController> logger,
        IValidator<GetOilPriceTrendParams> validator,
        IOilPriceService oilPriceService) : ControllerBase
    {
        private readonly ILogger<OilPriceController> _logger = logger;
        private readonly IValidator<GetOilPriceTrendParams> _validator = validator;
        private readonly IOilPriceService _oilPriceService = oilPriceService;

        /// <summary>
        /// Handles the GetOilPriceTrend JSON-RPC method.
        /// </summary>
        /// <param name="request">The JSON-RPC request payload.</param>
        /// <returns>The JSON-RPC response payload with the oil price trend.</returns>
        [HttpPost]
        public async Task<IActionResult> GetOilPriceTrend([FromBody] JsonRpcRequest<GetOilPriceTrendParams> request)
        {
            string methodName = MethodBase.GetCurrentMethod()!.GetCurrentAsyncMethodName();
            _logger.LogInformation("Received request for [{MethodName}].", methodName);

            if (request.Method != methodName)
            {
                _logger.LogWarning("Method not found: {MethodName}.", request.Method);
                return BadRequest(new JsonRpcResponse<object> { Id = request.Id, Error = "Method not found" });
            }

            if (request.Params == null)
            {
                _logger.LogWarning("Params cannot be null.");
                return BadRequest(new JsonRpcResponse<object> { Id = request.Id, Error = "Params cannot be null" });
            }

            var validationResult = await _validator.ValidateAsync(request.Params).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {ValidationErrors}.", validationResult.Errors);
                return BadRequest(new JsonRpcResponse<object> { Id = request.Id, Error = validationResult.Errors });
            }

            var data = await _oilPriceService.GetOilPriceDataAsync(
                request.Params.StartDateISO8601!.Value,
                request.Params.EndDateISO8601!.Value).ConfigureAwait(false);

            _logger.LogInformation("Successfully processed GetOilPriceTrend request.");

            return Ok(new JsonRpcResponse<object> { Id = request.Id, Result = new { prices = data } });
        }
    }
}