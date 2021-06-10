
using FluentValidation;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Validators
{
    public class GeoCoordinateValidator : AbstractValidator<GeoCoordinate>
    {
        public GeoCoordinateValidator()
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180);
        }
    }
}
