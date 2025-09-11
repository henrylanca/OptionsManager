using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OptionManager;
using System.Text.Json;

namespace OptionsManager;

public class GetOptionList
{
    private readonly ILogger<GetOptionList> _logger;

    public GetOptionList(ILogger<GetOptionList> logger)
    {
        _logger = logger;
    }

    [Function("GetOptionList")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }

    [Function("CalculateOptionPrice")]
    [OpenApiOperation(operationId: "CalculateOptionPrice", tags: new[] { "Option" }, Summary = "Calculate Option Price", Description = "Calculates the price of an option using the Black-Scholes model.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(OptionPriceRequest), Required = true, Description = "Option price calculation parameters")]
    public async Task<IActionResult> CalculateOptionPrice([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Calculating option price.");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<OptionPriceRequest>(requestBody, options);

            List<OptionPLByPrice> lstOptionPL = new List<OptionPLByPrice>();

            if (data == null)
                return new BadRequestObjectResult("Invalid request payload.");

            OptionsCalculater optionsCalcualtor = new OptionsCalculater(data.TradingDate, data.Interest, data.DividendRatio);

            foreach (OptionCombo optCombo in data.OptionCombos)
            {
                var optionPL = optionsCalcualtor.CalculateOptionSPL(optCombo.StockOptions, data.Volatility, data.StartPrice, data.EndPrice);
                optionPL.Title = optCombo.Title;

                lstOptionPL.Add(optionPL);
            }

            return new OkObjectResult(new { OptionsPL = lstOptionPL });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating option price.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

    }
}

public class OptionPriceRequest
{
    public double Interest { get; set; }
    public double DividendRatio { get; set; }
    public double Volatility { get; set; }

    public decimal StartPrice { get; set; }

    public decimal EndPrice { get; set; }

    public DateTime TradingDate { get; set; }

    public List<OptionCombo> OptionCombos { get; set; }

}

public class OptionCombo
{
    public string Title { get; set; }
    public List<StockOption> StockOptions { get; set; }
}
