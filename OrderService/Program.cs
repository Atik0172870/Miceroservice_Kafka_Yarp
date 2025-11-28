using Common.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderService.Kafka.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Bind Kafka settings
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

// Register producer using interface
builder.Services.AddSingleton<IKafkaProducerService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<KafkaSettings>>();
    return new KafkaProducerService(options.Value.BootstrapServers);
});

builder.Services.AddHostedService<OrderConsumerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
