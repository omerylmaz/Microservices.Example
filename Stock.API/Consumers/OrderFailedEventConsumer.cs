using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Stock.API.Contexts;
using StockModel = Stock.API.Models.Entities.Stock;

namespace Stock.API.Consumers
{
    public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
    {
        private readonly IMongoCollection<StockModel> _stockCollection;
        public OrderFailedEventConsumer(MongoDbContext context)
        {
            _stockCollection = context.GetCollection<StockModel>();
        }
        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            var paymentFailedEvent = context.Message;

            Console.WriteLine($"Payment failed for order {paymentFailedEvent.OrderId}");

           paymentFailedEvent.OrderItems.ForEach(async orderItem =>
           {
                var stockItem = await _stockCollection.Find(x => x.ProductId == orderItem.ProductId).FirstOrDefaultAsync();
                stockItem.Count += orderItem.Amount;
                await _stockCollection.ReplaceOneAsync(x => x.ProductId == orderItem.ProductId, stockItem);
            });
        }
    }
}
