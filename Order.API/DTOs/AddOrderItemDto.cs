namespace Order.API.DTOs
{
    public class AddOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
    }
}
