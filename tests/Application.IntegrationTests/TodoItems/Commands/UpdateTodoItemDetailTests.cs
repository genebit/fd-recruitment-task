using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItem;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoTags.CreateTodoTag;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        // Create tags first
        var urgentTagId = await SendAsync(new CreateTodoTagCommand { Tag = "Urgent" });
        var workTagId = await SendAsync(new CreateTodoTagCommand { Tag = "Work" });
        var importantTagId = await SendAsync(new CreateTodoTagCommand { Tag = "Important" });

        var urgentTag = await FindAsync<TodoTag>(urgentTagId);
        var workTag = await FindAsync<TodoTag>(workTagId);
        var importantTag = await FindAsync<TodoTag>(importantTagId);

        Assert.That(urgentTag, Is.Not.Null, "Urgent tag should exist");
        Assert.That(workTag, Is.Not.Null, "Work tag should exist");
        Assert.That(importantTag, Is.Not.Null, "Important tag should exist");

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "A1",
            Priority = PriorityLevel.High,
            Tags = new TodoTag[] { urgentTag, workTag, importantTag }
        };
        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Note.Should().Be(command.Note);
        item.Priority.Should().Be(command.Priority);
        item.TodoItemTags.Should().NotBeNull();
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
