using AutoMapper;

using Moq;

using ToDoList.Core.MappingProfiles;
using ToDoList.SharedKernel.Interfaces;

namespace Core.Handlers
{
    public abstract class HandlerBaseForTests
    {
        protected Mock<IRepository> RepoMock { get; private set; }
        protected Mock<IUnitOfWork> UnitOfWorkMock { get; private set; }

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
