using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ShipIt.Label.Domain.Model;

internal class LabelModel : IDocument
{
    public AddressModel Address { get; set; }
    public string TrackingNumber { get; set; }
    public double Price { get; set; }

    public LabelModel(AddressModel address, double price)
    {
        Address = address;
        Price = price;
        TrackingNumber = Guid.NewGuid().ToString();
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
		{
			page.Size(PageSizes.A4);
			page.Margin(4.0f, Unit.Centimetre);

			page.Header()
				.Text($"ShipIt Label {TrackingNumber}")
				.FontSize(28)
				.Bold()
				.FontColor(Colors.Red.Darken2);

			page.Content()
				.PaddingVertical(8)
				.Column(column =>
				{
 					column.Item().Text(Address.Name);
                    column.Item().Text(Address.AddressLine1);
                    column.Item().Text(Address.AddressLine2);
                    column.Item().Text(Address.Country);
                    column.Item().Text(Price.ToString());
				});

			page.Footer()
				.AlignCenter()
				.Text("De kleine lettertjes");
		});
    }
}
