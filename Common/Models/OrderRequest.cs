using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class OrderRequest
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
