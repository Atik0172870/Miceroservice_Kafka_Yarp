using Common.Events;
using Common.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace OrderService.Kafka.Consumer
{
    public class OrderConsumerService : BackgroundService
    {
        private readonly ILogger<OrderConsumerService> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly string[] _topics = { "send-bill-topic", "confirm-bill-topic" };

        public OrderConsumerService(ILogger<OrderConsumerService> logger, IOptions<KafkaSettings> ks)
        {
            _logger = logger;
            _kafkaSettings = ks.Value;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = "order-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();

            // --- CHECK EXISTING TOPICS USING ADMIN CLIENT ---
            using var admin = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            }).Build();

            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(5));

            var existingTopics = _topics
                .Where(t => metadata.Topics.Any(mt => mt.Topic == t && mt.Error.Code == ErrorCode.NoError))
                .ToList();

            if (!existingTopics.Any())
            {
                _logger.LogWarning("No existing topics found. Consumer will not subscribe.");
            }
            else
            {
                _logger.LogInformation("Subscribing to: {topics}", string.Join(", ", existingTopics));
                consumer.Subscribe(existingTopics);
            }

            // --- CONSUME LOOP ---
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(stoppingToken);
                string topic = cr.Topic;
                string json = cr.Message.Value;

                switch (topic)
                {
                    case "send-bill-topic":
                        var user = JsonSerializer.Deserialize<SendBillEvent>(json);
                        _logger.LogInformation("send-bill-topic received UserCreatedEvent: {@user}", user);
                        Console.Write("\n==================================\n");
                        Console.Write($"{topic} Received, data: {json}");
                        Console.Write("\n==================================\n");
                        break;

                    case "confirm-bill-topic":
                        var order = JsonSerializer.Deserialize<ConfirmBillEvent>(json);
                        _logger.LogInformation("confirm-bill-topic received OrderCreatedEvent: {@order}", order);
                        Console.Write("==================================\n");
                        Console.Write($"{topic} Received, data: {json}");
                        Console.Write("\n==================================\n");
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
