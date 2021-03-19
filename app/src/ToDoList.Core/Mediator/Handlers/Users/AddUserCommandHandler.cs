using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    internal class AddUserCommandHandler : AddCommandHandler<UserRequest, User>
    {
        public AddUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<UserRequest> request, CancellationToken cancellationToken)
        {
            var users = await UnitOfWork.Repository.GetAllAsync<User>();
            var item = users.SingleOrDefault(u => u.Name == request.Request.Name
                                                  && u.Password == request.Request.Password);

            if (item is null)
            {
                var user = Mapper.Map<User>(request.Request);

                await UnitOfWork.Repository.AddAsync(user);
                await UnitOfWork.SaveAsync();

                var defaultCheckilst = new Checklist { Name = "Untitled", UserId = user.Id };

                await UnitOfWork.Repository.AddAsync(defaultCheckilst);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
