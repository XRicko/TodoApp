
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Categorys
{
    class AddCategoryCommandHandler : AddCommandHandler<Category>
    {
        public AddCategoryCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
