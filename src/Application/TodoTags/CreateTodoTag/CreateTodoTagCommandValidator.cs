using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoTags.CreateTodoTag;

public class CreateTodoTagCommandValidator : AbstractValidator<CreateTodoTagCommand>
{
    public CreateTodoTagCommandValidator(IApplicationDbContext context)
    {
        RuleFor(v => v.Tag)
            .MaximumLength(200)
            .NotEmpty()
            .MustAsync(async (tag, cancellationToken) =>
            {
                return !await context.TodoTags.AnyAsync(t => t.Tag == tag, cancellationToken);
            }).WithMessage("A tag with the same name already exists.");
    }
}
