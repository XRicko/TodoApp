using System;

using FluentAssertions;

using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Services
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher passwordHasher;

        public PasswordHasherTests()
        {
            passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void Hash_ReturnsHashInProperFormat()
        {
            // Arrange
            string password = "][poiuytrewq";
            int expectedIterations = 12399;

            // Act
            string hashedPassword = passwordHasher.Hash(password, expectedIterations);
            int actualIterations = int.Parse(hashedPassword.Replace("$MYHASH$V1$", "")
                                                           .Split('$')[0]);

            // Assert
            hashedPassword.Should().Contain("$MYHASH$V1$");
            actualIterations.Should().Be(expectedIterations);
        }

        [Fact]
        public void Verify_ReturnsTrueGivenProperPassword()
        {
            // Arrange
            string password = "qwerty123456";
            string hashedPassword = passwordHasher.Hash(password);

            // Act
            bool isValid = passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Verify_ReturnsFalseGivenInvalidPassword()
        {
            // Arrange
            string password = "invalid_password";
            string hashedPassword = passwordHasher.Hash("valid_password");

            // Act
            bool isValid = passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            isValid.Should().BeFalse();
        }


        [Fact]
        public void Verify_ThrowsExceptionGivenInvalidHash()
        {
            // Arrange
            string password = "password";
            string hashedPassword = "$MyHash$invalid/hash";

            // Act
            Func<bool> func = () => passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            func.Should().Throw<NotSupportedException>();
        }
    }
}
