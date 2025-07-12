namespace Todo_App.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value == true && _done == false)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public int ListId { get; set; }

    public TodoList List { get; set; } = null!;

    public IList<TodoItemTag> TodoItemTags { get; set; } = new List<TodoItemTag>();
}
