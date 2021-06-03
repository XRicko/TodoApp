using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Moq;

using TestExtensions;

using ToDoList.MvcClient.Services;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace MvcClient.Tests.Services
{
    public class CookieTokenStorageTests
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly CookieTokenStorage cookieTokenStorage;

        private readonly string key;
        private readonly string token;

        public CookieTokenStorageTests()
        {
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            tokenParserMock = new Mock<ITokenParser>();

            cookieTokenStorage = new CookieTokenStorage(httpContextAccessorMock.Object, tokenParserMock.Object);

            key = "accessKey";
            token = "ejnfjeIEnipwfwef.EWPFiJMPxcj";
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsTokenGivenKey()
        {
            // Arrange
            httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Cookies[key])
                                   .Returns(token)
                                   .Verifiable();

            // Act
            string actual = await cookieTokenStorage.GetTokenAsync(key);

            // Assert
            Assert.Equal(token, actual);
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task SetTokenAsync_SetsTokenGivenKeyAndValue()
        {
            // Arrange
            DateTimeOffset expiryDate = new(2021, 05, 10, 13, 15, 59, TimeSpan.Zero);

            httpContextAccessorMock.Setup(x => x.HttpContext.Response.Cookies.Append(key, token,
                                                                                     It.Is<CookieOptions>(c => c.Expires == expiryDate)))
                                   .Verifiable();

            tokenParserMock.SetupGettingExpiryDate(token, expiryDate);

            // Act
            await cookieTokenStorage.SetTokenAsync(key, token);

            // Assert
            httpContextAccessorMock.Verify();
            tokenParserMock.Verify();
        }

        [Fact]
        public async Task RemoveTokenAsync()
        {
            // Arrange
            httpContextAccessorMock.Setup(x => x.HttpContext.Response.Cookies.Delete(key))
                                   .Verifiable();

            // Act
            await cookieTokenStorage.RemoveTokenAsync(key);

            // Assert
            httpContextAccessorMock.Verify();
        }
    }
}
