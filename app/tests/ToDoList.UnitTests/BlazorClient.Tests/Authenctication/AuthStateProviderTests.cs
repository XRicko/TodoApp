using System;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using TestExtensions;

using ToDoList.BlazorClient.Authentication;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace BlazorClient.Tests.Authenctication
{
    public class AuthStateProviderTests
    {
        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly HttpClient httpClient;

        private readonly AuthStateProvider authStateProvider;

        public AuthStateProviderTests()
        {
            tokenStorageMock = new Mock<ITokenStorage>();
            tokenParserMock = new Mock<ITokenParser>();

            httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:5001/api/") };

            authStateProvider = new AuthStateProvider(httpClient, tokenParserMock.Object, tokenStorageMock.Object);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyAuthencticationStateIfNoToken()
        {
            // Arrange
            tokenStorageMock.SetupGettingToken("accessToken", "");

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().BeEmpty();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyAuthencticationStateIfTokenExpired()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";

            var expiryDate = DateTimeOffset.Now.AddMinutes(-2);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipal);
            tokenParserMock.SetupGettingExpiryDate(token, expiryDate, claimsPrincipal);

            tokenStorageMock.SetupGettingToken("accessToken", token);

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().BeEmpty();

            tokenParserMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsAuthenticationStateWithClaimsIfTokenValid()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";

            var expiryDate = DateTimeOffset.Now.AddMinutes(4);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            tokenStorageMock.SetupGettingToken("accessToken", token);

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipal);
            tokenParserMock.SetupGettingExpiryDate(token, expiryDate, claimsPrincipal);

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            actual.User.Claims.Should().NotBeEmpty();
            actual.User.Claims.Should().Contain(x => x.Value == expiryDate.ToUnixTimeSeconds().ToString());

            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            tokenStorageMock.Verify();
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
