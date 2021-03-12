using System;
using System.Threading.Tasks;

namespace ToDoList.SharedKernel.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository Repository { get; }

        Task SaveAsync();
    }
}
