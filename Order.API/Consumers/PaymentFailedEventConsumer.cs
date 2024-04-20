using MassTransit;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Constants;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer(OrderDbContext orderDbContext) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var paymentFailedEvent = context.Message;

            Console.WriteLine($"Payment failed for order {paymentFailedEvent.OrderId}");

            var order = orderDbContext.Orders.FirstOrDefault(o => o.Id == paymentFailedEvent.OrderId);
            order.OrderItems = [.. orderDbContext.OrderItems.Where(x => x.OrderId == order.Id)];

            order.Status = OrderStatus.Canceled;

            var orderFailedEvent = new OrderFailedEvent
            {
                OrderId = order.Id,
                OrderItems = order.OrderItems.Select(oi => new OrderItemMessage
                {
                    ProductId = oi.ProductId,
                    Amount = oi.Quantity
                }).ToList()
            };

            var sendEnpoint = await context.GetSendEndpoint(new Uri($"queue:{MessageQueueConstants.Stock_OrderFailedQueue}"));
            await sendEnpoint.Send(orderFailedEvent);
            await orderDbContext.SaveChangesAsync();
        }
    }
}
