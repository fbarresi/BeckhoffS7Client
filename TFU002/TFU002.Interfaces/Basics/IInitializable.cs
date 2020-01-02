using System.Threading.Tasks;

namespace TFU002.Interfaces.Basics
{
    public interface IInitializable
    {
        Task<bool> Initialize();
    }
}