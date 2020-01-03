using System;
using System.Linq;

namespace TFU002.Interfaces.Models
{
    public class ExtenalPlcSetting
    {
        public string Name { get; set; } = "s7-300";
        public string IpAddress { get; set; } = "10.30.10.50";
        public int Port { get; set; } = 102;
        public int Rack { get; set; } = 0;
        public int Slot { get; set; } = 2;
    }
}