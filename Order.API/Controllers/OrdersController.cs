using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(OrderDbContext orderDbContext, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddOrder(AddOrderDto addOrderDto)
        {
            var order = new Order.API.Models.Entities.Order
            {
                Id = Guid.NewGuid(),
                BuyerId = addOrderDto.BuyerId,
                CreateDate = DateTime.Now,
                Status = OrderStatus.Pending,
            };

            order.OrderItems = addOrderDto.OrderItems.Select(orderItemDto => new OrderItem
            {
                Quantity = orderItemDto.Quantity,
                Price = orderItemDto.Price,
                ProductId = orderItemDto.ProductId,
            }).ToList();

            order.TotalPrice = addOrderDto.OrderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            await orderDbContext.Orders.AddAsync(order);
            await orderDbContext.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                OrderId = order.Id,
                BuyerId = order.BuyerId,
                OrderItems = order.OrderItems.Select(orderItem => new Shared.Messages.OrderItemMessage
                {
                    Amount = orderItem.Quantity,
                    ProductId = orderItem.ProductId,
                }).ToList(),
                TotalPrice = order.TotalPrice,
            };

            await publishEndpoint.Publish(orderCreatedEvent);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
