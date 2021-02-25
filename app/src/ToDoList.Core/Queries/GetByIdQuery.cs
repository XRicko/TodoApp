using MediatR;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Queries
{
    public class GetByIdQuery<T> : IRequest<T> where T : BaseEntity
    {
        public int Id { get; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
