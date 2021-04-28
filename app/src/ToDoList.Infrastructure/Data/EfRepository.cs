using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Data
{
    internal class EfRepository : IRepository
    {
        private readonly TodoListContext context;

        public EfRepository(TodoListContext toDoListContext)
        {
            context = toDoListContext ?? throw new ArgumentNullException(nameof(toDoListContext));
        }

        public Task<T> GetByNameAsync<T>(string name) where T : BaseEntity
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

            return context.Set<T>().SingleOrDefaultAsync(x => x.Name == name);
        }

        public async Task<T> FindByPrimaryKeysAsync<T>(params object[] keys) where T : BaseEntity
        {
            _ = keys ?? throw new ArgumentNullException(nameof(keys));
            return await context.FindAsync<T>(keys);
        }

        public IQueryable<T> GetAll<T>() where T : BaseEntity =>
            context.Set<T>().AsQueryable().AsNoTrackingWithIdentityResolution();

        public void Add<T>(T entity) where T : BaseEntity
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            context.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<T>(T entity) where T : BaseEntity
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            context.Set<T>().Remove(entity);
        }
    }
}
