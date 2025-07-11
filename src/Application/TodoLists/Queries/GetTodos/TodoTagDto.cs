using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public class TodoTagDto : IMapFrom<TodoTag>
{
    public int Id { get; set; }
    public string Tag { get; set; } = null!;
}
