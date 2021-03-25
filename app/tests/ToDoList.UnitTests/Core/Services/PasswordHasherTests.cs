using System;

using ToDoList.Core.Services;

using Xunit;

namespace ToDoList.UnitTests.Core.Services
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_ReturnsHashInProperFormat()
        {
            // Arrange
            string password = "][poiuytrewq";
            int expectedIterations = 12399;

            // Act
            string hashedPassword = PasswordHasher.Hash(password, expectedIterations);
            int actualIterations = int.Parse(hashedPassword.Replace("$MYHASH$V1$", "")
                                                           .Split('$')[0]);

            // Assert
            Assert.Contains("$MYHASH$V1$", hashedPassword);
            Assert.Equal(expectedIterations, actualIterations);
        }

        [Fact]
        public void Verify_ReturnsTrueGivenProperPassword()
        {
            // Arrange
            string password = "qwerty123456";
            string hashedPassword = PasswordHasher.Hash(password);

            // Act
            bool isValid = PasswordHasher.Verify(password, hashedPassword);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Verify_ReturnsFalseGivenInvalidPassword()
        {
            // Arrange
            string password = "invalid_password";
            string hashedPassword = PasswordHasher.Hash("valid_password");

            // Act
            bool isValid = PasswordHasher.Verify(password, hashedPassword);

            // Assert
            Assert.False(isValid);
        }


        [Fact]
        public void Verify_ThrowsExceptionGivenInvalidHash()
        {
            // Arrange
            string password = "password";
            string hashedPassword = "$MyHash$invalid/hash";

            // Act && Assert
            Assert.Throws<NotSupportedException>(() => PasswordHasher.Verify(password, hashedPassword));
        }
    }
}
