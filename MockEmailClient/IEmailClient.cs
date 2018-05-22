using System.Threading.Tasks;

namespace MockEmailClient
{
    public interface IEmailClient
    {
        void SendEmail(string to, string body);
        void Close();
    }
}