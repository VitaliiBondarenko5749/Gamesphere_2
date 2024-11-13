using Helpers;
using Aggregator.Models.CloudStorage;

namespace Aggregator.Services;

public interface ICloudStorageService
{
    Task<ServerResponse?> UploadFileAsync(UploadFileViewModel model);
    Task<ServerResponse?> DeleteFileAsync(string filePath);
}

public class CloudStorageService : ICloudStorageService
{
    private readonly HttpClient httpClient;

    public CloudStorageService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ServerResponse?> UploadFileAsync(UploadFileViewModel model)
    {
        byte[] fileBytes;

        using (MemoryStream memoryStream = new())
        {
            await model.File.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }

        var requestData = new
        {
            FileName = model.File.FileName,
            FileBytes = fileBytes,
            Folder = model.Folder
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/Main/UploadFile", requestData);

        return await response.Content.ReadFromJsonAsync<ServerResponse>();
    }

    public async Task<ServerResponse?> DeleteFileAsync(string filePath)
    {
        string request = $"/api/Main/DeleteFile/{filePath}";


        HttpResponseMessage response = await httpClient.DeleteAsync(request);

        return await response.Content.ReadFromJsonAsync<ServerResponse>();
    }
}