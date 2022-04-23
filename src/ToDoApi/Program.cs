using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Darker;
using Paramore.Darker.AspNetCore;
using Paramore.Darker.Policies;
using Paramore.Darker.QueryLogging;
using Polly;
using Polly.Registry;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Events;
using ToDoCore.Ports.Queries;
using ToDoCore.ViewModels;
using Constants = Paramore.Darker.Policies.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseNpgsql("Host=localhost;Database=ToDoDB;Username=postgres;Password=password"));

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

builder.Services.AddBrighter(options =>
    {
        options.PolicyRegistry = brighterPolicy;
        //EFCore by default registers Context as scoped, which forces the CommandProcessorLifetime to also be scoped
        options.CommandProcessorLifetime = ServiceLifetime.Scoped;
        //options.HandlerLifetime = ServiceLifetime.Scoped;
        //options.MapperLifetime = ServiceLifetime.Scoped;

    }).UseExternalBus(new RmqProducerRegistryFactory(
        new RmqMessagingGatewayConnection
        {
            AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
            Exchange = new Exchange("todo.backend.exchange")
        },
        new[]
        {
            new RmqPublication
            {
                Topic = new RoutingKey(nameof(TaskCreatedEvent)),
                MaxOutStandingMessages = 5,
                MaxOutStandingCheckIntervalMilliSeconds = 500,
                WaitForConfirmsTimeOutInMilliseconds = 1000,
                MakeChannels = OnMissingChannel.Create
            }
        }
    ).Create())
    .UseInMemoryOutbox()
    .AutoFromAssemblies();

builder.Services.AddDarker(options =>
    {
        //EFCore by default registers Context as scoped, which forces the QueryProcessorLifetime to also be scoped
        options.QueryProcessorLifetime = ServiceLifetime.Scoped;
    })
    .AddHandlersFromAssemblies(typeof(ToDoByIdQuery).Assembly)
    .AddPolicies(brighterPolicy)
    .AddJsonQueryLogging();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowAll");


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ToDoContext>().Database.EnsureCreated();
}

app.MapGet("/todo", async (HttpContext httpContext, IQueryProcessor queryProcessor, LinkGenerator linker) =>
    {
        var toDos = await queryProcessor.ExecuteAsync(new ToDoQueryAll(1, 10));

        foreach (var toDoItem in toDos.ToDoItems) toDoItem.Url = linker.GetUriByName(httpContext, "GetToDo", new { toDoItem.Id });

        return toDos.ToDoItems;
    })
    .WithName("GetToDos");

app.MapPost("/todo",
    async (AddToDoRequest request, IQueryProcessor queryProcessor, IAmACommandProcessor commandProcessor,
        LinkGenerator linker, HttpContext httpContext) =>
    {
        var addToDoCommand = new AddToDoCommand(request.Title, request.Completed, request.Order);

        await commandProcessor.SendAsync(addToDoCommand);

        var addedToDo = await queryProcessor.ExecuteAsync(new ToDoByIdQuery(addToDoCommand.ToDoItemId));
        addedToDo.Url = linker.GetUriByName(httpContext, "GetToDo", new {addedToDo.Id});

        return Results.CreatedAtRoute("GetTodo", new {id = addedToDo.Id}, addedToDo);
    })
    .WithName("PostToDo")
    .Produces<ToDoByIdQuery.Result>();


app.MapGet("/todo/{id:int}", async (int id, IQueryProcessor queryProcessor, LinkGenerator linker, HttpContext httpContext) =>
    {
        if (await queryProcessor.ExecuteAsync(new ToDoByIdQuery(id)) is { } todo)
        {
            todo.Url = linker.GetUriByName(httpContext, "GetToDo", new { todo.Id});
            return Results.Ok(todo);
        }

        return Results.NotFound();
    }).WithName("GetToDo")
    .Produces<ToDoByIdQuery.Result>()
    .Produces(StatusCodes.Status404NotFound);


app.MapDelete("/todo/{id:int}", async (int id, IAmACommandProcessor commandProcessor) =>
{
    var deleteToDoCommand = new DeleteToDoByIdCommand(id);

    await commandProcessor.SendAsync(deleteToDoCommand);

    return Results.Ok();
})
    .WithName("DeleteToDo");


app.MapDelete("/todo", async (IAmACommandProcessor commandProcessor) =>
{
    var deleteAllToDosCommand = new DeleteAllToDosCommand();

    await commandProcessor.SendAsync(deleteAllToDosCommand);

    return Results.Ok();
})
    .WithName("DeleteAllToDos");


app.MapMethods("/todo/{id:int}", new[] {"PATCH"}, async (int id, AddToDoRequest request, IQueryProcessor queryProcessor,
    IAmACommandProcessor commandProcessor, LinkGenerator linker, HttpContext httpContext) =>
{
    var updatedCommand = new UpdateToDoCommand(id, request.Title, request.Completed, request.Order);
    await commandProcessor.SendAsync(updatedCommand);

    var addedToDo = await queryProcessor.ExecuteAsync(new ToDoByIdQuery(id));
    addedToDo.Url = linker.GetUriByName(httpContext, "GetToDo", new { addedToDo.Id});

    return Results.Ok(addedToDo);
})
    .WithName("PatchToDo")
    .Produces<ToDoByIdQuery.Result>(); ;

app.Run();