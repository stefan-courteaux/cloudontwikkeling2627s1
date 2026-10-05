using System;

namespace ShipIt.PriceQuotes.Domain.ShipItExceptions;

public class FromCountryWeightException(string message) : Exception (message);
