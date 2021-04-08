using MediatR;

using Moq;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public abstract class ApiControllerBaseForTests
    {
        protected Mock<IMediator> MediatorMock { get; }

        protected ApiControllerBaseForTests()
        {
            MediatorMock = new Mock<IMediator>();
        }
    }
}