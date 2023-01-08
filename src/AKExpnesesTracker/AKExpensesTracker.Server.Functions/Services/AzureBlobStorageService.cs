using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Services;

public class AzureBlobStorageService : IStorageService
{

    private readonly string _connectionString;

    public AzureBlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var container = await GetContainerAsync();

        var fileName = Path.GetFileName(filePath);
        var blob = container.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName)
    {
        var container = await GetContainerAsync();

        // Retrieve and validate the extension 
        var extension = Path.GetExtension(fileName).ToLower(); // 34534.png => .png 
       
        // Generate a new uniqe name 
        var nameOnly = Path.GetFileNameWithoutExtension(fileName); // 34534.png => 34534
        var newFileName = $"{nameOnly}-{Guid.NewGuid()}{extension}"; // 34534-345345345345.png

        var blob = container.GetBlobClient(newFileName);
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(extension)
            }
        });
		
        return blob.Uri.AbsoluteUri;
    }

    private async Task<BlobContainerClient> GetContainerAsync()
    {
        var blobClient = new BlobServiceClient(_connectionString);
        var container = blobClient.GetBlobContainerClient("attacments");
        await container.CreateIfNotExistsAsync();
        return container;
    }

    private string GetContentType(string extension)
    {
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => throw new NotSupportedException($"The extension {extension} is not supported")
        };
    }
}