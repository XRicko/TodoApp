using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.MvcClient.Models
{
    public class UserModel : BaseModel
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
