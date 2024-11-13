using System.ComponentModel.DataAnnotations;

namespace Aggregator.Models.CloudStorage;

public class UploadFileViewModel
{
    [Required(ErrorMessage = "File can not be empty!")]
    public IFormFile File { get; set; } = default!;

    [Required(ErrorMessage = "Folder is required!")]
    public string Folder { get; set; } = default!;
}
