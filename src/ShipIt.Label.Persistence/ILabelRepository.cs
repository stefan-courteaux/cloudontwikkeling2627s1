using System;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace ShipIt.Label.Persistence;

public class LabelRepoOptions
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}

public interface ILabelRepository
{
    Task WriteAsync(byte[] content, string trackingNumber);
    Task<byte[]> ReadAsync(string trackingNumber);
}

public class LabelRepository(IOptions<LabelRepoOptions> _options) : ILabelRepository
{
    public async Task<byte[]> ReadAsync(string trackingNumber)
    {
        var blob = GetBlobClient($"{trackingNumber}.pdf");
        var download = await blob.DownloadContentAsync();
        return download.Value.Content.ToArray();
    }

    public async Task WriteAsync(byte[] content, string trackingNumber)
    {
        var blob = GetBlobClient($"{trackingNumber}.pdf");
        await blob.UploadAsync(new BinaryData(content));
    }

    private BlobClient GetBlobClient(string fileName)
    {
        var client = new BlobServiceClient(_options.Value.ConnectionString);
        var containerClient = client.GetBlobContainerClient(_options.Value.ContainerName);
        return containerClient.GetBlobClient(fileName);
    }    
}