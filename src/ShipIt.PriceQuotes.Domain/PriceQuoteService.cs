using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuotes.Domain.ShipItExceptions;
using ShipIt.PriceQuotes.Storage;
using ShipIt.Shared;

namespace ShipIt.PriceQuotes.Domain;


public interface IPriceQuoteService
{
    Task<PriceQuoteResponseContract> CreatePrice(PriceQuoteRequestContract priceToCalc);
    Task<PriceQuoteResponseContract> GetById(string id);
}

public class PriceQuoteService(IPriceQuoteRepository repository) : IPriceQuoteService
{
    public async Task<PriceQuoteResponseContract> CreatePrice(PriceQuoteRequestContract priceToCalc)
    {
        // Check NL business rule
        if(priceToCalc.FromCountry == CountryEnum.NL && priceToCalc.WeightKg > 10)
            throw new FromCountryWeightException($"Package weight may not exceed 10kg for fromCountry NL. Value provided was {priceToCalc.WeightKg}");

        // Calculate Price
        var calculatedPrice = Calc(priceToCalc);
        var calculatedValidUntil = DateTime.UtcNow.AddHours(48);

        // Save price via storage
        var dataModel = new PriceQuoteDataModel
        {
          createdOn = DateTime.UtcNow,
          fromCountry = priceToCalc.FromCountry!.Value,
          toCountry = priceToCalc.ToCountry!.Value,
          heightCm = priceToCalc.HeightCm,
          lengthCm = priceToCalc.LengthCm,
          widthCm = priceToCalc.WidthCm,          
          weightKg = priceToCalc.WeightKg,
          validUntil = calculatedValidUntil,
          price = calculatedPrice
        };

        var created = await repository.CreateAsync(dataModel);

        // Return created object
        return new PriceQuoteResponseContract
        {
            Id = created.id,
            Price = created.price,
            ValidUntil = created.validUntil
        };
    }

    public async Task<PriceQuoteResponseContract> GetById(string id)
    {
        var dbQuote = await repository.GetAsync(id);

        return new PriceQuoteResponseContract
        {
            Id = dbQuote.id,
            Price = dbQuote.price,
            ValidUntil = dbQuote.validUntil
        };    
    }

    private static decimal Calc(PriceQuoteRequestContract priceToCalc)
    {
        var volumeCm3 = priceToCalc.HeightCm * priceToCalc.LengthCm * priceToCalc.WidthCm;        

        var basePrice = 2.5 + volumeCm3 * 0.0001 + priceToCalc.WeightKg * 1.15;
        var borderSurcharge = priceToCalc.FromCountry == priceToCalc.ToCountry ? 0.0 : 3.0;

        var totalPrice = (decimal)(basePrice + borderSurcharge);
        var totalPriceRounded = Math.Round(totalPrice, 1);

        return totalPriceRounded;
    }
}
