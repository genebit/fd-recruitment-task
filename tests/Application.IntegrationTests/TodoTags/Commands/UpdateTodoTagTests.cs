using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoTags.CreateTodoTag;
using Todo_App.Application.TodoTags.UpdateTodoTag;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoTags.Commands;

using static Testing;

public class UpdateTodoTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoTagId()
    {
        var command = new UpdateTodoTagCommand { Id = 99, Tag = "New Tag" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoTag()
    {
        var userId = await RunAsDefaultUserAsync();

        var tagId = await SendAsync(new CreateTodoTagCommand
        {
            Tag = "New Tag"
        });

        var command = new UpdateTodoTagCommand
        {
            Id = tagId,
            Tag = "Updated Item Title"
        };

        await SendAsync(command);

        var item = await FindAsync<TodoTag>(tagId);

        item.Should().NotBeNull();
        item!.Tag.Should().Be(command.Tag);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
