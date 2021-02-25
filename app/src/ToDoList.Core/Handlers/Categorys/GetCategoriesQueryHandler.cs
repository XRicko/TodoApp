using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Categorys
{
    class GetCategoriesQueryHandler : GetAllQueryHandler<Category>
    {
        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
