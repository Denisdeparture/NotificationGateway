using BuisnesLogic.Models.Kafka;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public class MailMessageTransportContract : MessageTransportContract
    {
        public SmtpMessageModel config { get; set; } = null!;
    }
}
