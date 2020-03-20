using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TFU002.Interfaces.Basics;
using TFU002.Interfaces.Models;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Logic.Services
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ILogger<SettingsProvider> logger;
        private readonly IDirectoryProvider directoryProvider;
        public ApplicationSettings Settings { get; private set; }
        private void LoadOrCreateSettings()
        {
            var settingsPath = Path.Combine(directoryProvider.DefaultPath, "TFU002.settings.json");
            logger.LogInformation($"Looking for application settings into {settingsPath}");
            try
            {
                if (File.Exists(settingsPath))
                {
                    Settings = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(settingsPath));
                    logger.LogInformation("Application settings successfully loaded!");
                }
                else
                {
                    logger.LogInformation("Application settings do not exist...");
                    var fallbackSettingsPath = Path.Combine(directoryProvider.AssemblyDirectory, "TFU002.settings.json");
                    logger.LogInformation($"looking for settings into {fallbackSettingsPath}...");
                    if (File.Exists(fallbackSettingsPath))
                    {
                        Settings = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(fallbackSettingsPath));
                        logger.LogInformation("Application settings successfully loaded!");
                    }
                    else
                    {
                        logger.LogInformation("Application settings not found also in fallback path...");
                        Settings = new ApplicationSettings();
                        Settings.ExtenalPlcSettings.Add(new ExtenalPlcSetting());
                        var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                        if (Directory.Exists(directoryProvider.DefaultPath))
                        {
                            logger.LogInformation($"Writing default settings into {settingsPath}");
                            File.WriteAllText(settingsPath, json);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while loading application settings");
            }
        }

        public SettingsProvider(ILogger<SettingsProvider> logger, IDirectoryProvider directoryProvider)
        {
            this.logger = logger;
            this.directoryProvider = directoryProvider;
            LoadOrCreateSettings();
        }
        
    }
}