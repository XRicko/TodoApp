using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;

using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace SharedClientLibrary.Tests.Services
{
    public class ApiInvokerTests
    {
        private readonly ApiInvoker apiInvoker;

        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;

        private readonly HttpClient httpClient;

        private readonly CategoryModel category;

        private readonly string token;
        private readonly string refreshToken;

        public ApiInvokerTests()
        {
            tokenStorageMock = new Mock<ITokenStorage>();
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://localhost:5001/api/") };
            apiInvoker = new ApiInvoker(httpClient, tokenStorageMock.Object);

            category = new CategoryModel { Id = 543, Name = "Important" };

            token = "eyhJfepcmve_FEpimfpi.ceFGNJOewipqm";
            refreshToken = "eyjhFNjpz[pxlwxnc.zegvEcverGX";
        }

        [Fact]
        public async Task GetItemAsync_ReturnsItemIfNoError()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                  ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                     && x.RequestUri == uri),
                                                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<CategoryModel>(category, new JsonMediaTypeFormatter()),
                })
                .Verifiable();

            // Act
            var actual = await apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/Important");

            // Assert
            Assert.Equal(category.Id, actual.Id);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemAsync_RefreshesTokenIfUnathorizedAndReturnsItem()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");

            SetupRefreshingToken();

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                                                          ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                             && x.RequestUri == uri),
                                                          ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<CategoryModel>(category, new JsonMediaTypeFormatter()),
                });

            // Act
            var actual = await apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/Important");

            // Assert
            Assert.Equal(category.Id, actual.Id);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

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

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                                apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/Important"));

            Assert.Equal(statusCode, exception.StatusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            // Act
            await apiInvoker.PostItemAsync("Categories", category);

            // Assert
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            SetupRefreshingToken();

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            await apiInvoker.PostItemAsync("Categories", category);

            // Assert
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
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

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => apiInvoker.PostItemAsync("Categories", category));

            Assert.Equal(statusCode, exception.StatusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Put
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                })
                .Verifiable();

            // Act
            await apiInvoker.PutItemAsync("Categories", category);

            // Assert
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            SetupRefreshingToken();

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Put
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            await apiInvoker.PutItemAsync("Categories", category);

            // Assert
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
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

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Put
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => apiInvoker.PutItemAsync("Categories", category));

            Assert.Equal(statusCode, exception.StatusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_ReturnsItemsIfSuccess()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                     && r.RequestUri == uri),
                                                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<IEnumerable<CategoryModel>>(categories, new JsonMediaTypeFormatter())
                })
                .Verifiable();

            // Act
            var result = await apiInvoker.GetItemsAsync<CategoryModel>("Categories");

            // Assert
            Assert.Equal(categories.Count, result.Count());
            Assert.Equal(categories.First().Name, result.First().Name);

            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_RefreshesTokenIfUnathorizedAndReturnsItems()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

            SetupRefreshingToken();

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                                                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                     && r.RequestUri == uri),
                                                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<IEnumerable<CategoryModel>>(categories, new JsonMediaTypeFormatter())
                });

            // Act
            var result = await apiInvoker.GetItemsAsync<CategoryModel>("Categories");

            // Assert
            Assert.Equal(categories.Count, result.Count());
            Assert.Equal(categories.First().Name, result.First().Name);

            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
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

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                 ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                    && r.RequestUri == uri),
                                                 ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => apiInvoker.GetItemsAsync<CategoryModel>("Categories"));

            Assert.Equal(statusCode, exception.StatusCode);

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

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                  ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                     && r.RequestUri == uri),
                                                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<AuthenticatedModel>(authenticatedModel, new JsonMediaTypeFormatter())
                })
                .Verifiable();

            SetupSettingTokens();

            // Act
            var actual = await apiInvoker.AuthenticateUserAsync("Authentication/Login", user);

            // Assert
            Assert.Equal(authenticatedModel.AccessToken, actual.AccessToken);
            Assert.Equal(authenticatedModel.RefreshToken, actual.RefreshToken);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task AuthenticateUserAsync_ThrowsException()
        {
            // Arrange
            var user = new UserModel { Name = "admin", Password = "qwerty" };

            var uri = new Uri("https://localhost:5001/api/User/Login");
            var statusCode = HttpStatusCode.NotFound;

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                  && r.RequestUri == uri),
                                               ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => apiInvoker.AuthenticateUserAsync("User/Login", user));

            Assert.Equal(statusCode, exception.StatusCode);

            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task DeleteItemAsync_Executes()
        {
            // Arrange 
            int id = 4;
            var uri = new Uri("https://localhost:5001/api/Categories/" + id);

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Delete
                                                                                && r.RequestUri == uri),
                                             ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            // Act
            await apiInvoker.DeleteItemAsync("Categories/", id);

            // Assert
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);
            httpMessageHandlerMock.Verify();
        }

        [Fact]
        public async Task DeleteItemAsync_ThrowsException()
        {
            // Arrange 
            int id = 4;

            var uri = new Uri("https://localhost:5001/api/Categories/" + id);
            var statusCode = HttpStatusCode.InternalServerError;

            tokenStorageMock.Setup(x => x.GetTokenAsync("accessToken"))
                            .ReturnsAsync(token)
                            .Verifiable();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Delete
                                                                                && r.RequestUri == uri),
                                             ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => apiInvoker.DeleteItemAsync("Categories/", id));
            Assert.Equal(statusCode, exception.StatusCode);

            httpMessageHandlerMock.Verify();
            tokenStorageMock.Verify();
        }

        private void SetupRefreshingToken()
        {
            var uri = new Uri("https://localhost:5001/api/Authentication/Refresh");
            var authenticatedModel = new AuthenticatedModel { AccessToken = token, RefreshToken = refreshToken };

            tokenStorageMock.Setup(x => x.GetTokenAsync("refreshToken"))
                            .ReturnsAsync(refreshToken)
                            .Verifiable();

            SetupSettingTokens();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                          ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == uri
                                                                                             && x.Method == HttpMethod.Post),
                                                          ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<AuthenticatedModel>(authenticatedModel, new JsonMediaTypeFormatter())
                })
                .Verifiable();
        }

        private void SetupSettingTokens()
        {
            tokenStorageMock.Setup(x => x.SetTokenAsync("accessToken", token))
                            .Verifiable();
            tokenStorageMock.Setup(x => x.SetTokenAsync("refreshToken", refreshToken))
                            .Verifiable();
        }
    }
}
