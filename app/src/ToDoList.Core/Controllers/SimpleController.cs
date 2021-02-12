using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Controllers
{
    public class SimpleController : ControllerBase
    {
        public SimpleController(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
