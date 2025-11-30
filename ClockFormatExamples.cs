using System;
using System.Collections.Generic;

namespace StatusBarClock
{
    /// <summary>
    /// Provides predefined format examples for the clock display
    /// </summary>
    public static class ClockFormatExamples
    {
        public static readonly Dictionary<string, string> Formats = new Dictionary<string, string>
        {
            { "yyyy-MM-dd (dddd) HH:mm:ss", "2024-01-15 (Monday) 14:30:45" },
            { "yyyy-MM-dd HH:mm:ss.fff", "2024-01-15 14:30:45.123" },
            { "yyyy/MM/dd HH:mm:ss", "2024/01/15 14:30:45" },
            { "dd/MM/yyyy HH:mm:ss", "15/01/2024 14:30:45" },
            { "MM/dd/yyyy hh:mm:ss tt", "01/15/2024 02:30:45 PM" },
            { "dddd, MMMM dd, yyyy HH:mm:ss", "Monday, January 15, 2024 14:30:45" },
            { "HH:mm:ss", "14:30:45" },
            { "hh:mm:ss tt", "02:30:45 PM" },
            { "HH:mm", "14:30" },
            { "yyyy-MM-dd", "2024-01-15" },
            { "ddd MMM dd HH:mm:ss", "Mon Jan 15 14:30:45" }
        };

        public static string GetExample(string format)
        {
            try
            {
                return DateTime.Now.ToString(format);
            }
            catch
            {
                return "Invalid format";
            }
        }

        public static string GetFormatHelp()
        {
            return @"Date/Time Format Specifiers:
yyyy - Year (4 digits)         MM - Month (2 digits)       dd - Day (2 digits)
MMMM - Month name (full)       MMM - Month name (short)    
dddd - Day name (full)         ddd - Day name (short)
HH - Hour (24-hour, 2 digits)  hh - Hour (12-hour, 2 digits)
mm - Minutes (2 digits)        ss - Seconds (2 digits)
fff - Milliseconds (3 digits)  ff - Milliseconds (2 digits)  f - Milliseconds (1 digit)
tt - AM/PM designator

Examples:
yyyy-MM-dd (dddd) HH:mm:ss     Å® 2024-01-15 (Monday) 14:30:45
yyyy-MM-dd HH:mm:ss.fff        Å® 2024-01-15 14:30:45.123
MM/dd/yyyy hh:mm:ss tt         Å® 01/15/2024 02:30:45 PM
HH:mm:ss                       Å® 14:30:45";
        }
    }
}
