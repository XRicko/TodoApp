
using FluentAssertions;

using ToDoList.Core.Validators;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Validators
{
    public class GeoCoordinateValidatorTests
    {
        private readonly GeoCoordinateValidator validator;

        public GeoCoordinateValidatorTests()
        {
            validator = new GeoCoordinateValidator();
        }

        [Theory]
        [InlineData(91, 181)]
        [InlineData(-91, -181)]
        [InlineData(948, 420)]
        public void ShouldHaveValidationErrorWhenLongitudeOutsideRange(double latitude, double longitude)
        {
            // Act
            var result = validator.Validate(new GeoCoordinate(longitude, latitude));

            // Assert
            result.IsValid.Should().BeFalse();
        }


        [Theory]
        [InlineData(69, 42)]
        [InlineData(90, 180)]
        public void ShouldNotHaveValidationErrorsWhenLongitudeInsideRange(double latitude, double longitude)
        {
            // Act
            var result = validator.Validate(new GeoCoordinate(longitude, latitude));

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
