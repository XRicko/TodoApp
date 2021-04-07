using AutoMapper;

using Moq;

using ToDoList.Core.MappingProfiles;
using ToDoList.SharedKernel.Interfaces;

namespace Core.Tests.Handlers
{
    public abstract class HandlerBaseForTests
    {
        protected Mock<IRepository> RepoMock { get; }
        protected Mock<IUnitOfWork> UnitOfWorkMock { get; }

        protected IMapper Mapper { get; private set; }


        protected HandlerBaseForTests()
        {
            RepoMock = new Mock<IRepository>();
            UnitOfWorkMock = new Mock<IUnitOfWork>();

            UnitOfWorkMock.SetupGet(x => x.Repository)
             .Returns(RepoMock.Object);

            var profile = new EntityToDtoMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            Mapper = new Mapper(configuration);
        }
    }
}
