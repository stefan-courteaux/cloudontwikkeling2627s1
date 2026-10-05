using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuotes.Domain.ShipItExceptions;

namespace ShipIt.PriceQuotes.Domain;


public interface IPriceQuoteService
{
    PriceQuoteResponseContract CreatePrice(PriceQuoteRequestContract priceToCalc);
}

public class PriceQuoteService : IPriceQuoteService
{
    public PriceQuoteResponseContract CreatePrice(PriceQuoteRequestContract priceToCalc)
    {
//        throw new Exception("KABOOOM");

        if(priceToCalc.FromCountry == CountryEnum.NL && priceToCalc.WeightKg > 10)
            throw new FromCountryWeightException($"Package weight may not exceed 10kg for fromCountry NL. Value provided was {priceToCalc.WeightKg}");

        return new PriceQuoteResponseContract
            {
              Price = Calc(priceToCalc),
              ValidUntil = DateTime.UtcNow.AddHours(48)
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
