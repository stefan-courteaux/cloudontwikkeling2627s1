using System;

namespace ShipIt.PriceQuote.Api.Contracts;

public class PriceQuoteResponseContract
{
    public required string Id { get; set; }
    public decimal Price { get; set; }
    public DateTime ValidUntil { get; set; }
}
