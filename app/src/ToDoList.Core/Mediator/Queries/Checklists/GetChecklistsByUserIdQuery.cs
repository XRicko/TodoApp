using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Checklists
{
    public class GetChecklistsByUserIdQuery : IRequest<IEnumerable<ChecklistResponse>>
    {
        public int UserId { get; }

        public GetChecklistsByUserIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
