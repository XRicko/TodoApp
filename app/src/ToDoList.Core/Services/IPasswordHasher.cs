using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Core.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password, int iterations);
        bool Verify(string password, string hashedPassword);
    }
}
