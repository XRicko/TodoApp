using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using Moq;

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
            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync("")
                            .Verifiable();

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.Empty(actual.User.Claims);
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyAuthencticationStateIfTokenExpired()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";
            var expiryDate = DateTimeOffset.Now.AddMinutes(-2);

            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
                new Claim("exp", expiryDate.ToUnixTimeSeconds().ToString())
            }, "jwtAuthentication"));

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            tokenParserMock.Setup(x => x.GetClaimsPrincipal(token))
                           .Returns(claimsPrincipal)
                           .Verifiable();

            tokenParserMock.Setup(x => x.GetExpiryDate(token, claimsPrincipal))
                           .Returns(expiryDate)
                           .Verifiable();

            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.Empty(actual.User.Claims);

            tokenParserMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_ReturnsAuthencticationStateWithClaimsIfTokenValid()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";
            var expiryDate = DateTimeOffset.Now.AddMinutes(4);

            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
                new Claim("exp", expiryDate.ToUnixTimeSeconds().ToString())
            }, "jwtAuthentication"));

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            tokenParserMock.Setup(x => x.GetClaimsPrincipal(token))
                           .Returns(claimsPrincipal)
                           .Verifiable();

            tokenParserMock.Setup(x => x.GetExpiryDate(token, claimsPrincipal))
                           .Returns(expiryDate)
                           .Verifiable();
            // Act
            var actual = await authStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.NotEmpty(actual.User.Claims);

            Assert.Equal(expiryDate.ToUnixTimeSeconds().ToString(), actual.User.FindFirst("exp").Value);
            Assert.Equal(token, httpClient.DefaultRequestHeaders.Authorization.Parameter);

            tokenStorageMock.Verify();
        }

        [Fact]
        public void NotifyUserAuthentication_RaisesEventGivenToken()
        {
            // Arrange
            string token = "ejnefNWPQDMpf.FE{OFekwpqvje";
            var expiryDate = DateTimeOffset.Now.AddMinutes(4);

            bool eventRaised = false;

            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
                new Claim("exp", expiryDate.ToUnixTimeSeconds().ToString())
            }, "jwtAuthentication"));

            tokenParserMock.Setup(x => x.GetClaimsPrincipal(token))
                           .Returns(claimsPrincipal)
                           .Verifiable();

            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            // Act
            authStateProvider.NotifyUserAuthentication(token);

            // Assert
            Assert.True(eventRaised);
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
            Assert.True(eventRaised);
        }
    }
}
