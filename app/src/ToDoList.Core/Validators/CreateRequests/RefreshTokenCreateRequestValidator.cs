
using FluentValidation;

using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.Core.Validators.CreateRequests
{
    public class RefreshTokenCreateRequestValidator : AbstractValidator<RefreshTokenCreateRequest>
    {
        public RefreshTokenCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(25, 255);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
