using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Events;

namespace Todo_App.Application.TodoItems.Commands.CreateTodoTag;

public record CreateTodoTagCommand : IRequest<int>
{
    public string? Tag { get; init; }
}

public class CreateTodoTagCommandHandler : IRequestHandler<CreateTodoTagCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoTagCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Tag))
        {
            throw new ArgumentException("Tag name cannot be null or empty.", nameof(request.Tag));
        }

        var entity = new TodoTag
        {
            Tag = request.Tag
        };

        entity.AddDomainEvent(new TodoTagCreatedEvent(entity));

        _context.TodoTags.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
