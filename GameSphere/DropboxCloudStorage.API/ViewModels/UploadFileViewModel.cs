using System.ComponentModel.DataAnnotations;

namespace DropboxCloudStorage.API.ViewModels;

public class UploadFileViewModel
{
    [Required(ErrorMessage = "FileName can not be empty!")]
    public string FileName { get; set; }

    [Required]
    public byte[] FileBytes { get; set; }

    [Required(ErrorMessage = "Folder is required!")]
    public string Folder { get; set; }
}