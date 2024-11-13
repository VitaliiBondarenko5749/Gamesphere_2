namespace Aggregator.Services;

public interface IFileService
{
    Task<IFormFile> ChangeNameAsync(IFormFile file, string newName);
}

public class FileService : IFileService
{
    public async Task<IFormFile> ChangeNameAsync(IFormFile file, string newName)
    {
        MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);

        byte[] fileBytes = memoryStream.ToArray();

        IFormFile renamedFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length,
                                              file.Name, newName)
        {
            Headers = file.Headers,
            ContentType = file.ContentType
        };

        return renamedFile;
    }
}