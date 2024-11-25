using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public class SmsMessageTransportContract : MessageTransportContract
    {
        public SmsMessageModel SmsMessageConfig { get; set; } = new SmsMessageModel();
    }
}
