using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;
using Moq.Protected;

using TestExtensions;

using ToDoList.SharedClientLibrary;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace SharedClientLibrary.Tests.Services
{
    public class ApiInvokerTests
    {
        private readonly ApiInvoker apiInvoker;

        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;

        private readonly HttpClient httpClient;

        private readonly CategoryModel category;

        private readonly string token;
        private readonly string refreshToken;

        public ApiInvokerTests()
        {
            tokenStorageMock = new Mock<ITokenStorage>();
            tokenParserMock = new Mock<ITokenParser>();

            httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://localhost:5001/api/") };
            apiInvoker = new ApiInvoker(httpClient, tokenStorageMock.Object, tokenParserMock.Object);

            category = new CategoryModel { Id = 543, Name = "Important" };

            token = "eyhJfepcmve_FEpimfpi.ceFGNJOewipqm";
            refreshToken = "eyjhFNjpz[pxlwxnc.zegvEcverGX";
        }

        [Fact]
        public async Task GetItemAsync_ReturnsItemIfNoError()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");
            var content = new ObjectContent<CategoryModel>(category, new JsonMediaTypeFormatter());

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Get, HttpStatusCode.OK, content);

            // Act
            var actual = await apiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/Important");

            // Assert
            actual.Id.Should().Be(category.Id);
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemAsync_RefreshesTokenIfUnathorizedAndReturnsItem()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");
            var content = new ObjectContent<CategoryModel>(category, new JsonMediaTypeFormatter());

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Get, content);

            // Act
            var actual = await apiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/Important");

            // Assert
            actual.Id.Should().Be(category.Id);
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");
            var statusCode = HttpStatusCode.InternalServerError;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Get, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.GetItemAsync<CategoryModel>($"{ApiEndpoints.CategoryByName}/Important");

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, HttpStatusCode.OK);

            // Act
            await apiInvoker.PostItemAsync(ApiEndpoints.Categories, category);

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Post);

            // Act
            await apiInvoker.PostItemAsync(ApiEndpoints.Categories, category);

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var statusCode = HttpStatusCode.ServiceUnavailable;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.PostItemAsync(ApiEndpoints.Categories, category);

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostFileAsync_Executes()
        {
            // Arrange
            string fileName = "rnieo.jpg";

            var uri = new Uri("https://localhost:5001/api/Images");
            var content = new ObjectContent<string>(fileName, new JsonMediaTypeFormatter());

            byte[] bytes = new byte[123];
            new Random().NextBytes(bytes);

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, HttpStatusCode.OK, content);

            // Act
            string actual = await apiInvoker.PostFileAsync(ApiEndpoints.Images, "randsqw.jpeg", bytes);

            // Assert
            actual.Should().Be(fileName);
            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task PostFileAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Images");
            var statusCode = HttpStatusCode.ServiceUnavailable;

            byte[] bytes = new byte[123];
            new Random().NextBytes(bytes);

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, statusCode);

            // Act && Assert
            Func<Task> action = async () => await apiInvoker.PostFileAsync(ApiEndpoints.Images, "randsqw.jpeg", bytes);

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Put, HttpStatusCode.OK);

            // Act
            await apiInvoker.PutItemAsync(ApiEndpoints.Categories, category);

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Put);

            // Act
            await apiInvoker.PutItemAsync(ApiEndpoints.Categories, category);

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Put
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var statusCode = HttpStatusCode.BadRequest;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Put, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.PutItemAsync(ApiEndpoints.Categories, category);

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_ReturnsItemsIfSuccess()
        {
            // Arrange
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

            var uri = new Uri("https://localhost:5001/api/Categories");
            var content = new ObjectContent<IEnumerable<CategoryModel>>(categories, new JsonMediaTypeFormatter());

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Get, HttpStatusCode.OK, content);

            // Act
            var result = await apiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);

            // Assert
            result.Count().Should().Be(categories.Count);
            result.First().Name.Should().Be(categories.First().Name);

            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_RefreshesTokenIfUnathorizedAndReturnsItems()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

            var content = new ObjectContent<IEnumerable<CategoryModel>>(categories, new JsonMediaTypeFormatter());

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Get, content);

            // Act
            var result = await apiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);

            // Assert
            result.Count().Should().Be(categories.Count);
            result.First().Name.Should().Be(categories.First().Name);

            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var statusCode = HttpStatusCode.BadGateway;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Get, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.GetItemsAsync<CategoryModel>(ApiEndpoints.Categories);

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task AuthenticateUserAsync_Executes()
        {
            // Arrange 
            var authenticatedModel = new AuthenticatedModel { AccessToken = token, RefreshToken = refreshToken };
            var user = new UserModel { Id = 3, Name = "admin", Password = "qwerty" };

            var uri = new Uri("https://localhost:5001/api/Authentication/Login");
            var content = new ObjectContent<AuthenticatedModel>(authenticatedModel, new JsonMediaTypeFormatter());

            SetupSettingTokens();
            tokenParserMock.SetupGettingExpiryDate(authenticatedModel.AccessToken, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, HttpStatusCode.OK, content);

            // Act
            var actual = await apiInvoker.AuthenticateUserAsync(ApiEndpoints.Login, user);

            // Assert
            actual.AccessToken.Should().Be(authenticatedModel.AccessToken);
            actual.RefreshToken.Should().Be(authenticatedModel.RefreshToken);

            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(authenticatedModel.AccessToken);

            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task AuthenticateUserAsync_ThrowsException()
        {
            // Arrange
            var user = new UserModel { Name = "admin", Password = "qwerty" };

            var uri = new Uri("https://localhost:5001/api/Authentication/Login");
            var statusCode = HttpStatusCode.Unauthorized;

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.AuthenticateUserAsync(ApiEndpoints.Login, user);

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task DeleteItemAsync_Executes()
        {
            // Arrange 
            int id = 4;
            var uri = new Uri($"https://localhost:5001/api/Categories/{id}");

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Delete, HttpStatusCode.OK);

            // Act
            await apiInvoker.DeleteItemAsync($"{ApiEndpoints.Categories}/{id}");

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);
            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task DeleteItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange 
            int id = 4;
            var uri = new Uri($"https://localhost:5001/api/Categories/{id}");

            tokenStorageMock.SetupGettingToken("accessToken", token);

            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Delete);

            // Act
            await apiInvoker.DeleteItemAsync($"{ApiEndpoints.Categories}/{id}");

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Delete
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task DeleteItemAsync_ThrowsException()
        {
            // Arrange 
            int id = 4;

            var uri = new Uri($"https://localhost:5001/api/Categories/{id}");
            var statusCode = HttpStatusCode.InternalServerError;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Delete, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.DeleteItemAsync($"{ApiEndpoints.Categories}/{id}");

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task LogoutAsync_Executes()
        {
            // Arrange 
            var uri = new Uri("https://localhost:5001/api/Authentication/Logout");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRemovingTokens();

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Delete, HttpStatusCode.OK);

            // Act
            await apiInvoker.LogOutAsync();

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task LogoutAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange 
            var uri = new Uri("https://localhost:5001/api/Authentication/Logout");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            tokenStorageMock.SetupGettingToken("accessToken", token);
            SetupRemovingTokens();
            SetupRefreshingToken();

            SetupHttpCallSequenceForRefreshingToken(uri, HttpMethod.Delete);

            // Act
            await apiInvoker.LogOutAsync();

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task LogoutAsync_ThrowsException()
        {
            // Arrange 
            var uri = new Uri("https://localhost:5001/api/Authentication/Logout");
            var statusCode = HttpStatusCode.InternalServerError;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Delete, statusCode);

            // Act && Assert
            Func<Task> action = () => apiInvoker.LogOutAsync();

            (await action.Should().ThrowAsync<HttpRequestException>())
                         .And.StatusCode.Should().Be(statusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task AddAuthorizationHeaderAsync_DoesntAddHeaderIfTokenIsNull()
        {
            // Arrange
            httpClient.DefaultRequestHeaders.Authorization = null;
            tokenStorageMock.SetupGettingToken("accessToken", null);

            // Act
            await apiInvoker.AddAuthorizationHeaderAsync();

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
            tokenStorageMock.Verify();
        }


        [Fact]
        public async Task AddAuthorizationHeaderAsync_DoesntAddHeaderIfTokenIsExpired()
        {
            // Arrange
            httpClient.DefaultRequestHeaders.Authorization = null;

            tokenStorageMock.SetupGettingToken("accessToken", token);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(-2));

            // Act
            await apiInvoker.AddAuthorizationHeaderAsync();

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();

            tokenStorageMock.Verify();
            tokenParserMock.Verify();
        }

        [Fact]
        public async Task AddAuthorizationHeaderAsync_AddsHeaderIfTokenUpToDate()
        {
            // Arrange
            httpClient.DefaultRequestHeaders.Authorization = null;
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            // Act
            await apiInvoker.AddAuthorizationHeaderAsync(token);

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            tokenParserMock.Verify();
            tokenStorageMock.Verify(x => x.GetTokenAsync("accessToken"), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_RefreshesTokenAndSetsIt()
        {
            // Arrange
            SetupRefreshingToken();

            // Act
            await apiInvoker.RefreshTokenAsync();

            // Assert
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

            tokenStorageMock.Verify();
            httpMessageHandlerMock.Verify();
        }

        private void SetupHttpCallSequenceForRefreshingToken(Uri uri, HttpMethod httpMethod,
                                                             ObjectContent objectContent = null)
        {
            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                                                          ItExpr.Is<HttpRequestMessage>(r => r.Method == httpMethod
                                                                                             && r.RequestUri == uri),
                                                          ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = objectContent
                });
        }

        private void SetupRefreshingToken()
        {
            var authenticatedModel = new AuthenticatedModel { AccessToken = token, RefreshToken = refreshToken };

            var uri = new Uri("https://localhost:5001/api/Authentication/Refresh");
            var content = new ObjectContent<AuthenticatedModel>(authenticatedModel, new JsonMediaTypeFormatter());

            tokenStorageMock.SetupGettingToken("refreshToken", refreshToken);
            tokenParserMock.SetupGettingExpiryDate(token, DateTimeOffset.Now.AddMinutes(2));

            SetupSettingTokens();

            httpMessageHandlerMock.SetupHttCall(uri, HttpMethod.Post, HttpStatusCode.OK, content);
        }

        private void SetupSettingTokens()
        {
            tokenStorageMock.Setup(x => x.SetTokenAsync("accessToken", token))
                            .Verifiable();
            tokenStorageMock.Setup(x => x.SetTokenAsync("refreshToken", refreshToken))
                            .Verifiable();
        }

        private void SetupRemovingTokens()
        {
            tokenStorageMock.Setup(x => x.RemoveTokenAsync("accessToken"))
                            .Verifiable();
            tokenStorageMock.Setup(x => x.RemoveTokenAsync("refreshToken"))
                            .Verifiable();
        }
    }
}
