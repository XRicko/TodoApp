
using FluentValidation;

using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.Core.Validators.CreateRequests
{
    public class ChecklistCreateRequestValidator : AbstractValidator<ChecklistCreateRequest>
    {
        public ChecklistCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 75);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
