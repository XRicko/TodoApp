using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Categories
{
    class GetCategoriesQueryHandler : GetAllQueryHandler<Category>
    {
        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
