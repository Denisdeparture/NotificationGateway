using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Kafka
{
    public record KafkaFileTransport(byte[] Data, string Name, bool IsItAttachment);
}
