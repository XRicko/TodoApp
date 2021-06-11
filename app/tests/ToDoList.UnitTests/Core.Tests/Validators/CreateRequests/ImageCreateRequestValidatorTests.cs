
using FluentAssertions;

using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class ImageCreateRequestValidatorTests
    {
        private readonly ImageCreateRequestValidator validator;

        public ImageCreateRequestValidatorTests()
        {
            validator = new ImageCreateRequestValidator();
        }

        [Fact]
        public void ShouldHaveValidationErrorWhenValuesShort()
        {
            // Arrange
            string name = "qwe";
            string path = "qw";

            ImageCreateRequest categoryCreateRequest = new(name, path);

            // Act
            var result = validator.Validate(categoryCreateRequest);

            // Assert
            result.IsValid.Should().BeFalse();
        }


        [Fact]
        public void ShouldNotHaveValidationErrorWhenValuesNotShort()
        {
            // Arrange
            string name = "qwerty";
            string path = "c:\\programs";

            ImageCreateRequest categoryCreateRequest = new(name, path);

            // Act
            var result = validator.Validate(categoryCreateRequest);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
