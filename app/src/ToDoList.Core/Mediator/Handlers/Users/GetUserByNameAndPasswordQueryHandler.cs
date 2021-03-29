using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    public class GetUserByNameAndPasswordQueryHandler : HandlerBase, IRequestHandler<GetUserByNameAndPasswordQuery, UserResponse>
    {
        public GetUserByNameAndPasswordQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<UserResponse> Handle(GetUserByNameAndPasswordQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var users = await UnitOfWork.Repository.GetAllAsync<User>();
            var user = users.SingleOrDefault(u => u.Name == request.Name
                                                  && passwordHasher.Verify(request.Password, u.Password));

            var response = Mapper.Map<UserResponse>(user);

            return response;
        }
    }
}
