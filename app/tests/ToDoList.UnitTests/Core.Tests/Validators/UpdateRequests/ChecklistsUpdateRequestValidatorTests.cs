
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Validators.UpdateRequests;

using Xunit;

namespace Core.Tests.Validators.UpdateRequests
{
    public class ChecklistsUpdateRequestValidatorTests
    {
        private readonly ChecklistUpdateRequestValidator validator;

        public ChecklistsUpdateRequestValidatorTests()
        {
            validator = new ChecklistUpdateRequestValidator();
        }

        [Theory]
        [InlineData(1, "smth", 0)]
        [InlineData(45, "do", 87)]
        [InlineData(-8, "chores", 10)]
        public void ShouldHaveValidationErrorWhenInvalidValues(int id, string name, int userId)
        {
            // Act
            var result = validator.Validate(new ChecklistUpdateRequest(id, name, userId));

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Assert
            int id = 12;
            int userId = 5;
            string name = "name";

            // Act
            var result = validator.Validate(new ChecklistUpdateRequest(id, name, userId));

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
