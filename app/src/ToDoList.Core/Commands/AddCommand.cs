using MediatR;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Commands
{
    public class AddCommand<T> : IRequest where T : BaseEntity
    {
        public T Entity { get; }

        public AddCommand(T entity)
        {
            Entity = entity;
        }
    }
}
