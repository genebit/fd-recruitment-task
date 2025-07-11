namespace Todo_App.Domain.Events;

public class TodoTagCreatedEvent : BaseEvent
{
    public TodoTagCreatedEvent(TodoTag item)
    {
        Item = item;
    }

    public TodoTag Item { get; }
}
