using System;

namespace ShipIt.PriceQuotes.Storage;

public class PriceQuoteRepositoryOptions
{
    public string Connectionstring { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
}