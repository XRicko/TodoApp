using System;
using System.Net.Http;

using Moq;

using ToDoList.BlazorClient.Authentication;
using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Services;

namespace BlazorClient.Tests.Services
{
    public class BlazorApiInvokerTests
    {
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;

        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly Mock<ITokenParser> tokenParserMock;

        private readonly HttpClient httpClient;

        private readonly BlazorApiInvoker blazorApiInvoker;

        public BlazorApiInvokerTests()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            tokenStorageMock = new Mock<ITokenStorage>();
            tokenParserMock = new Mock<ITokenParser>();

            httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://localhost:5001/api/") };

            var stateProvider = new AuthStateProvider(httpClient, tokenParserMock.Object, tokenStorageMock.Object);
            blazorApiInvoker = new BlazorApiInvoker(httpClient, stateProvider, tokenStorageMock.Object);
        }

        // TODO
    }
}
