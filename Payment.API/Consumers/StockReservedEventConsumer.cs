using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private const int BuyerPriceLimit = 500;
        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var stockReservedEvent = context.Message;
            
            if (stockReservedEvent.TotalPrice < BuyerPriceLimit)
            {
                _publishEndpoint.Publish(new PaymentSuccededEvent
                {
                    OrderId = stockReservedEvent.OrderId,
                });
            }
            else
            {
                _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    OrderId = stockReservedEvent.OrderId,
                });
            }


            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
