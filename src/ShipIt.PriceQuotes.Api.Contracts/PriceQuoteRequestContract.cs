using System;
using System.ComponentModel.DataAnnotations;
using ShipIt.Shared;

namespace ShipIt.PriceQuote.Api.Contracts;

public class PriceQuoteRequestContract
{
    [Required]
    public double WidthCm { get; set; }
    [Required]
    public double LengthCm { get; set; }
    [Required]
    public double HeightCm { get; set; }
    [Required]
    public double WeightKg { get; set; }
    [Required]
    public CountryEnum? FromCountry { get; set; }
    [Required]
    public CountryEnum? ToCountry { get; set; }
}
