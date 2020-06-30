using System;

namespace TFU002.Interfaces.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan AtLeast(this TimeSpan timeSpan, TimeSpan minimalTimeSpan)
        {
            if (timeSpan < minimalTimeSpan) return minimalTimeSpan;
            return timeSpan;
        }
    }
}
