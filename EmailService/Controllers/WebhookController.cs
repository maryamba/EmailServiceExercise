using System.Net;
using System.Net.Http;
using EmailService.Contracts;
using EmailService.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly IEmailService _emailService;

        public WebhookController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        
         // POST api/webhook
        [HttpPost]
        public IActionResult Post([FromBody] Email email)
        {
             var status = _emailService.SendEmail(email);
             if(status == "Success!")
              return  Created("", email);
             else
               return BadRequest($"Failed sending email");            
        }
    }
}