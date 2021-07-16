using System;
using System.Security.Claims;

using Moq;

using ToDoList.SharedClientLibrary.Services;

namespace TestExtensions
{
    public static class TokenParserMockExtensions
    {
        public static void SetupGettingClaimsPrincipal(this Mock<ITokenParser> tokenParserMock,
                                                       string token, ClaimsPrincipal claimsPrincipal)
        {
            _ = tokenParserMock ?? throw new ArgumentNullException(nameof(tokenParserMock));

            tokenParserMock.Setup(x => x.GetClaimsPrincipal(token))
                           .Returns(claimsPrincipal)
                           .Verifiable();
        }

        public static void SetupGettingExpiryDate(this Mock<ITokenParser> tokenParserMock,
                                                  string token, DateTimeOffset expiryDate,
                                                  ClaimsPrincipal claimsPrincipal = null)
        {
            _ = tokenParserMock ?? throw new ArgumentNullException(nameof(tokenParserMock));

            tokenParserMock.Setup(x => x.GetExpiryDate(token, claimsPrincipal))
                           .Returns(expiryDate)
                           .Verifiable();
        }
    }
}
