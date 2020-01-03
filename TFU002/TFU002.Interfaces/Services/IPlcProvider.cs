using System.Collections.Generic;
using Sharp7.Rx.Interfaces;

namespace TFU002.Interfaces.Services
{
    public interface IPlcProvider
    {
        Dictionary<string, IPlc> Plcs { get; }
    }
}