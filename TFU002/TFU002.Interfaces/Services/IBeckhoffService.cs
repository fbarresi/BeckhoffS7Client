using System;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TFU002.Interfaces.Services
{
    public interface IBeckhoffService
    {
        IObservable<ConnectionState> ConnectionState { get; }
        AdsClient Client { get; }
        IObservable<ISymbolCollection<ISymbol>> Symbols { get; }
    }
}