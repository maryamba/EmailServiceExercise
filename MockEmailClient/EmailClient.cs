using System;
using System.Threading.Tasks;

namespace MockEmailClient
{
    // DO NOT MODIFY
    public class EmailClient : IEmailClient
    {
        /* maxConnections and Connections are meant to simulate
         a limited resource */

        private readonly int _maximumNumberOfConnections;
        private int _numberOfConnections;

        /* Chance of failure and random are meant to reproduce
         an unexpected error in the Close() method

        In reality, some close methods like FileStream.Close 
        can throw due to not enough space on the hard disk */

        private readonly int _chanceOfFailure;
        private readonly Random _random;

        public EmailClient(int maximumNumberOfConnections)
        {
            _maximumNumberOfConnections = maximumNumberOfConnections;
            _chanceOfFailure = 10;
            _numberOfConnections = 0;
            _random = new Random();
        }

        public void SendEmail(string to, string body)
        {
            _numberOfConnections++;

            if (_numberOfConnections > _maximumNumberOfConnections)
            {
                throw new Exception("Connection Failed");
            }

            //Assume a call to either an email API provider like sendgrid, or even a validator for the email is made

            Task.Delay(5000).Wait();
        }

        public void Close()
        {
            _numberOfConnections--;

            //Assume the finalize call to send the actual email is made

            if (_random.Next(_chanceOfFailure) == _chanceOfFailure - 1)
            {
                throw new Exception("Unexpected Error");
            }
        }
    }
}
