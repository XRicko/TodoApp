using Moq;

using ToDoList.MvcClient.Services.Api;

namespace ToDoList.UnitTests.MvcClient.Controllers
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
