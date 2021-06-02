using System.Threading.Tasks;

using Blazored.LocalStorage;

using Moq;

using ToDoList.BlazorClient.Services;

using Xunit;

namespace BlazorClient.Tests.Services
{
    public class LocalTokenStorageTests
    {
        private readonly Mock<ILocalStorageService> localStorageServiceMock;
        private readonly LocalTokenStorage localTokenStorage;

        private readonly string key;
        private readonly string token;

        public LocalTokenStorageTests()
        {
            localStorageServiceMock = new Mock<ILocalStorageService>();
            localTokenStorage = new LocalTokenStorage(localStorageServiceMock.Object);

            key = "accessKey";
            token = "ejnfjeIEnipwfwef.EWPFiJMPxcj";
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsTokenGivenKey()
        {
            // Arrange
            localStorageServiceMock.Setup(x => x.GetItemAsStringAsync(key))
                                   .ReturnsAsync(token)
                                   .Verifiable();

            // Act
            string actual = await localTokenStorage.GetTokenAsync(key);

            // Assert
            Assert.Equal(token, actual);
            localStorageServiceMock.Verify();
        }

        [Fact]
        public async Task SetTokenAsync_SetsTokenGivenKeyAndValue()
        {
            // Act
            await localTokenStorage.SetTokenAsync(key, token);

            // Assert
            localStorageServiceMock.Verify(x => x.SetItemAsStringAsync(key, token), Times.Once);
        }

        [Fact]
        public async Task RemoveTokenAsync()
        {
            // Act
            await localTokenStorage.RemoveTokenAsync(key);

            // Assert
            localStorageServiceMock.Verify(x => x.RemoveItemAsync(key), Times.Once);
        }
    }
}
