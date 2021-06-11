
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class CategoryCreateRequestValidatorTests
    {
        private readonly CategoryCreateRequestValidator validator;

        public CategoryCreateRequestValidatorTests()
        {
            validator = new CategoryCreateRequestValidator();
        }

        [Fact]
        public void ShouldHaveValidationErrorWhenNameIsShort()
        {
            // Arrange
            string name = "q";
            CategoryCreateRequest categoryCreateRequest = new(name);

            // Act
            var result = validator.Validate(categoryCreateRequest);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenNameIsNotShort()
        {
            // Arrange
            string name = "qwerty";
            CategoryCreateRequest categoryCreateRequest = new(name);

            // Act
            var result = validator.Validate(categoryCreateRequest);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
