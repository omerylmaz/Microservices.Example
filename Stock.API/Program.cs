using MassTransit;
using Stock.API.Consumers;
using Stock.API.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderCreatedEventConsumer>();
    config.AddConsumer<OrderFailedEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"]);

        cfg.ReceiveEndpoint(Shared.Constants.MessageQueueConstants.Stock_OrderCreatedQueue, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(Shared.Constants.MessageQueueConstants.Stock_OrderFailedQueue, e =>
        {
            e.ConfigureConsumer<OrderFailedEventConsumer>(ctx);
        });
    });
});

builder.Services.AddSingleton<Stock.API.Contexts.MongoDbContext>();


using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDbContext context = scope.ServiceProvider.GetService<MongoDbContext>();
context.SeedData();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
