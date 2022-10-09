using Azure.Storage.Blobs; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Services;

public interface IStorageService
{
    /// <summary>
    /// Save a file to storage source and return its full url 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> SaveFileAsync(Stream stream, string fileName);

    /// <summary>
    /// Delete a file by using its full path 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Task DeleteFileAsync(string filePath);
}

public class AzureBlobStorageService : IStorageService
{

    private readonly string _connectionString;

    public AzureBlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName)
    {
        var container = await GetContainerAsync();

        // Retrieve and validate the extension 
        var extension = Path.GetExtension(fileName).ToLower(); // 34534.png => .png 
        var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
        if (!validExtensions.Contains(extension))
            throw new NotSupportedException($"The extension {extension} is not supported");

        // Generate a new uniqe name 
        var nameOnly = Path.GetFileNameWithoutExtension(fileName); // 34534.png => 34534
        var newFileName = $"{nameOnly}-{Guid.NewGuid()}{extension}"; // 34534-345345345345.png

        var blob = container.GetBlobReference(newFileName);
        await blob.UploadFromStreamAsync(stream);
    }

    private async Task<CloudBlobContainer> GetContainerAsync()
    {
        var blobClient = new BlobServiceClient(_connectionString);
        var container = blobClient.
        var container = blobClient.GetContainerReference("attachments");
        await container.CreateIfNotExistsAsync();
        return container;
    }
}