using MassTransit;
using System.Reflection;

namespace WorkerService2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();

                    services.AddMassTransit(config =>
                    {
                        config.AddConsumers(Assembly.GetExecutingAssembly());

                        config.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "requests", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });
                })
               .UseDefaultServiceProvider(options => options.ValidateOnBuild = false)
               .Build();

            host.Run();
        }
    }
}