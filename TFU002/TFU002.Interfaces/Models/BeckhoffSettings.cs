using System;
using System.Linq;

namespace TFU002.Interfaces.Models
{
    public class BeckhoffSettings
    {
        public string AmsNetId { get; set; } = "";
        public int Port { get; set; } = 851;
    }
}