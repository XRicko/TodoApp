using System.Threading.Tasks;

using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Data
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly ToDoListContext context;

        public IRepository Repository { get; }

        public UnitOfWork(ToDoListContext toDoListContext, IRepository repository)
        {
            context = toDoListContext;
            Repository = repository;
        }

        public Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
