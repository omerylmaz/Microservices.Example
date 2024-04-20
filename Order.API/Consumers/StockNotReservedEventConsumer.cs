using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer(OrderDbContext orderDbContext) : IConsumer<StockNotReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var stockNotReservedEvent = context.Message;
            Console.Out.WriteLine(stockNotReservedEvent.OrderId + " - " + stockNotReservedEvent.BuyerId + " - " + stockNotReservedEvent.Message);

            var order = await orderDbContext.Orders.FirstOrDefaultAsync(x => x.Id == stockNotReservedEvent.OrderId);
            
            order.Status = OrderStatus.Canceled;

            await orderDbContext.SaveChangesAsync();
        }
    }
}
