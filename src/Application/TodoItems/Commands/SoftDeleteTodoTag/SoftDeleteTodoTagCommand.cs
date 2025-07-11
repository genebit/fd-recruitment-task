using MediatR;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Events;

namespace Todo_App.Application.TodoTags.Commands.SoftDeleteTodoTag;

public record SoftDeleteTodoTagCommand(int Id) : IRequest;

public class SoftDeleteTodoTagCommandHandler : IRequestHandler<SoftDeleteTodoTagCommand>
{
    private readonly IApplicationDbContext _context;

    public SoftDeleteTodoTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SoftDeleteTodoTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoTags
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoTag), request.Id);
        }

        entity.IsDeleted = true;
        entity.LastModified = DateTime.UtcNow;

        entity.AddDomainEvent(new TodoTagDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
