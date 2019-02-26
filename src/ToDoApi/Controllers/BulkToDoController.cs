using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using ToDoCore.Ports.Commands;
using ToDoCore.ViewModels;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    public class BulkToDoController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public BulkToDoController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpPost]
        public IActionResult Post([FromBody]BulkAddToDoRequest request)
        {
            var bulkAddCommand = new BulkAddToDoCommand(request.ItemsToAdd);

            _commandProcessor.Post(bulkAddCommand);

            //TODO: Should have a monitoring endpoint, and share a job table location for tracking progress
            return Accepted();
        }

    }
}