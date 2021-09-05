using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerDirectory
{
    public interface IManager
    {
        Task Start();
        Task Run();
    }
}
