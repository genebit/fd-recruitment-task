using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoTags.CreateTodoTag;
using Todo_App.Application.TodoTags.SoftDeleteTodoTag;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoTags.Commands;

using static Testing;

public class DeleteTodoTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoTagId()
    {
        var command = new SoftDeleteTodoTagCommand(99);

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoTag()
    {
        var tagId = await SendAsync(new CreateTodoTagCommand
        {
            Tag = "New Tag"
        });

        await SendAsync(new SoftDeleteTodoTagCommand(tagId));

        var item = await FindAsync<TodoTag>(tagId);

        item.Should().BeNull();
    }
}
