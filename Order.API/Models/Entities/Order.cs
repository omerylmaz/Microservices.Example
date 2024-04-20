using Order.API.Models.Enums;

namespace Order.API.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public long TotalPrice { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
