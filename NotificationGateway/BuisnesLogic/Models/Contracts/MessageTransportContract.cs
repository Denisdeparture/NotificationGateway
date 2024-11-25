using BuisnesLogic.Models.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Messages
{
    public class MessageTransportContract
    {
        public string Message { get; set; } = null!;
        public IEnumerable<KafkaFileTransport>? Files { get; set; }
        public string? Subject { get; set; }
        public Guid Id { get; set; }
    }
}
