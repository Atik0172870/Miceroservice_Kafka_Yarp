using BillingService.Kafka.Consumer;
using Common.Kafka;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Bind Kafka settings from appsettings.json
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

// Register KafkaProducerService for PaymentService
builder.Services.AddSingleton<IKafkaProducerService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<KafkaSettings>>();
    return new KafkaProducerService(options.Value.BootstrapServers);
});

builder.Services.AddHostedService<PaymentConsumerService>();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
