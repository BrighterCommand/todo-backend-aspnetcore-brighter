using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Darker.AspNetCore;
using Paramore.Darker.Policies;
using Paramore.Darker.QueryLogging;
using Polly;
using Polly.Registry;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;
using Constants = Paramore.Darker.Policies.Constants;

namespace ToDoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ToDoContext>(options =>
                options.UseNpgsql("Host=localhost;Database=ToDoDB;Username=postgres;Password=password"));

            var rmqMessagingGatewayConnection = new RmqMessagingGatewayConnection
            {
                Name = nameof(ToDoApi),
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                Exchange = new Exchange("todo.backend.exchange")
            };

            var asyncRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(150)
            });
            var asyncCircuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(10, new TimeSpan(5000));

            var brighterPolicy = new PolicyRegistry
            {
                {CommandProcessor.CIRCUITBREAKER, Policy.Handle<Exception>().CircuitBreaker(10, new TimeSpan(5000))},
                {CommandProcessor.CIRCUITBREAKERASYNC, asyncCircuitBreakerPolicy},
                {
                    CommandProcessor.RETRYPOLICY,
                    Policy.Handle<Exception>().WaitAndRetry(new[]
                    {
                        TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100),
                        TimeSpan.FromMilliseconds(150)
                    })
                },
                {CommandProcessor.RETRYPOLICYASYNC, asyncRetryPolicy},
                {Constants.RetryPolicyName, asyncRetryPolicy},
                {Constants.CircuitBreakerPolicyName, asyncCircuitBreakerPolicy}
            };


            services.AddBrighter(options =>
                {
                    options.BrighterMessaging = new BrighterMessaging(new InMemoryMessageStore(), new RmqMessageProducer(rmqMessagingGatewayConnection));
                    options.PolicyRegistry = brighterPolicy;
                    //EFCore by default registers Context as scoped, which forces the CommandProcessorLifetime to also be scoped
                    options.CommandProcessorLifetime = ServiceLifetime.Scoped;
                }).AutoFromAssemblies();

            services.AddDarker(options =>
                {
                    //EFCore by default registers Context as scoped, which forces the QueryProcessorLifetime to also be scoped
                    options.QueryProcessorLifetime = ServiceLifetime.Scoped;
                })
                .AddHandlersFromAssemblies(typeof(ToDoByIdQuery).Assembly)
                .AddPolicies(brighterPolicy)
                .AddJsonQueryLogging();

            services.AddMvc();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, ToDoContext context)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors("AllowAll");
            app.UseMvc();

            context.Database.EnsureCreated();
        }
    }
}