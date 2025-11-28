
using Common.Events;
using Common.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BillingService.Kafka.Consumer
{
    public class PaymentConsumerService : BackgroundService
    {
        private readonly ILogger<PaymentConsumerService> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly string[] _topics = { "order-created-topic", "bill-created-topic" };
        private readonly IKafkaProducerService _producer;

        public PaymentConsumerService(ILogger<PaymentConsumerService> logger
            , IKafkaProducerService producer
            , IOptions<KafkaSettings> ks

            )
        {
            _logger = logger;
            _producer = producer;
            _kafkaSettings = ks.Value;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = "payment-service-group",
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
                    case "bill-created-topic":
                        var user = JsonSerializer.Deserialize<UserCreatedEvent>(json);
                        _logger.LogInformation("OrderService received UserCreatedEvent: {@user}", user);
                        Console.Write("\n==================================\n");
                        Console.Write($"{topic} Received, data: {json}");
                        Console.Write("\n==================================\n");
                        break;
                    case "order-created-topic":
                        var order = JsonSerializer.Deserialize<OrderCreatedEvent>(json);
                        _logger.LogInformation("OrderService received OrderCreatedEvent: {@order}", order);
                        Console.Write("\n==================================\n");
                        Console.Write($"{topic} Received, data: {json}");
                        Console.Write("\n==================================\n");
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
