using EmailService.Contracts;

namespace EmailService.Service
{
    public interface IEmailService
    {
        string SendEmail(Email email);
    }
}