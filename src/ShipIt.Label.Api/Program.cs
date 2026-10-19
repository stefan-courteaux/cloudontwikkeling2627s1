using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShipIt.Label.Domain;
using ShipIt.Label.Domain.DTO;
using ShipIt.Label.Persistence;
using ShipIt.PriceQuote.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<LabelRepoOptions>(
    builder.Configuration.GetSection(nameof(LabelRepoOptions))
);

builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<IDomainController, DomainController>();

builder.Services.AddHttpClient();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

app.MapGet("/hello", () => "Hello");

app.MapGet("api/labels/{trackingnumber}",
async Task<Results<Ok<string>, BadRequest, FileContentHttpResult>>(
    [FromRoute] string trackingNumber,
    [FromHeader(Name = "Accept")] string acceptValue,
    IDomainController domain) =>
{
    if(acceptValue == "text/json")
    {
        // Dummy implementatie
        return TypedResults.Ok(trackingNumber);
    }
    else if (acceptValue == "application/pdf")
    {
        var pdfStream = await domain.GetLabelAsync(trackingNumber);
        return TypedResults.Bytes(
            pdfStream,
            "application/pdf",
            $"{trackingNumber}.pdf");
    }

    return TypedResults.BadRequest();

}).WithName("GetLabel");

app.MapPost("api/labels", async Task<CreatedAtRoute<string>> (
    IHttpClientFactory clientFactory, 
    [FromBody] LabelRequestContract contract, 
    IDomainController domain) => {

    // get price
    var httpClient = clientFactory.CreateClient();
    var baseUrl = "https://hogent-stefanc-gent-quote.azurewebsites.net";
    var url = $"{baseUrl}/api/pricequote/{contract.QuoteId}";//TODO url should be plural for resources

    var options = new JsonSerializerOptions();
    options.Converters.Add(new JsonStringEnumConverter());
    options.PropertyNameCaseInsensitive = true;
    var response = await httpClient
        .GetFromJsonAsync<PriceQuoteResponseContract>(url, options); 

    // create label
    var creation = new LabelCreationDTO{
        RecipientAddressLine1 = contract.RecipientAddressLine1,
        RecipientAddressLine2 = contract.RecipientAddressLine2,
        RecipientCountry = contract.RecipientCountry,
        RecipientName = contract.RecipientName,
        ShipmentPrice = (double) response.Price 
    };
    var trackingNumber = await domain.CreateLabelAsync(creation); 

    // return trackingNumber / resource identifier
    return TypedResults.CreatedAtRoute( 
        trackingNumber, "GetLabel", new { trackingnumber = trackingNumber});
});




app.Run();

public record LabelRequestContract
{
    public string QuoteId { get; set; }
    public string RecipientName { get; set; }
    public string RecipientAddressLine1 { get; set; }
    public string RecipientAddressLine2 { get; set; }
    public string RecipientCountry { get; set; }
}