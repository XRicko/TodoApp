using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Controllers
{
    public abstract class ControllerBase
    {
        protected IUnitOfWork UnitOfWork { get; }

        public ControllerBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public Task<T> GetByNameAsync<T>(string name) where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync<T>(int id) where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetItemsAsync<T>() where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task AddItemAsync<T>(T entity) where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync<T>(T entity) where T : BaseEntity
        {
            throw new NotImplementedException();
        }
    }
}
