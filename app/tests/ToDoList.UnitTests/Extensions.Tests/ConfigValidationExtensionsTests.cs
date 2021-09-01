using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using ToDoList.Extensions;

using Xunit;

namespace Extensions.Tests
{
    public class ConfigValidationExtensionsTests
    {
        [Fact]
        public void GetValid_ReturnsValidInstance()
        {
            // Assert
            string key = "JsokfXlqSijNeuAnyYudn";

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Key", key } })
                .Build();

            // Act 
            var actual = configuration.GetValid<SomeOptions>();

            // Assert
            actual.Key.Should().Be(key);
        }

        [Fact]
        public void GetValid_ThrowsErrorIfKeyIsShort()
        {
            // Assert
            string key = "Jsok";

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Key", key } })
                .Build();

            // Act && Assert
            Action action = () => configuration.GetValid<SomeOptions>();
            action.Should()
                  .Throw<ValidationException>()
                  .WithMessage("The field Key must be a string or array type with a minimum length of '12'.");
        }
    }

    internal class SomeOptions
    {
        [Required]
        [MinLength(12)]
        public string Key { get; set; }
    }
}
