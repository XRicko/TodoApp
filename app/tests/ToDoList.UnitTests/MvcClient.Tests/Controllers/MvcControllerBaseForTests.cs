using Moq;

using ToDoList.MvcClient.Services.Api;

namespace MvcClient.Tests.Controllers
{
    public abstract class MvcControllerBaseForTests
    {
        protected Mock<IApiInvoker> ApiCallsServiceMock { get; }

        public MvcControllerBaseForTests()
        {
            ApiCallsServiceMock = new Mock<IApiInvoker>();
        }
    }
}
