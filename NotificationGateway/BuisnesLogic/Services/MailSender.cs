using BuisnesLogic.Models.Messages;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
namespace BuisnesLogic.Realization
{
    public class MailSender : ISender
    {
        private IConfiguration _config;
        public MailSender(IConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public void SendMailSmtp(MimeMessage message, SmtpMessageModel config_message)
        {
            if(message is null) throw new ArgumentNullException(nameof(message));
            if(config_message is null) throw new ArgumentNullException(nameof(config_message));
            var array_nulls = config_message.GetType().GetProperties().Where(x => x.GetValue(config_message) is null).ToArray();
            if (array_nulls.Length > 0) throw new ArgumentException($"Params in {config_message} were null");
            const string protocol = "smtp";
            string connectionString = string.Format("{0}.{1}.{2}", protocol, config_message.provider, config_message.domain_region);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(connectionString, config_message.port, config_message.useSsl);
                smtpClient.Authenticate(_config[$"SmtpClients:{config_message.provider}:User"], _config[$"SmtpClients:{config_message.provider}:Pass"]);
                message.To.Add(new MailboxAddress(string.Empty, config_message.RecipientEmail));
                message.From.Add(new MailboxAddress(config_message.nameCompanyOrAdministration, config_message.SenderEmail));
                smtpClient.Send(message);
            }
        }
    }
}
