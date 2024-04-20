using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class MessageQueueConstants
    {
        public const string Stock_OrderCreatedQueue = "stock-order-created-event-queue";
        public const string Payment_StockReservedQueue = "payment-stock-reserved-event-queue";
        public const string Order_PaymentSuccededEventQueue = "order-payment-succeded-event-queue";
        public const string Order_PaymentFailedEventQueue = "order-payment-failed-event-queue";
        public const string Order_StockNotReservedEventQueue = "order-stock-not-reserved-event-queue";
        public const string Stock_OrderFailedQueue = "stock-order-failed-event-queue";
    }
}
