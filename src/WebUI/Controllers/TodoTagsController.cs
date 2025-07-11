using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoItems.Commands.CreateTodoTag;
using Todo_App.Application.TodoTags.Commands.SoftDeleteTodoTag;

namespace Todo_App.WebUI.Controllers;

public class TodoTagsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteTodoTagCommand(id));

        return NoContent();
    }
}
