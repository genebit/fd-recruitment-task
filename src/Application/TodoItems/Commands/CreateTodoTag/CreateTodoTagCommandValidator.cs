using FluentValidation;

namespace Todo_App.Application.TodoItems.Commands.CreateTodoTag;

public class CreateTodoTagCommandValidator : AbstractValidator<CreateTodoTagCommand>
{
    public CreateTodoTagCommandValidator()
    {
        RuleFor(v => v.Tag)
            .MaximumLength(200)
            .NotEmpty();
    }
}
