using System.Threading.Tasks;

namespace ToDoList.Core.Services
{
    public interface IGeocodingService
    {
        Task<string> GetAddressAsync(double latitude, double longitude);
    }
}