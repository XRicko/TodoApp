using FluentValidation;

using ToDoList.Core.Mediator.Requests;

namespace ToDoList.Core.Validators
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 125);

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(6, 256);
        }
    }
}
