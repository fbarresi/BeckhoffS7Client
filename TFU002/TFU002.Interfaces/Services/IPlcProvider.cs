using System.Collections.Generic;
using Sharp7.Rx.Interfaces;

namespace TFU002.Interfaces.Services
{
    public interface IPlcProvider
    {
        IPlc GetPlc();
        IPlc GetPlc(string name);
    }
}