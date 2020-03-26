using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Logic.Services
{
    public class DirectoryProvider : IDirectoryProvider
    {

        public DirectoryProvider(ILogger<DirectoryProvider> logger)
        {
            var assemblyLocation = Assembly.GetAssembly(this.GetType()).Location;
            var currentDirectory = Directory.GetParent(assemblyLocation);
            AssemblyDirectory = currentDirectory.FullName;
            logger.LogDebug($"Actual path: {currentDirectory}");
        }

        public string AssemblyDirectory { get; }
    }
}