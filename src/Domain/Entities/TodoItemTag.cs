namespace Todo_App.Domain.Entities;

public class TodoItemTag
{
    public int ItemId { get; set; }
    
    public TodoItem Item { get; set; } = null!;

    public int TagId { get; set; }

    public TodoTag Tag { get; set; } = null!;
}
