using System;

using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Validators.UpdateRequests;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Validators.UpdateRequests
{
    public class TodoItemUpdateRequestValidatorTests
    {
        private readonly TodoItemUpdateRequestValidator validator;

        public TodoItemUpdateRequestValidatorTests()
        {
            validator = new TodoItemUpdateRequestValidator();
        }

        [Theory]
        [InlineData(0, "name", 58, 2, "2001-12-24", "2069-12-24", 3, null)]
        [InlineData(1, "q", 0, 3, "2041-12-24", "2069-12-24", null, -5)]
        [InlineData(4, "smth", 12, -8, "2001-12-24", "2069-12-24", -5, 88)]
        [InlineData(40, "MOre", 2, 1, "2001-12-24", "2001-12-24", 3, 15)]
        public void ShouldHaveValidationErrorWhenInvalidValues(int id, string name, int checklistId, int statusId,
                                                               string startDate, string dueDate, int? categoryId,
                                                               int? imageId)
        {
            // Assert
            var start = DateTime.Parse(startDate);
            var due = DateTime.Parse(dueDate);

            // Act
            var result = validator.Validate(new TodoItemUpdateRequest(id, name, checklistId, statusId, start,
                                                                      due, null, categoryId, imageId));

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldHaveValidationErrorWhenInvalidGeoPoint()
        {
            // Assert
            GeoCoordinate geoCoordinate = new(270, -97);
            TodoItemUpdateRequest todoItem = new(69, "Name", 2, 1, DateTime.Now.AddDays(-2),
                                                 DateTime.Now.AddMonths(1), geoCoordinate, null, null);

            // Act
            var result = validator.Validate(todoItem);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenValidValues()
        {
            // Assert
            GeoCoordinate geoCoordinate = new(170, -87);
            TodoItemUpdateRequest todoItem = new(69, "Name", 2, 1, DateTime.Now.AddDays(-3),
                                                 DateTime.Now.AddMonths(1), geoCoordinate, null, null);

            // Act
            var result = validator.Validate(todoItem);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
