using FluentValidation;

using ToDoList.Core.Mediator.Requests.Update;

namespace ToDoList.Core.Validators.UpdateRequests
{
    class ProjectUpdateRequestValidator : AbstractValidator<ProjectUpdateRequest>
    {
        public ProjectUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 175);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
