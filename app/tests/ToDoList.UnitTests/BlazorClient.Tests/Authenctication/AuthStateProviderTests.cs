using System;
using System.Threading.Tasks;

using TestExtensions;

using ToDoList.BlazorClient.Authentication;

namespace BlazorClient.Tests.Authenctication
{
    public class AuthStateProviderTests
    {
        private readonly Mock<IApiInvoker> apiInvokerMock;

        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly AuthStateProvider authStateProvider;

        public AuthStateProviderTests()
        {
            apiInvokerMock = new Mock<IApiInvoker>();

            tokenStorageMock = new Mock<ITokenStorage>();
            tokenParserMock = new Mock<ITokenParser>();

            authStateProvider = new AuthStateProvider(apiInvokerMock.Object, tokenParserMock.Object, tokenStorageMock.Object);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyAuthencticationStateIfNoToken()
        {
            // Arrange
            string refreshToken = "enfeofwjeiWQpojpcowX.qwfno";

            tokenStorageMock.SetupGettingToken("accessToken", "");
            tokenStorageMock.SetupGettingToken("refreshToken", refreshToken);

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().BeEmpty();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyAuthencticationStateIfNoRefreshToken()
        {
            // Arrange
            string accessToken = "edjWDopjcNjsqw.Wdnjap[q";

            tokenStorageMock.SetupGettingToken("accessToken", accessToken);
            tokenStorageMock.SetupGettingToken("refreshToken", "");

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().BeEmpty();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_RefreshesTokenAndReturnsAuthenticationStateWithClaimsIfTokenExpired()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";
            string refreshToken = "enfeofwjeiWQpojpcowX.qwfno";

            string refreshedToken = "eYsiajWNogwjemg.QspojvewjQ";

            var expiredDate = DateTimeOffset.Now.AddMinutes(-2);
            var newDate = DateTimeOffset.Now.AddMinutes(5);

            var claimsPrincipalWithExpired = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiredDate);
            var newClaimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(newDate);

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipalWithExpired);
            tokenParserMock.SetupGettingClaimsPrincipal(refreshedToken, newClaimsPrincipal);

            tokenParserMock.SetupGettingExpiryDate(token, expiredDate, claimsPrincipalWithExpired);

            tokenStorageMock.SetupSequence(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .ReturnsAsync(refreshedToken);

            tokenStorageMock.SetupGettingToken("refreshToken", refreshToken);

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().NotBeEmpty();

            tokenParserMock.Verify();

            tokenStorageMock.Verify();
            tokenStorageMock.Verify(x => x.GetTokenAsync("accessToken"), Times.Exactly(2));

            apiInvokerMock.Verify(x => x.AddAuthorizationHeaderAsync(refreshedToken), Times.Once);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsAuthenticationStateWithClaimsIfTokenValid()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";
            string refreshToken = "enfeofwjeiWQpojpcowX.qwfno";

            var expiryDate = DateTimeOffset.Now.AddMinutes(5);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipal);
            tokenParserMock.SetupGettingExpiryDate(token, expiryDate, claimsPrincipal);

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenStorageMock.SetupGettingToken("refreshToken", refreshToken);

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().NotBeEmpty();

            tokenParserMock.Verify();
            tokenStorageMock.Verify();

            apiInvokerMock.Verify(x => x.AddAuthorizationHeaderAsync(token), Times.Once);
        }

        [Fact]
        public void NotifyUserAuthentication_RaisesEventGivenToken()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";

            var expiryDate = DateTimeOffset.Now.AddMinutes(4);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipal);

            // Act
            authStateProvider.NotifyUserAuthentication(token);

            // Assert
            eventRaised.Should().BeTrue();
            tokenParserMock.Verify();
        }

        [Fact]
        public void NotifyUserLogout_RaisesEvent()
        {
            // Arrange
            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            // Act
            authStateProvider.NotifyUserLogout();

            // Assert
            eventRaised.Should().BeTrue();
        }
    }
}
