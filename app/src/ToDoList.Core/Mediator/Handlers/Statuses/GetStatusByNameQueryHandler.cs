
using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Statuses
{
    internal class GetStatusByNameQueryHandler : GetByNameQueryHandler<Status, StatusResponse>
    {
        [ExcludeFromCodeCoverage]
        public GetStatusByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
