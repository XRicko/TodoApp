
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Validators;

using Xunit;

namespace Core.Tests.Validators
{
    public class UserRequestValidatorTests
    {
        private readonly UserRequestValidator validator;

        public UserRequestValidatorTests()
        {
            validator = new UserRequestValidator();
        }

        [Theory]
        [InlineData("qw", "seef")]
        [InlineData("", "")]
        public void ShouldHaveValidationErrorWhenParametersTooShort(string name, string password)
        {
            // Act
            var result = validator.Validate(new UserRequest(name, password));

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenParametersAcceptableLenght()
        {
            // Assert
            string name = "admin";
            string password = "qwerty";

            // Act
            var result = validator.Validate(new UserRequest(name, password));

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
