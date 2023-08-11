using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace FundooFunction
{
    public class SendEmailOnReceive
    {
        private readonly IConfiguration _configuration;
        private readonly string _senderEmail;
        private readonly string _senderAppPassword;

        private const int _port = 587;
        private const string _host = "smtp.gmail.com";

        public SendEmailOnReceive(IConfiguration configuration)
        {
            _configuration = configuration;

            _senderEmail = _configuration["SenderEmail"];
            _senderAppPassword = _configuration["SenderAppPassword"];
        }

        [FunctionName("SendEmailOnReceive")]
        public void Run([ServiceBusTrigger("messagequeue", Connection = "ServiceBusConnection")] string body, string to, string label)
        {
            using(SmtpClient smtpClient = new SmtpClient(_host))
            {
                smtpClient.Port = _port;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderAppPassword);

                smtpClient.Send(_senderEmail, to, label, body);
                smtpClient.Dispose();
            }
        }
    }
}
