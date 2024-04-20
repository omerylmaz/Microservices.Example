using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<Entities.Order> Orders { get; set; }
        public DbSet<Entities.OrderItem> OrderItems { get; set; }
    }
}
