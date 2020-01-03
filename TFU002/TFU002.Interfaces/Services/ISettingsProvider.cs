using System.Threading.Tasks;
using TFU002.Interfaces.Models;

namespace TFU002.Interfaces.Services
{
    public interface ISettingsProvider
    {
        ApplicationSettings Settings { get; }
    }
}