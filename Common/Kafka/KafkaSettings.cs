using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string OrderCreatedTopic { get; set; } = "order-created";
        public string OrderCreatedDltTopic { get; set; } = "order-created-dlt";
        public string ClientId { get; set; } = "service-client";
        public string GroupId { get; set; } = "service-group";
    }
}
