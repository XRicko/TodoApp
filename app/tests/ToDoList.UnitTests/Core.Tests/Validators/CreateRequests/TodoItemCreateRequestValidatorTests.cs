using System;

using FluentAssertions;

using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Validators.CreateRequests;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Validators.CreateRequests
{
    public class TodoItemCreateRequestValidatorTests
    {
        private readonly TodoItemCreateRequestValidator validator;

        public TodoItemCreateRequestValidatorTests()
        {
            validator = new TodoItemCreateRequestValidator();
        }

        [Theory]
        [InlineData("q", 58, 2, "2069-12-24", 3, null)]
        [InlineData("name", 0, 3, "2069-12-24", null, -5)]

        public void ShouldHaveValidationErrorWhenInvalidValues(string name, int checklistId, int statusId,
                                                               string dueDate, int? categoryId, int? imageId)
        {
            // Assert
            var due = DateTime.Parse(dueDate);

            // Act
            var result = validator.Validate(new TodoItemCreateRequest(name, checklistId, statusId, due,
                                                                              null, categoryId, imageId));

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldHaveValidationErrorWhenInvalidGeoPoint()
        {
            // Assert
            GeoCoordinate geoCoordinate = new(270, -97);
            TodoItemCreateRequest todoItem = new("Name", 2, 1, DateTime.Now.AddMonths(1),
                                                 geoCoordinate, null, null);

            // Act
            var result = validator.Validate(todoItem);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Assert
            GeoCoordinate geoCoordinate = new(170, -87);
            TodoItemCreateRequest todoItem = new("Name", 2, 1, DateTime.Now.AddMonths(1),
                                                 geoCoordinate, null, null);

            // Act
            var result = validator.Validate(todoItem);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
