using System;
using EmailService.Contracts;
using EmailService.Service;
using Microsoft.Extensions.Logging;
using MockEmailClient;
using Moq;
using Xunit;
using Polly;
using Microsoft.Extensions.Configuration;

namespace EmailServiceTests
{
    public class EmailingServiceTests
    {
        private readonly EmailingService _sut;
        private readonly Mock<IEmailClient> _mockClient;
        private readonly Mock<ILogger<EmailingService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        public EmailingServiceTests()
        {
            _mockClient = new Mock<IEmailClient>();
            _mockLogger = new Mock<ILogger<EmailingService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            _sut = new EmailingService(_mockClient.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void Should_Send_Emails_to_Email_Client()
        {
            var email = new Email { To = "George", Body = "Very Important!" };

            _sut.SendEmail(email);
            _sut.SendEmail(email);
            _sut.SendEmail(email);
            _sut.SendEmail(email);

            _mockClient.Verify(call => call.SendEmail(email.To, email.Body), Times.Exactly(4));
        }

        [Fact]
        public void Should_Handle_SendEmail_Failure()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Sending error"));

            var result = _sut.SendEmail(email);

            Assert.Equal("Failure.", result);
        }

        [Fact]
        public void Should_Close_the_EmailClient_on_Send_Email_Success()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _sut.SendEmail(email);
            _mockClient.Verify(call => call.Close());
        }

        [Fact]
        public void Should_Close_the_EmailClient_on_Send_Email_Failure()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Sending error"));
            _sut.SendEmail(email);
            _mockClient.Verify(call => call.Close());
        }


        [Fact]
        public void Should_Handle_Close_EmailClient_Exception_on_Send_Email_Success()
        {

            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.Close()).Throws(new Exception("Unexpected Error"));
            var result = _sut.SendEmail(email);
            Assert.Equal("Success!", result);
        }

        [Fact]

        public void Should_Handle_Close_EmailClient_Exception_on_Send_Email_Failure()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Sending error"));
            _mockClient.Setup(call => call.Close()).Throws(new Exception("Unexpected Error"));
            var result = _sut.SendEmail(email);
            Assert.Equal("Failure.", result);
        }


        [Fact]
        public void Should_Retry_On_Connection_Failed_Exception()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            var retryNumber = 2;
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Connection Failed"));

            _mockConfiguration.SetupGet(m => m["RetryPolicy:RetryNumber"]).Returns("2");

            var result = _sut.SendEmail(email);
            _mockClient.Verify(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(retryNumber + 1));
            Assert.Equal("Failure.", result);
        }

        [Fact]
        public void Should_Not_Retry_On_Connection_Failed_Exception_If_RetryNumber_Does_Not_Exixt_in_Configuration()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Connection Failed"));
            var result = _sut.SendEmail(email);
            _mockClient.Verify(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.Equal("Failure.", result);
        }

        [Fact]
        public void Should_Not_Retry_On_Connection_Failed_Exception_If_RetryNumber_Is_Invalid_in_Configuration()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Connection Failed"));
            _mockConfiguration.SetupGet(m => m["RetryPolicy:RetryNumber"]).Returns("Invalid Interger!");
            var result = _sut.SendEmail(email);
            _mockClient.Verify(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.Equal("Failure.", result);
        }

        [Fact]
        public void Should_Not_Retry_OnException_Not_Equal_To_Connection_Failed_Exception()
        {
            var email = new Email { To = "George", Body = "Very Important!" };
            _mockClient.Setup(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Another Exception"));
            var result = _sut.SendEmail(email);
            _mockClient.Verify(call => call.SendEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.Equal("Failure.", result);
        }

    }
}
