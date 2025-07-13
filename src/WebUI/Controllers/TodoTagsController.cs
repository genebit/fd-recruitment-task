using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoTags.CreateTodoTag;
using Todo_App.Application.TodoTags.SoftDeleteTodoTag;
using Todo_App.Application.TodoTags.UpdateTodoTag;

namespace Todo_App.WebUI.Controllers;

public class TodoTagsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoTagCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteTodoTagCommand(id));

        return NoContent();
    }
}
