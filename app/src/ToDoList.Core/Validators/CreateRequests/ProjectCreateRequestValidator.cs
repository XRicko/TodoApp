using FluentValidation;

using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.Core.Validators.CreateRequests
{
    class ProjectCreateRequestValidator : AbstractValidator<ProjectCreateRequest>
    {
        public ProjectCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 175);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
