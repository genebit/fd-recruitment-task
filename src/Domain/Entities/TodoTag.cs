namespace Todo_App.Domain.Entities;

public class TodoTag : BaseAuditableEntity
{
    public string Tag { get; set; } = null!;

    public IList<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}
