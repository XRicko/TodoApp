using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using TestExtensions;

using ToDoList.BlazorClient.Authentication;
using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace BlazorClient.Tests.Services
{
    public class BlazorApiInvokerTests
    {
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;

        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly HttpClient httpClient;
        private readonly AuthStateProvider authStateProvider;

        private readonly BlazorApiInvoker blazorApiInvoker;

        public BlazorApiInvokerTests()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            tokenParserMock = new Mock<ITokenParser>();

            httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://localhost:5001/api/") };

            Mock<ITokenStorage> tokenStorageMock = new();

            authStateProvider = new AuthStateProvider(httpClient, tokenParserMock.Object, tokenStorageMock.Object);
            blazorApiInvoker = new BlazorApiInvoker(httpClient, tokenStorageMock.Object, authStateProvider);
        }

        [Fact]
        public async Task AuthenticateUserAsync_RaisesEventAndReturnsAuthenticatedModel()
        {
            // Arrange
            string token = "eyhJfepcmve_FEpimfpi.ceFGNJOewipqm";

            var authenticatedModel = new AuthenticatedModel
            {
                AccessToken = token,
                RefreshToken = "eyjhFNjpz[pxlwxnc.zegvEcverGX"
            };

            var expiryDate = DateTimeOffset.Now.AddMinutes(2);
            var claimsPrincipal = ClaimsPrincipalHelpers.CreateClaimsPrincipal(expiryDate);

            var uri = new Uri("https://localhost:5001/api/Authentication/Login");
            var content = new ObjectContent<AuthenticatedModel>(authenticatedModel, new JsonMediaTypeFormatter());

            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            tokenParserMock.SetupGettingClaimsPrincipal(token, claimsPrincipal);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, HttpStatusCode.OK, content);

            // Act
            var actual = await blazorApiInvoker.AuthenticateUserAsync("Authentication/Login",
                                                                      new UserModel { Name = "admin", Password = "QWERty" });

            // Assert
            actual.AccessToken.Should().Be(authenticatedModel.AccessToken);
            actual.RefreshToken.Should().Be(authenticatedModel.RefreshToken);

            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            eventRaised.Should().BeTrue();

            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task LogoutAsync_RaisesEvent()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Authentication/Logout");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "efnepwigj.IEJFpweivnvj");

            bool eventRaised = false;
            authStateProvider.AuthenticationStateChanged += (obj) => eventRaised = true;

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Delete, HttpStatusCode.OK);

            // Act
            await blazorApiInvoker.LogoutAsync();

            // Assert
            eventRaised.Should().BeTrue();
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();

            httpMessageHandlerMock.Verify();
        }
    }
}
