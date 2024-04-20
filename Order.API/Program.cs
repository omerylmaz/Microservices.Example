using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Order.API.Consumers;
using Order.API.Models;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<PaymentSuccededEventConsumer>();
    config.AddConsumer<StockNotReservedEventConsumer>();
    config.AddConsumer<PaymentFailedEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"]);
        
        cfg.ReceiveEndpoint(MessageQueueConstants.Order_PaymentSuccededEventQueue, 
            e => e.ConfigureConsumer<PaymentSuccededEventConsumer>(ctx));
        cfg.ReceiveEndpoint(MessageQueueConstants.Order_StockNotReservedEventQueue,
            e => e.ConfigureConsumer<StockNotReservedEventConsumer>(ctx));
        cfg.ReceiveEndpoint(MessageQueueConstants.Order_PaymentFailedEventQueue,
            e => e.ConfigureConsumer<PaymentFailedEventConsumer>(ctx));
    });
});




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
    c.RoutePrefix = string.Empty; // Set the Swagger UI at the root URL
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
