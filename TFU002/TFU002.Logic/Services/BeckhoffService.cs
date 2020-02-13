using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Extensions;
using TFU002.Interfaces.Models;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace TFU002.Logic.Services
{
    public class BeckhoffService : ServiceBase, IBeckhoffService
    {
        private readonly ILogger<IPlcProvider> logger;
        private readonly ISettingsProvider settingsProvider;
        private readonly BehaviorSubject<ConnectionState> connectionStateSubject = new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.Unknown);
        public IObservable<ConnectionState> ConnectionState => connectionStateSubject.AsObservable();
        public BeckhoffService(ILogger<IPlcProvider> logger, ISettingsProvider settingsProvider) : base(logger)
        {
            this.logger = logger;
            this.settingsProvider = settingsProvider;
            Client = new AdsClient();
        }

        public AdsClient Client { get; }

        private void InitializeBeckhoff(BeckhoffSettings setting)
        {
            logger.LogInformation($"Connecting with Beckhoff at '{setting.AmsNetId}:{setting.Port}'...");
            if (string.IsNullOrEmpty(setting.AmsNetId))
            {
                Client.Connect(setting.Port); //Connect ads locally
            }
            else
            {
                Client.Connect(setting.AmsNetId, setting.Port);
            }
        }

        public override async Task<bool> Initialize()
        {
            try
            {
                InitializeBeckhoff(settingsProvider.Settings.BeckhoffSettings);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while initializing beckhoff");
            }
            
            Observable.FromEventPattern<ConnectionStateChangedEventArgs>(ev => Client.ConnectionStateChanged += ev,
                                                                         ev => Client.ConnectionStateChanged -= ev)
                .Select(pattern => pattern.EventArgs.NewState)
                .Subscribe(connectionStateSubject.OnNext)
                .AddDisposableTo(Disposables);

            connectionStateSubject
                .DistinctUntilChanged()
                .Do(state => logger.LogDebug($"Beckhoff state changed to '{state}'"))
                .Where(state => state == TwinCAT.ConnectionState.Connected)
                .Do(UpdateSymbols)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            return await base.Initialize();
        }
        
        private void UpdateSymbols(ConnectionState state)
        {
            logger.LogDebug("Update beckhoff symbols on beckhoff became connected");
            if (state == TwinCAT.ConnectionState.Connected)
            {
                var loader = SymbolLoaderFactory.Create(Client, new SymbolLoaderSettings(SymbolsLoadMode.Flat));
                symbolsSubject.OnNext(loader.Symbols);
            }
            else
            {
                symbolsSubject.OnNext(null);
            }
        }
        private Subject<ISymbolCollection<ISymbol>> symbolsSubject = new Subject<ISymbolCollection<ISymbol>>();
        public IObservable<ISymbolCollection<ISymbol>> Symbols => symbolsSubject.AsObservable();
    }
}