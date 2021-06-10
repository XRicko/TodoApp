
using FluentValidation;

using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.Core.Validators.CreateRequests
{
    public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
    {
        public CategoryCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 75);
        }
    }
}
