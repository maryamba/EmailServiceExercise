using System;
using System.Threading;
using EmailService.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockEmailClient;
using Polly;

namespace EmailService.Service
{
    public class EmailingService : IEmailService
    {
        private readonly ILogger<EmailingService> _logger;
        private readonly IEmailClient _emailClient;
        private readonly IConfiguration _configuration;
             
        public EmailingService(IEmailClient emailClient, ILogger<EmailingService> logger,IConfiguration configuration)
        {
            _emailClient = emailClient;
            _logger = logger;
            _configuration = configuration;
            
        }
        
        private int GetRetryNumber()
        {
             var retryNumber=  _configuration["RetryPolicy:RetryNumber"];  
             if(String.IsNullOrEmpty(retryNumber))
             {
                 return 0;
             }
             else 
             {
                 int number;
                 bool succesful = Int32.TryParse(retryNumber,out number);
                 if(!succesful)
                 {
                     _logger.LogError("Retry number in configuration is not valid");
                     return 0;                     
                 }  
                 else
                 {
                     return number;
                 }          
             }
              
        }
        public string SendEmail(Email email)
        {
            _logger.LogInformation($"Sending email to {email.To}");
            try
            {
                 var retryNumber= GetRetryNumber();    
                                              
                var retryPolicy = Policy.Handle<Exception>(ex => ex.Message == "Connection Failed")                         
                .Retry(retryNumber, (ex, retryCount) =>
                    {
                        _logger.LogInformation(ex, $"Retrying sending email to {email.To}, retry count: {retryCount}");
                    });

                retryPolicy.Execute(() =>
                   {
                       _emailClient.SendEmail(email.To, email.Body);

                   });                   
                return "Success!";
            }
            catch (Exception e)
            {

                _logger.LogError(e, $"Error sending email to {email.To}");
                return "Failure.";
            }
            finally
            {
                try
                {
                    _emailClient.Close();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error closing the email client, email to {email.To}");
                }
            }
        }
    }
}
