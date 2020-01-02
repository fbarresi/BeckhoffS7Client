using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Basics;

namespace TFU002.Logic.Basics
{
    public class ServiceBase : DisposableBase, IInitializable
    {
        public ILogger Logger { get; }
        public bool Initialized { get; set; }
        public ServiceBase(ILogger logger)
        {
            Logger = logger;
        }
        public virtual Task<bool> Initialize()
        {
            Initialized = true;
            return Task.FromResult(true);
        }
    }
}