using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Logic.Services
{
    public class DirectoryProvider : ServiceBase, IDirectoryProvider
    {
        public DirectoryProvider(ILogger<DirectoryProvider> logger) : base(logger)
        {
        }

        public override Task<bool> Initialize()
        {
            var assemblyLocation = Assembly.GetAssembly(this.GetType()).Location;
            var currentDirectory = Directory.GetParent(assemblyLocation);
            AssemblyDirectory = currentDirectory.FullName;
            Logger.LogInformation($"Actual path: {currentDirectory}");
            return base.Initialize();
        }

        public string AssemblyDirectory { get; set; }
    }
}