using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Moq;
using Moq.Protected;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services.Api;

using Xunit;

namespace MvcClient.Tests.Services.Api
{
    public class ApiInvokerTests
    {
        private readonly ApiInvoker apiInvoker;

        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
        private readonly HttpClient httpClient;

        private readonly CategoryModel category;

        private readonly string token;
        private readonly string refreshToken;

        public ApiInvokerTests()
        {
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://localhost:5001/api/") };
            apiInvoker = new ApiInvoker(httpClient, httpContextAccessorMock.Object);

            category = new CategoryModel { Id = 543, Name = "Important" };

            token = "eyhJfepcmve_FEpimfpi.ceFGNJOewipqm";
            refreshToken = "eyjhFNjpz[pxlwxnc.zegvEcverGX";

            httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Cookies["Token"])
                                   .Returns(token)
                                   .Verifiable();
        }

        [Fact]
        public async Task GetItemAsync_ReturnsItemIfSuccess()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");

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
            Assert.Equal(category, actual);
            Assert.Equal(category.Id, actual.Id);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task GetItemAsync_RefreshesTokenIfUnathorizedAndReturnsItem()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");

            SetupRefreshToken();

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
            Assert.Equal(category, actual);
            Assert.Equal(category.Id, actual.Id);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task GetItemAsync_ThrowsExceptionIfNotSuccess()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories/GetByName/Important");

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                                        apiInvoker.GetItemAsync<CategoryModel>("Categories/GetByName/Important"));

            Assert.Equal("Internal Server Error", exception.Message);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

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
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            SetupRefreshToken();

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

            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PostItemAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => apiInvoker.PostItemAsync("Categories", category));

            Assert.Equal("Bad Request", exception.Message);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_Executes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

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
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_RefreshesTokenIfUnathorizedAndExecutes()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            SetupRefreshToken();

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

            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task PutItemAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                     ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Put
                                                                                        && r.RequestUri == uri),
                                                     ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => apiInvoker.PutItemAsync("Categories", category));

            Assert.Equal("Bad Request", exception.Message);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_ReturnsItemsIfSuccess()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

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
            Assert.Equal(categories, result);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_RefreshesTokenIfUnathorizedAndReturnsItems()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");
            var categories = new List<CategoryModel> { category, new CategoryModel { Id = 123, Name = "Unimportant" } };

            SetupRefreshToken();

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
            Assert.Equal(categories, result);
            Assert.Equal(httpClient.DefaultRequestHeaders.Authorization.Parameter, token);

            httpMessageHandlerMock.Verify();
            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2),
                                                      ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get
                                                                                         && x.RequestUri == uri),
                                                      ItExpr.IsAny<CancellationToken>());

            httpContextAccessorMock.Verify();
        }

        [Fact]
        public async Task GetItemsAsync_ThrowsException()
        {
            // Arrange
            var uri = new Uri("https://localhost:5001/api/Categories");

            httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                 ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                                                    && r.RequestUri == uri),
                                                 ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway,
            })
            .Verifiable();

            var cookiesMock = new Mock<IRequestCookieCollection>();

            httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Cookies)
                                   .Returns(cookiesMock.Object)
                                   .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => apiInvoker.GetItemsAsync<CategoryModel>("Categories"));

            Assert.Equal("Bad Gateway", exception.Message);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify();
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
            await apiInvoker.AuthenticateUserAsync("Authentication/Login", user);

            // Assert
            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify(x => x.HttpContext.Response.Cookies.Append(It.Is<string>(s => s == "RefreshToken" || s == "Token"),
                                                                                      It.Is<string>(s => s == token || s == refreshToken),
                                                                                      It.IsAny<CookieOptions>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AuthenticateUserAsync_ThrowsException()
        {
            // Arrange
            var user = new UserModel { Name = "admin", Password = "qwerty" };
            var uri = new Uri("https://localhost:5001/api/User/Login");

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                               ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post
                                                                                  && r.RequestUri == uri),
                                               ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                })
                .Verifiable();

            // Act && Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => apiInvoker.AuthenticateUserAsync("User/Login", user));

            Assert.Equal("Not Found", exception.Message);

            httpMessageHandlerMock.Verify();
            httpContextAccessorMock.Verify(x => x.HttpContext.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(),
                                                                                      It.IsAny<CookieOptions>()), Times.Never);
        }

        [Fact]
        public async Task DeleteItemAsync_Executes()
        {
            // Arrange 
            int id = 4;
            var uri = new Uri("https://localhost:5001/api/Categories/" + id);

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

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                             ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Delete
                                                                                && r.RequestUri == uri),
                                             ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.ServiceUnavailable
                })
                .Verifiable();


            // Act && Assert
            await Assert.ThrowsAsync<Exception>(() => apiInvoker.DeleteItemAsync("Categories/", id));
        }

        private void SetupRefreshToken()
        {
            var authenticatedModel = new AuthenticatedModel { AccessToken = token, RefreshToken = refreshToken };

            httpContextAccessorMock.SetupGet(x => x.HttpContext.Request.Cookies["RefreshToken"])
                                   .Returns(refreshToken)
                                   .Verifiable();

            SetupSettingTokens();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                          ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post),
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
            httpContextAccessorMock.Setup(x => x.HttpContext.Response.Cookies.Append("Token", token,
                                                                                     It.IsAny<CookieOptions>()))
                                   .Verifiable();
            httpContextAccessorMock.Setup(x => x.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken,
                                                                                     It.IsAny<CookieOptions>()))
                                   .Verifiable();
        }
    }
}
