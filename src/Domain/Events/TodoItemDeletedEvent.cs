namespace Todo_App.Domain.Events;

public class TodoTagDeletedEvent : BaseEvent
{
    public TodoTagDeletedEvent(TodoTag item)
    {
        Item = item;
    }

    public TodoTag Item { get; }
}
