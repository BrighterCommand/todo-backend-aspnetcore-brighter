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

namespace ToDoApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>

                {
                    var subscriptions = new RmqSubscription[]
                    {
                        new RmqSubscription<TaskCreatedEvent>(routingKey: new RoutingKey(nameof(TaskCreatedEvent)), runAsync:true),
                        new RmqSubscription<TaskCompletedEvent>(routingKey: new RoutingKey(nameof(TaskCompletedEvent)), runAsync: true)
                    };

                    var rmqConnection = new RmqMessagingGatewayConnection
                    {
                        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                        Exchange = new Exchange("todo.backend.exchange")
                    };

                    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnection);

                    services.AddServiceActivator(options =>
                        {
                            options.Subscriptions = subscriptions;
                            options.ChannelFactory = new ChannelFactory(rmqMessageConsumerFactory);
                        })
                        .UseInMemoryOutbox()
                        .AutoFromAssemblies();

                    services.AddHostedService<ServiceActivatorHostedService>();
                }).ConfigureLogging(builder => 
                    builder.AddConsole())
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();

        }
    }
}
