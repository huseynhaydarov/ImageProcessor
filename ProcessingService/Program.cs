using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProcessingService;
using ProcessingService.Consumers;
using ProcessingService.Data;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        // Configure MassTransit
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<PhotoUploadedConsumer>();

            cfg.UsingRabbitMq((context, rabbitMqConfig) =>
            {
                rabbitMqConfig.Host(new Uri(hostContext.Configuration["RabbitMq:Host"]), h =>
                {
                    h.Username(hostContext.Configuration["RabbitMq:Username"]);
                    h.Password(hostContext.Configuration["RabbitMq:Password"]);
                });

                rabbitMqConfig.ReceiveEndpoint("photo-uploaded", e =>
                {
                    e.ConfigureConsumer<PhotoUploadedConsumer>(context);
                });
            });
        });        
       
    });

var host = builder.Build();
await host.RunAsync();