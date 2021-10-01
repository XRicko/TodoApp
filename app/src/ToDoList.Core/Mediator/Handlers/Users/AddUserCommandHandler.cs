using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    internal class AddUserCommandHandler : AddCommandHandler<UserRequest, User>
    {
        private readonly IPasswordHasher passwordHasher;

        public AddUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher hasher) : base(unitOfWork, mapper)
        {
            passwordHasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public override async Task<Unit> Handle(AddCommand<UserRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            bool recordExists = UnitOfWork.Repository.GetAll<User>()
                                                     .Any(u => u.Name == request.Request.Name);

            if (!recordExists)
            {
                string hashedPassword = passwordHasher.Hash(request.Request.Password, 10000);

                var user = Mapper.Map<User>(request.Request);
                user.Password = hashedPassword;

                UnitOfWork.Repository.Add(user);
                await UnitOfWork.SaveAsync();

                var defaultProject = new Project { Name = "Untitled", UserId = user.Id };

                UnitOfWork.Repository.Add(defaultProject);
                await UnitOfWork.SaveAsync();

                var defaultChecklist = new Checklist { Name = "Untitled", ProjectId = defaultProject.Id };

                UnitOfWork.Repository.Add(defaultChecklist);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
