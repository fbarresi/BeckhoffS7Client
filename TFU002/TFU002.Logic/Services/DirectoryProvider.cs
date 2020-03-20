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
        private readonly ILogger<DirectoryProvider> logger;

        public DirectoryProvider(ILogger<DirectoryProvider> logger)
        {
            this.logger = logger;
            var assemblyLocation = Assembly.GetAssembly(this.GetType()).Location;
            var currentDirectory = Directory.GetParent(assemblyLocation);
            AssemblyDirectory = currentDirectory.FullName;
            logger.LogInformation($"Actual path: {currentDirectory}");
        }

        public string AssemblyDirectory { get; }
        public string DefaultPath => Constants.DefaultDirectory;
    }
}