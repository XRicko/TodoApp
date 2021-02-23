using MediatR;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Commands
{
    public class RemoveCommand<T> : IRequest where T : BaseEntity
    {
        public int Id { get; set; }

        public RemoveCommand(int id)
        {
            Id = id;
        }
    }
}
