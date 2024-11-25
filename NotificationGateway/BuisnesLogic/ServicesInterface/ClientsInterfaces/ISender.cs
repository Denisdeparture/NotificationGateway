using BuisnesLogic.Models.Messages;
using MimeKit;
namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface ISender
    {
        public void SendMailSmtp(MimeMessage message, SmtpMessageModel config_message);
    }
}
