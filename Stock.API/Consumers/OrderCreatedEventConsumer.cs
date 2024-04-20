using MassTransit;
using MongoDB.Driver;
using Shared.Constants;
using Shared.Events;
using Stock.API.Contexts;
using StockModel = Stock.API.Models.Entities.Stock;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        IMongoCollection<StockModel> _stockCollection;
        public OrderCreatedEventConsumer(MongoDbContext mongoDbContext)
        {
            _stockCollection = mongoDbContext.GetCollection<StockModel>();
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {

            // Get the order from the event
            var order = context.Message;
            Console.Out.WriteLine(order.OrderId + " - " + order.BuyerId);

            var stockAvailableList = new List<bool>();
            foreach (var orderItem in order.OrderItems)
            {
                stockAvailableList.Add(_stockCollection.Find(x => x.ProductId == orderItem.ProductId && x.Count >= orderItem.Amount).Any());
            }

            if(stockAvailableList.TrueForAll(x => x.Equals(true)))
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var stockItem = _stockCollection.Find(x => x.ProductId == orderItem.ProductId).FirstOrDefault();
                    stockItem.Count -= orderItem.Amount;
                    _stockCollection.ReplaceOne(x => x.ProductId == orderItem.ProductId, stockItem);
                }

                StockReservedEvent stockReservedEvent = new StockReservedEvent
                {
                    OrderId = order.OrderId,
                    BuyerId = order.BuyerId,
                    TotalPrice = order.TotalPrice,
                };

                var sendEnpoint = await context.GetSendEndpoint(new Uri($"queue:{MessageQueueConstants.Payment_StockReservedQueue}"));
                await sendEnpoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent
                {
                    OrderId = order.OrderId,
                    BuyerId = order.BuyerId,
                    Message = "Stock not available",
                };

                await context.Publish(stockNotReservedEvent);
            }
            //return Task.CompletedTask;
            //// Get the stock item from the database
            //var stockItem = await _stockDbContext.StockItems
            //    .FirstOrDefaultAsync(x => x.ProductId == order.ProductId);

            //// Update the stock item
            //stockItem.Quantity -= order.Quantity;

            //// Save the changes
            //await _stockDbContext.SaveChangesAsync();
        }
    }
}
