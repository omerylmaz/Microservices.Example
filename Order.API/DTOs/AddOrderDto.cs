using Order.API.Models.Entities;
using Order.API.Models.Enums;

namespace Order.API.DTOs
{
    public class AddOrderDto
    {
        public Guid BuyerId { get; set; }
        public List<AddOrderItemDto> OrderItems { get; set; }
    }
}
