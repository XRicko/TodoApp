
using Moq;

using ToDoList.MvcClient.Services.Api;

namespace ToDoList.UnitTests.MvcClient.Controllers
{
    abstract public class MvcControllerBaseForTests
    {
        protected Mock<IApiCallsService> ApiCallsServiceMock { get; }

        public MvcControllerBaseForTests()
        {
            ApiCallsServiceMock = new Mock<IApiCallsService>();
        }
    }
}
