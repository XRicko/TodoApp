using Moq;

using ToDoList.MvcClient.Services.Api;

namespace MvcClient.Tests.Controllers
{
    public abstract class MvcControllerBaseForTests
    {
        protected Mock<IApiInvoker> ApiInvokerMock { get; }

        public MvcControllerBaseForTests()
        {
            ApiInvokerMock = new Mock<IApiInvoker>();
        }
    }
}
