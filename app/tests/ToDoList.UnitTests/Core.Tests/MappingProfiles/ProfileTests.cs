using AutoMapper;

using ToDoList.Core.MappingProfiles;

using Xunit;

namespace Core.Tests.MappingProfiles
{
    public class ProfileTests
    {
        [Fact]
        public void EntityToDtoMappingProfile_Valid()
        {
            // Arrange
            var profile = new EntityToDtoMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
