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
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace TFU002.Logic.Services
{
    public class BeckhoffService : ServiceBase, IBeckhoffService
    {
        private readonly ILogger<IPlcProvider> logger;
        private readonly ISettingsProvider settingsProvider;
        private readonly BehaviorSubject<ConnectionState> connectionStateSubject = new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.Unknown);
        private readonly BehaviorSubject<AdsState> adsStateSubject = new BehaviorSubject<AdsState>(TwinCAT.Ads.AdsState.Init);
        public IObservable<ConnectionState> ConnectionState => connectionStateSubject.AsObservable();
        public IObservable<AdsState> AdsState => adsStateSubject.AsObservable();

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
            
            Client.WhenAdsStateChanges()
                .Subscribe(adsStateSubject.OnNext)
                .AddDisposableTo(Disposables);

            adsStateSubject
                .DistinctUntilChanged()
                .Do(state => logger.LogDebug($"Ads state changed to '{state}'"))
                .Where(state => state == TwinCAT.Ads.AdsState.Run)
                .Do(UpdateSymbols)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            return await base.Initialize();
        }
        
        private void UpdateSymbols(AdsState state)
        {
            if (state == TwinCAT.Ads.AdsState.Run)
            {
                logger.LogDebug($"Update symbols on beckhoff change to {state}");

                var loader = SymbolLoaderFactory.Create(Client, new SymbolLoaderSettings(SymbolsLoadMode.Flat));
                symbolsSubject.OnNext(loader.Symbols);
            }
            else
            {
                logger.LogDebug($"Deleting symbols on beckhoff state change to {state}");
                symbolsSubject.OnNext(null);
            }
        }
        private BehaviorSubject<ISymbolCollection<ISymbol>> symbolsSubject = new BehaviorSubject<ISymbolCollection<ISymbol>>(null);
        public IObservable<ISymbolCollection<ISymbol>> Symbols => symbolsSubject.AsObservable();
    }
}