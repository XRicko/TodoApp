using System;

using FluentValidation;

using ToDoList.Core.Mediator.Requests.Update;

namespace ToDoList.Core.Validators.UpdateRequests
{
    public class TodoItemUpdateRequestValidator : AbstractValidator<TodoItemUpdateRequest>
    {
        public TodoItemUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 200);

            RuleFor(x => x.ChecklistId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.StatusId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.StartDate)
                .LessThan(DateTime.Now);

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now)
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.CategoryId)
                .GreaterThanOrEqualTo(1)
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.ImageId)
                .GreaterThanOrEqualTo(1)
                .When(x => x.ImageId.HasValue);

            RuleFor(x => x.GeoPoint)
                .SetValidator(new GeoCoordinateValidator())
                .When(x => x.GeoPoint is not null);
        }
    }
}
