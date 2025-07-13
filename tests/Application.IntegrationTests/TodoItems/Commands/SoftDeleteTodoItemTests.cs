using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Commands.SoftDeleteTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class SoftDeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new SoftDeleteTodoItemCommand(99);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldSoftDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Item to Soft Delete"
        });

        await SendAsync(new SoftDeleteTodoItemCommand(itemId));

        var deletedItem = await FindAsyncIgnoringFilters<TodoItem>(itemId);
        deletedItem.Should().NotBeNull();
        deletedItem!.IsDeleted.Should().BeTrue();
        deletedItem!.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Test]
    public async Task ShouldNotReturnSoftDeletedItemInNormalQueries()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Item to Hide"
        });

        await SendAsync(new SoftDeleteTodoItemCommand(itemId));

        var allLists = await SendAsync(new GetTodosQuery());
        allLists.Lists.Any(l => l.Items.Any(i => i.Id == itemId)).Should().BeFalse();
    }

    [Test]
    public async Task ShouldThrowExceptionWhenDeletingAlreadyDeletedItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Item to Delete Twice"
        });

        await SendAsync(new SoftDeleteTodoItemCommand(itemId));

        await FluentActions.Invoking(() => SendAsync(new SoftDeleteTodoItemCommand(listId)))
            .Should().ThrowAsync<NotFoundException>();
    }
}
