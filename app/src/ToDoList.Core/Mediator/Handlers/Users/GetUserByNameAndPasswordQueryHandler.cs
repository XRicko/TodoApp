using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    internal class GetUserByNameAndPasswordQueryHandler : HandlerBase, IRequestHandler<GetUserByNameAndPasswordQuery, UserResponse>
    {
        public GetUserByNameAndPasswordQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<UserResponse> Handle(GetUserByNameAndPasswordQuery request, CancellationToken cancellationToken)
        {
            var users = await UnitOfWork.Repository.GetAllAsync<User>();
            var user = users.SingleOrDefault(u => u.Name == request.Name
                                                  && u.Password == request.Passsword);

            var response = Mapper.Map<UserResponse>(user);

            return response;
        }
    }
}
