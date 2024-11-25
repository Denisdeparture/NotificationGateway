using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public class SmsMessageModel
    {
        public string? NumberSender { get; set; }
        public string? NumberReceiver { get; set; }
        public string? Message { get; set; }
        public SmsMessageModel(string number, string destination, string text)
        {
            Message = text;
            NumberSender = number;
            NumberReceiver = destination;
        }
        public SmsMessageModel() { }

    }

}
