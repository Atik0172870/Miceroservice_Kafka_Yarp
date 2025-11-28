using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class PaymentCompletedEvent
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public bool IsSuccessful { get; set; }
    }

}
