
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
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            if (string.IsNullOrEmpty(passsword))
                throw new System.ArgumentException($"'{nameof(passsword)}' cannot be null or empty", nameof(passsword));

            Name = name;
            Password = passsword;
        }
    }
}
