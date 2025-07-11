namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public class TodosVm
{
    public IList<PriorityLevelDto> PriorityLevels { get; set; } = new List<PriorityLevelDto>();

    public IList<TodoTagDto> Tags { get; set; } = new List<TodoTagDto>();

    public IList<TodoListDto> Lists { get; set; } = new List<TodoListDto>();
}
