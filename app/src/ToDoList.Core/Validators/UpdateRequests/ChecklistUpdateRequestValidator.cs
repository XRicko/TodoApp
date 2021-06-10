
using FluentValidation;

using ToDoList.Core.Mediator.Requests.Update;

namespace ToDoList.Core.Validators.UpdateRequests
{
    public class ChecklistUpdateRequestValidator : AbstractValidator<ChecklistUpdateRequest>
    {
        public ChecklistUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 75);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
