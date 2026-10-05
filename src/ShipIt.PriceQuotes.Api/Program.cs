using System.Text.Json.Serialization;
using ShipIt.PriceQuotes.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.Converters.Add( 
        new JsonStringEnumConverter());
});
builder.Services.AddScoped<IPriceQuoteService, PriceQuoteService>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment()) 
    app.UseExceptionHandler("/error-development");
else
    app.UseExceptionHandler("/error");

app.Run();
