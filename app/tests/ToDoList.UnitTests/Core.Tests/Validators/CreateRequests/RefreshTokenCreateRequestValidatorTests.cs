
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class RefreshTokenCreateRequestValidatorTests
    {
        private readonly RefreshTokenCreateRequestValidator validator;

        public RefreshTokenCreateRequestValidatorTests()
        {
            validator = new RefreshTokenCreateRequestValidator();
        }

        [Theory]
        [InlineData("wqeiojrf", 5)]
        [InlineData("qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjkl", 0)]
        public void ShouldHaveValidationErrorWhenInvalidValues(string name, int userId)
        {
            // Act
            var result = validator.Validate(new RefreshTokenCreateRequest(name, userId));

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Assert
            string token = "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjkl";
            int userId = 85;

            // Act
            var result = validator.Validate(new RefreshTokenCreateRequest(token, userId));

            // Assert
            Assert.True(result.IsValid);
        }

    }
}
