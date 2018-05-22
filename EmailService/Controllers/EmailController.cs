using EmailService.Contracts;
using EmailService.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // POST api/email
        [HttpPost]
        public string Post([FromBody] Email email)
        {
            var status = _emailService.SendEmail(email);
            var response = $"Email to {email.To} received status {status}";
            return response;
        }
    }
}
