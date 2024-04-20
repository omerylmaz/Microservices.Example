using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class OrderFailedEvent
    {
        public Guid OrderId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
