using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Basics;
using TFU002.Interfaces.Models;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Logic.Services
{
    public class SettingsProvider : ServiceBase, ISettingsProvider
    {
        public ApplicationSettings Settings { get; set; }
        public SettingsProvider(ILogger<SettingsProvider> logger) : base(logger)
        {
        }

        public override Task<bool> Initialize()
        {
            Logger.LogInformation("Initializing settins provider...");

            return base.Initialize();
        }
    }
}