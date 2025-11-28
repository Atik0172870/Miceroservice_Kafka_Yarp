using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Events
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}
