namespace Shared.Messages
{
    public class OrderItemMessage
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}