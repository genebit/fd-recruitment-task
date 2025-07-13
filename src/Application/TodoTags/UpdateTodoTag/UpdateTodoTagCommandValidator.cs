using FluentValidation;

namespace Todo_App.Application.TodoTags.UpdateTodoTag;

public class UpdateTodoTagCommandValidator : AbstractValidator<UpdateTodoTagCommand>
{
    public UpdateTodoTagCommandValidator()
    {
        RuleFor(v => v.Tag)
            .MaximumLength(200)
            .NotEmpty();
    }
}
