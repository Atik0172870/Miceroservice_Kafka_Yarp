using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _config;

        public KafkaProducerService(string bootstrapServers)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All
            };
        }

        public async Task ProduceAsync<T>(string topic, T message)
        {
            using var producer = new ProducerBuilder<Null, string>(_config).Build();
            string json = JsonSerializer.Serialize(message);
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
            Console.Write("\n==================================\n");
            Console.Write($"{topic} Send, data: {json}");
            Console.Write("\n==================================\n");
            producer.Flush();
        }
    }
  
    public interface IKafkaProducerService
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}
