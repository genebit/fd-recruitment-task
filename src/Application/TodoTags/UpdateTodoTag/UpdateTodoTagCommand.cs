using MediatR;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoTags.UpdateTodoTag;

public record UpdateTodoTagCommand : IRequest
{
    public int Id { get; init; }

    public string? Tag { get; init; }
}

public class UpdateTodoTagCommandHandler : IRequestHandler<UpdateTodoTagCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTodoTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoTags
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoTag), request.Id);
        }

        if (string.IsNullOrWhiteSpace(request.Tag))
        {
            throw new ArgumentException("Tag name cannot be null or empty.", nameof(request.Tag));
        }

        entity.Tag = request.Tag;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
