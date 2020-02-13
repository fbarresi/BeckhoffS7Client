using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sharp7.Rx;
using Sharp7.Rx.Interfaces;
using TFU002.Interfaces.Extensions;
using TFU002.Interfaces.Models;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Logic.Services
{
    public class PlcProvider : ServiceBase, IPlcProvider
    {
        private readonly ILogger<IPlcProvider> logger;
        private readonly ISettingsProvider settingsProvider;
        public Dictionary<string, IPlc> Plcs { get; private set; } = new Dictionary<string, IPlc>();

        public PlcProvider(ILogger<IPlcProvider> logger, ISettingsProvider settingsProvider) : base(logger)
        {
            this.logger = logger;
            this.settingsProvider = settingsProvider;
        }

        private async Task<IPlc> InitializePlc(ExtenalPlcSetting setting)
        {
            var plc = new Sharp7Plc(setting.IpAddress, setting.Rack, setting.Slot);
            await plc.InitializeAsync();
            plc.ConnectionState
                .DistinctUntilChanged()
                .Do(state => logger.LogDebug($"PLC '{setting.Name}' state changed to '{state}'"))
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            return plc;
        }

        private async Task InitializeAllPlc(ApplicationSettings settings)
        {
            foreach (var plcSetting in settings.ExtenalPlcSettings)
            {
                Plcs[plcSetting.Name] = await InitializePlc(plcSetting);
                Disposables.Add(Plcs[plcSetting.Name]);
            }
        }

        public override async Task<bool> Initialize()
        {
            await InitializeAllPlc(settingsProvider.Settings);
            return await base.Initialize();
        }

        public IPlc GetPlc(string name)
        {
            return Plcs.ContainsKey(name) ? Plcs[name] : null;
        }
        
        public IPlc GetPlc()
        {
            return Plcs.Any() ? Plcs.First().Value : null;
        }
    }
}