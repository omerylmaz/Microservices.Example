using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentSuccededEventConsumer(OrderDbContext orderDbContext) : IConsumer<PaymentSuccededEvent>
    {
        public async Task Consume(ConsumeContext<PaymentSuccededEvent> context)
        {
            var paymentSuccededEvent = context.Message;

            var order = await orderDbContext.Orders.FindAsync(paymentSuccededEvent.OrderId);
            order.Status = OrderStatus.Completed;
            await orderDbContext.SaveChangesAsync();
        }
    }
}
