using System.IO;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using ToDoList.WebApi.Validators;

using Xunit;

namespace WebApi.Tests.Validators
{
    public class FormFileValidatorTests
    {
        private readonly FormFileValidator validator;

        public FormFileValidatorTests()
        {
            validator = new FormFileValidator();
        }

        [Theory]
        [InlineData("rand.docx")]
        [InlineData("rand.exe")]
        [InlineData("rand.sql")]
        public void ShouldHaveValidationErrorWhenInvalidExtension(string fileName)
        {
            // Act
            var result = validator.Validate(new FormFile(new MemoryStream(), 0, 0, "Name", fileName));

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("rand.jpg")]
        [InlineData("rand.jpeg")]
        [InlineData("rand.png")]
        public void ShouldNotHaveValidationErrorWhenValidExtension(string fileName)
        {
            // Act
            var result = validator.Validate(new FormFile(new MemoryStream(), 0, 0, "Name", fileName));

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
