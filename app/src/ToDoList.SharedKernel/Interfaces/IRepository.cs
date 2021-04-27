using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.SharedKernel.Interfaces
{
    public interface IRepository
    {
        Task<T> GetByNameAsync<T>(string name) where T : BaseEntity;
        Task<T> FindByPrimaryKeysAsync<T>(params object[] keys) where T : BaseEntity;

        IQueryable<T> GetAll<T>() where T : BaseEntity;

        Task AddAsync<T>(T entity) where T : BaseEntity;
        void Update<T>(T entity) where T : BaseEntity;
        void Remove<T>(T entity) where T : BaseEntity;
    }
}
