using MediatR;

using Moq;

namespace WebApi.Tests.Controllers
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