
using Moq;

using ToDoList.SharedClientLibrary.Services;

namespace TestExtensions
{
    public static class TokenStorageMockExtensions
    {
        public static void SetupGettingToken(this Mock<ITokenStorage> tokenStorageMock,
                                             string key, string token)
        {
            tokenStorageMock.Setup(x => x.GetTokenAsync(key))
                            .ReturnsAsync(token)
                            .Verifiable();
        }


    }
}
