using CurrencyConverterTask.Lib;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverter>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (errorFeature != null)
        {

            var exception = errorFeature.Error;

            var statusCode = exception switch
            {
                InvalidOperationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var errorMessage = new
            {
                Type = exception.GetType().Name,
                errorFeature.Error.Message
            };

            var json = JsonSerializer.Serialize(errorMessage);
            await context.Response.WriteAsync(json);
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/exchangeRates", ([FromServices] ICurrencyConverter currencyConverter) =>
{
    var result = currencyConverter.GetExchangeRates();
    return Results.Ok(result);

})
.WithName("GetExchangeRates")
.WithOpenApi();

app.MapPost("/config", ([FromServices] ICurrencyConverter currencyConverter,
    [FromBody] IEnumerable<Tuple<string, string, double>> conversionRates) =>
{
    currencyConverter.UpdateConfiguration(conversionRates);
    return Results.Ok();

})
.WithName("UpdateConfiguration")
.WithOpenApi();

app.MapPost("/convert/{fromCurrency}/{toCurrency}/{amount}", ([FromServices] ICurrencyConverter currencyConverter,
    [FromRoute] string fromCurrency, [FromRoute] string toCurrency, [FromRoute] double amount) =>
{
    var result = currencyConverter.Convert(fromCurrency, toCurrency, amount);
    return Results.Ok(new { ConvertedAmount = result });

})
.WithName("Convert")
.WithOpenApi();

app.MapDelete("/clear", ([FromServices] ICurrencyConverter currencyConverter) =>
{
    currencyConverter.ClearConfiguration();
    return Results.Ok();
})
.WithName("ClearConfiguration")
.WithOpenApi();

app.Run();

