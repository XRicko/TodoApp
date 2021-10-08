
using FluentAssertions;

using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Validators.UpdateRequests;

using Xunit;

namespace Core.Tests.Validators.UpdateRequests
{
    public class ProjectUpdateRequestTests
    {
        private readonly ProjectUpdateRequestValidator validator;

        public ProjectUpdateRequestTests()
        {
            validator = new ProjectUpdateRequestValidator();
        }

        [Theory]
        [InlineData(-12, "Somtheint", 5)]
        [InlineData(12, "So", 98)]
        [InlineData(12, "Something", -42)]
        public void ShouldHaveValidationErrorWhenInvalidValues(int id, string name, int userId)
        {
            // Act
            var result = validator.Validate(new ProjectUpdateRequest(id, name, userId));

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Arrange
            int id = 12;
            string name = "Somehting";
            int userId = 1431;

            // Act
            var result = validator.Validate(new ProjectUpdateRequest(id, name, userId));

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
