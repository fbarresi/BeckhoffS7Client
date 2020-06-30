using System;
using System.Collections.Generic;

namespace TFU002.Interfaces.Models
{
    public class ApplicationSettings
    {
        public BeckhoffSettings BeckhoffSettings { get; set; } = new BeckhoffSettings();
        public List<ExtenalPlcSetting> ExtenalPlcSettings { get; set; } = new List<ExtenalPlcSetting>();
        public TimeSpan NotificationCycleTime { get; set; } = TimeSpan.FromMilliseconds(100);
        public TimeSpan IntervalTransmissionCycleTime { get; set; } = TimeSpan.FromSeconds(1);
    }
}