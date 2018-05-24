using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        
        [HttpPost]
        public IActionResult Post([FromBody] Email email)
        {
            // Below solution is simple and short solution but to have a complete 
            //solution ,We can store the Email in a queue and then return acknowledgement
            // We also need to have a queue processor that dequeue messages(emails) 
            //and sends email and then it sends the coresponding http response back to the client

            Task.Factory.StartNew(() => _emailService.SendEmail(email));
            return Accepted();      
        }       

    }
}