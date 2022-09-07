using System.Threading.Tasks;

namespace ManagerDirectory
{
    public interface IManager
    {
        Task Start();
        Task Run();
    }
}
