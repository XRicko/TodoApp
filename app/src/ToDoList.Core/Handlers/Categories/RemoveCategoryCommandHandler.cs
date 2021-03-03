
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Categories
{
    class RemoveCategoryCommandHandler : RemoveCommandHandler<Category>
    {
        public RemoveCategoryCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
