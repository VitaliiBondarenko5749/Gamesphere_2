using Helpers;
using Aggregator.Models.CloudStorage;
using Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DropboxController : ControllerBase
{
    private readonly IGameService gameService;
    private readonly IFileService fileService;
    private readonly ICloudStorageService cloudStorageService;

    public DropboxController(IGameService gameService, IFileService fileService, ICloudStorageService cloudStorageService)
    {
        this.gameService = gameService;
        this.fileService = fileService;
        this.cloudStorageService = cloudStorageService;
    }

    [HttpPost("UploadGameImage")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<ActionResult<ServerResponse>> UploadGameImageAsync([FromForm] AddGameImageViewModel model)
    {
        try
        {
            string randomChars = StringHelper.GenerateRandomString(5);

            model.Image = await fileService.ChangeNameAsync(model.Image, $"{model.GameName}-{randomChars}-{model.Image.FileName}".Replace(' ', '-'));

            string fullDirectory = $"{model.Directory}/{model.Image.FileName}";

            UploadFileViewModel uploadFileViewModel = new() { File = model.Image, Folder = model.Directory };

            ServerResponse? response = null;

            if (model.Directory.Equals("Games/Icons"))
            {
                response = await gameService.UpdateGameIconAsync(model.GameName, fullDirectory);
            }
            else if (model.Directory.Equals("Games/Images"))
            {
                response = await gameService.AddImageToGameAsync(model.GameName, fullDirectory);
            }

            if (response is not null && response.IsSuccess)
            {
                await cloudStorageService.UploadFileAsync(uploadFileViewModel);
            }

            return new ServerResponse { Message = "Something went wrong...", IsSuccess = false };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpDelete("DeleteGameImage")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<ActionResult<ServerResponse>> DeleteGameImageAsync([FromQuery] DeleteGameImageViewModel model)
    {
        try
        {
            await cloudStorageService.DeleteFileAsync(model.Directory);
            await gameService.RemoveImageFromGameAsync(model.Id);

            return new ServerResponse { Message = "Image has been removed!", IsSuccess = true };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }
}