using DAMApi.Models.DTOs;
using DAMApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DAMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;
        public EmailController(IEmailService emailService, 
            ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailReceiverDto request)
        {
            _emailService.SendEmail(request);
            Response.StatusCode = StatusCodes.Status200OK;
            _logger.LogInformation("Email sent Successfully!");
            return Ok("Email sent successfully!");
        }
    }
}
