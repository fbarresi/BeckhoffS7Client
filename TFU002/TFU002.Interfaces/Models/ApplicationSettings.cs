using System.Collections.Generic;

namespace TFU002.Interfaces.Models
{
    public class ApplicationSettings
    {
        public BeckhoffSettings BeckhoffSettings { get; set; } = new BeckhoffSettings();
        public List<ExtenalPlcSetting> ExtenalPlcSettings { get; set; } = new List<ExtenalPlcSetting>();
    }
}