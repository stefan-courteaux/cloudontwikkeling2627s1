using System;

namespace ShipIt.Label.Domain.DTO;

public class LabelCreationDTO
{
    public required string RecipientName { get; init; }
    public required string RecipientAddressLine1 { get; init; }
    public required string RecipientAddressLine2 { get; init; }
    public required string RecipientCountry { get; init; }
    public required double ShipmentPrice { get; init; }
}