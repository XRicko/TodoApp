using MediatR;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Commands
{
    public class UpdateCommand<T> : IRequest where T : BaseEntity
    {
        public T Entity { get; set; }

        public UpdateCommand(T entity)
        {
            Entity = entity;
        }
    }
}
