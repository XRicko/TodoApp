using System;
using System.Threading.Tasks;

using TestExtensions;

using ToDoList.BlazorClient.Authentication;
using ToDoList.BlazorClient.Services;

namespace BlazorClient.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private Mock<IApiInvoker> apiInvokerMock;
        private Mock<ITokenParser> tokenParserMock;

        private AuthStateProvider authStateProvider;

        private AuthenticationService authenticationService;

        public AuthenticationServiceTests()
        {
            apiInvokerMock = new Mock<IApiInvoker>();
            tokenParserMock = new Mock<ITokenParser>();

            Mock<ITokenStorage> tokenStorageMock = new();

            authStateProvider = new AuthStateProvider(apiInvokerMock.Object, tokenParserMock.Object, tokenStorageMock.Object);

            authenticationService = new AuthenticationService(apiInvokerMock.Object, authStateProvider);
        }

        [Fact]
        public async Task AuthenticateAsync_AuthenticatesUserAndRaisesEvent()
        {
            // Arrange
            var user = new UserModel { Name = "admin", Password = "qwerty" };
            var authenticatedModel = new AuthenticatedModel { AccessToken = "eihfIifqm", RefreshToken = "ejuyQenfnjWfn" };

            var expiryDate = DateTimeOffset.Now.AddMinutes(2);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            apiInvokerMock.Setup(x => x.AuthenticateUserAsync(ApiEndpoints.Register, user))
                          .ReturnsAsync(authenticatedModel)
                          .Verifiable();

            tokenParserMock.SetupGettingClaimsPrincipal(authenticatedModel.AccessToken, claimsPrincipal);

            // Action
            await authenticationService.AuthenticateAsync(ApiEndpoints.Register, user);

            // Assert
            eventRaised.Should().BeTrue();

            apiInvokerMock.Verify();
            tokenParserMock.Verify();
        }

        [Fact]
        public async Task LogOutAsync_LogsOutAndRaisesEvent()
        {
            // Arrange
            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            // Act
            await authenticationService.LogOutAsync();

            // Assert
            eventRaised.Should().BeTrue();
            apiInvokerMock.Verify(x => x.LogOutAsync(), Times.Once);
        }
    }
}
