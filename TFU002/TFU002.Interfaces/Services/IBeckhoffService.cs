using System;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TFU002.Interfaces.Services
{
    public interface IBeckhoffService
    {
        IObservable<ConnectionState> ConnectionState { get; }
        IObservable<AdsState> AdsState { get; }
        AdsClient Client { get; }
        IObservable<ISymbolCollection<ISymbol>> Symbols { get; }
        public TimeSpan NotificationCycleTime { get; }
        public TimeSpan IntervalTransmissionCycleTime { get; }
    }
}