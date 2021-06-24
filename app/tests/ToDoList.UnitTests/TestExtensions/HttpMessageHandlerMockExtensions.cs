﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;

namespace TestExtensions
{
    public static class HttpMessageHandlerMockExtensions
    {
        public static void SetupHttCall(this Mock<HttpMessageHandler> httpMessageHandlerMock,
                                        Uri uri, HttpMethod httpMethod, HttpStatusCode statusCode,
                                        ObjectContent objectContent = null)
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                  ItExpr.Is<HttpRequestMessage>(x => x.Method == httpMethod
                                                                                     && x.RequestUri == uri),
                                                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = objectContent,
                })
                .Verifiable();
        }
    }
}
