
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Categories
{
    class GetCategoryByIdQueryHandler : GetByIdQueryHandler<Category>
    {
        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
