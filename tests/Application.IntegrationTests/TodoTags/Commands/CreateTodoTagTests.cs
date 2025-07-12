using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoTags.CreateTodoTag;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoTags.Commands;

using static Testing;

public class CreateTodoTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoTagCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoTag()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTodoTagCommand
        {
            Tag = "Tasks"
        };

        var tagId = await SendAsync(command);

        var item = await FindAsync<TodoTag>(tagId);

        item.Should().NotBeNull();
        item!.Tag.Should().Be(command.Tag);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
