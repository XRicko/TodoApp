
using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Users
{
    public class GetUserByNameAndPasswordQuery : IRequest<UserResponse>
    {
        public string Name { get; }
        public string Password { get; set; }

        public GetUserByNameAndPasswordQuery(string name, string passsword)
        {
            Name = name;
            Password = passsword;
        }
    }
}
