
using Common.Events;
using Common.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IKafkaProducerService _producer;

        public OrdersController(IKafkaProducerService producer)
        {
            _producer = producer;
        }

        [HttpPost("create-bill")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreatedEvent user)
        {
            await _producer.ProduceAsync("bil-created-topic", user);
            return Ok("BillCreatedEvent sent.");
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreatedEvent order)
        {
            await _producer.ProduceAsync("order-created-topic", order);
            return Ok("OrderCreatedEvent sent.");
        }
    }
}
