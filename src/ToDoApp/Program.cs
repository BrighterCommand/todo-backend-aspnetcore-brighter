using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Serilog;
using ToDoCore.Ports.Events;
using ToDoCore.Ports.Mappers;

namespace ToDoApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>

                {
                    var connections = new Connection[]
                    {
                        new Connection<TaskCreatedEvent>(routingKey: new RoutingKey(nameof(TaskCreatedEvent)) , isAsync: true),
                        new Connection<TaskCompletedEvent>(routingKey: new RoutingKey(nameof(TaskCompletedEvent)) , isAsync: true)
                    };

                    var rmqConnection = new RmqMessagingGatewayConnection
                    {
                        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                        Exchange = new Exchange("todo.backend.exchange")
                    };

                    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnection);

                    services.AddServiceActivator(options =>
                        {
                            options.Connections = connections;
                            options.ChannelFactory = new ChannelFactory(rmqMessageConsumerFactory);
                            options.BrighterMessaging = new BrighterMessaging(new InMemoryMessageStore(), new RmqMessageProducer(rmqConnection));
                        }).AutoFromAssemblies().MapperRegistry(registry => registry.Register<TaskCreatedEvent, GenericMessageMapper<TaskCreatedEvent>>());

                    services.AddHostedService<ServiceActivatorHostedService>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog(dispose: true);
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();

        }
    }
}
