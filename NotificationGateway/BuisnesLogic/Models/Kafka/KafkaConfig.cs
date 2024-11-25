using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Kafka
{
    public class KafkaConfig
    {
        public required string Host;
        public string ClientId = "webserverclient";
        public string GroupId = "consumerGroupTest";
        public IEnumerable<string>? Topics;
    }
}
