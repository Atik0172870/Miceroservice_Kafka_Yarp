using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public record OrderCreated(Guid OrderId, Guid CustomerId, decimal Amount, DateTime CreatedAt);
}
