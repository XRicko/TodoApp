
using FluentAssertions;

using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class ProjectCreateRequestValidatorTests
    {
        private readonly ProjectCreateRequestValidator validator;

        public ProjectCreateRequestValidatorTests()
        {
            validator = new ProjectCreateRequestValidator();
        }

        [Theory]
        [InlineData("qw", 5)]
        [InlineData("Somehting", 0)]
        [InlineData("q", -5)]
        public void ShouldHaveValidationErrorWhenInvalidValues(string name, int userId)
        {
            // Act
            var result = validator.Validate(new ProjectCreateRequest(name, userId));

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Arrange
            string name = "Somehting";
            int userId = 12;

            // Act
            var result = validator.Validate(new ProjectCreateRequest(name, userId));

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
