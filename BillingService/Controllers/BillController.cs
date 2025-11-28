using Common.Events;
using Common.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IKafkaProducerService _producer;
        public BillController(IKafkaProducerService producer)
        {
            _producer = producer;
        }

        [HttpPost("send-bill")]
        public async Task<IActionResult> CreateUser([FromBody] SendBillEvent bill)
        {
            await _producer.ProduceAsync("send-bill-topic", bill);
            return Ok("SendBillTopicEvent sent.");
        }

        [HttpPost("confirm-bill")]
        public async Task<IActionResult> CreateOrder([FromBody] ConfirmBillEvent order)
        {
            await _producer.ProduceAsync("confirm-bill-topic", order);
            return Ok("ConfirmBillEvent sent.");
        }
    }
}
