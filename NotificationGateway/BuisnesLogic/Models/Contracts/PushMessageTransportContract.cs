using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public class PushMessageTransportContract : MessageTransportContract
    {
        public string? UriForWebPush { get; set; }
        public string TypePush { get; set; } = null!;

    }
}
