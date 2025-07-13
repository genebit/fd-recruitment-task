using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Commands.SoftDeleteTodoList;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class SoftDeleteTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new SoftDeleteTodoListCommand(99);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldSoftDeleteTodoList()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "List to Soft Delete"
        });

        await SendAsync(new SoftDeleteTodoListCommand(listId));

        var deletedList = await FindAsyncIgnoringFilters<TodoList>(listId);
        deletedList.Should().NotBeNull();
        deletedList!.IsDeleted.Should().BeTrue();
        deletedList!.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Test]
    public async Task ShouldNotReturnSoftDeletedListInNormalQueries()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "List to Hide"
        });

        await SendAsync(new SoftDeleteTodoListCommand(listId));

        var allLists = await SendAsync(new GetTodosQuery());
        allLists.Lists.Any(l => l.Id == listId).Should().BeFalse();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeletingAlreadyDeletedList()
    {
        // Arrange
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "List to Delete Twice"
        });

        await SendAsync(new SoftDeleteTodoListCommand(listId));

        // Act & Assert
        await FluentActions.Invoking(() => SendAsync(new SoftDeleteTodoListCommand(listId)))
            .Should().ThrowAsync<NotFoundException>();
    }
}
