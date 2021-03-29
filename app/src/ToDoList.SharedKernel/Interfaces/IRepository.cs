using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoList.SharedKernel.Interfaces
{
    public interface IRepository
    {
        Task<T> GetAsync<T>(int id) where T : BaseEntity;
        Task<T> GetAsync<T>(string name) where T : BaseEntity;

        Task<IEnumerable<T>> GetAllAsync<T>() where T : BaseEntity;

        Task AddAsync<T>(T entity) where T : BaseEntity;
        void Update<T>(T entity) where T : BaseEntity;
        void Remove<T>(T entity) where T : BaseEntity;
    }
}
