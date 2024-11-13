using System.ComponentModel.DataAnnotations;

namespace EmailService.API.ViewModels;

public class SendEmailViewModel
{
    [Required]
    public string SendTo { get; set; } = default!;

    [Required]
    public string Subject { get; set; } = default!;

    [Required]
    public string Message { get; set; } = default!;
    public bool IsHtml { get; set; } = false; 
}