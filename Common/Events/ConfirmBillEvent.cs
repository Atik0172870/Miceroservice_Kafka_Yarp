using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class ConfirmBillEvent
    {
        public int BillId { get; set; }
        public string BillName { get; set; }
        public int Amount { get; set; }
    }
}
