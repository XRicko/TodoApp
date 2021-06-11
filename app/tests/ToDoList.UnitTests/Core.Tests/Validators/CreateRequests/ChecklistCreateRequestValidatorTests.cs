﻿
using FluentAssertions;

using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class ChecklistCreateRequestValidatorTests
    {
        private readonly ChecklistCreateRequestValidator validator;

        public ChecklistCreateRequestValidatorTests()
        {
            validator = new ChecklistCreateRequestValidator();
        }

        [Theory]
        [InlineData("", 87)]
        [InlineData("chores", 0)]
        public void ShouldHaveValidationErrorWhenInvalidValues(string name, int userId)
        {
            // Act
            var result = validator.Validate(new ChecklistCreateRequest(name, userId));

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Assert
            int userId = 5;
            string name = "name";

            // Act
            var result = validator.Validate(new ChecklistCreateRequest(name, userId));

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
