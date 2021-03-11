using MediatR;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Commands
{
    public class RemoveCommand<TEntity> : IRequest where TEntity : BaseEntity
    {
        public int Id { get; set; }

        public RemoveCommand(int id)
        {
            Id = id;
        }
    }
}
