using MassTransit;
using Payment.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<StockReservedEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"]);

        cfg.ReceiveEndpoint(Shared.Constants.MessageQueueConstants.Payment_StockReservedQueue, e =>
        {
            e.ConfigureConsumer<StockReservedEventConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
