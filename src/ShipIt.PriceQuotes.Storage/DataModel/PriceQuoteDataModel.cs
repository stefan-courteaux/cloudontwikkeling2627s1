using ShipIt.Shared;

public class PriceQuoteDataModel
{
    public string id {get;set;}
    public double widthCm { get; set; }
    public double heightCm { get; set; }
    public double lengthCm { get; set; }
    public double weightKg { get; set; }
    public CountryEnum fromCountry { get; set; }
    public CountryEnum toCountry { get; set; }
    public DateTime createdOn { get; set; }
    public DateTime validUntil { get; set; }
    public decimal price { get; set; }
}