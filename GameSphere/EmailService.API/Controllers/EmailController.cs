using EmailService.API.Services;
using EmailService.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> logger;
    private readonly IEmailSenderService emailSenderService;

    public EmailController(ILogger<EmailController> logger, IEmailSenderService emailSenderService)
    {
        this.logger = logger;
        this.emailSenderService = emailSenderService;
    }

    [HttpPost]
    public async Task<IActionResult> SendAsync([FromBody] SendEmailViewModel model)
    {
        try
        {
            await emailSenderService.SendEmailAsync(model);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(500, "Internal server error!");
        }
    }
}