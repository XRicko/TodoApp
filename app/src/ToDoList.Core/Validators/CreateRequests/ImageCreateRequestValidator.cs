
using FluentValidation;

using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.Core.Validators.CreateRequests
{
    public class ImageCreateRequestValidator : AbstractValidator<ImageCreateRequest>
    {
        public ImageCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(4, 255);

            RuleFor(x => x.Path)
                .NotEmpty()
                .Length(3, 260);
        }
    }
}
