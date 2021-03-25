using MediatR;

using Moq;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    abstract public class ControllerBaseForTests
    {
        protected Mock<IMediator> MediatorMock { get; }

        protected ControllerBaseForTests()
        {
            MediatorMock = new Mock<IMediator>();
        }
    }
}