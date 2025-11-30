using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace StatusBarClock
{
    /// <summary>
    /// Options for the status bar clock
    /// </summary>
    public class ClockOptions : DialogPage
    {
        public event EventHandler SettingsSaved;

        [Category("Display")]
        [DisplayName("Date/Time Format")]
        [Description("Format string for the date/time display.\n\n" +
                     "Format Specifiers:\n" +
                     "yyyy = Year (4 digits)      MM = Month (2 digits)        dd = Day (2 digits)\n" +
                     "MMMM = Month name (full)    MMM = Month name (short)\n" +
                     "dddd = Day name (full)      ddd = Day name (short)\n" +
                     "HH = Hour 24h (00-23)       hh = Hour 12h (01-12)\n" +
                     "mm = Minutes (00-59)        ss = Seconds (00-59)\n" +
                     "fff = Milliseconds          tt = AM/PM\n\n" +
                     "Examples:\n" +
                     "yyyy-MM-dd (dddd) HH:mm:ss Å® 2024-01-15 (Monday) 14:30:45\n" +
                     "yyyy-MM-dd HH:mm:ss.fff Å® 2024-01-15 14:30:45.123\n" +
                     "MM/dd/yyyy hh:mm:ss tt Å® 01/15/2024 02:30:45 PM")]
        [DefaultValue("yyyy-MM-dd (dddd) HH:mm:ss")]
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd (dddd) HH:mm:ss";

        [Category("Display")]
        [DisplayName("Update Interval (ms)")]
        [Description("How often the clock updates in milliseconds. Default is 1000 (1 second). Set to 100 for smoother millisecond display.")]
        [DefaultValue(1000)]
        public int UpdateInterval { get; set; } = 1000;

        [Category("Display")]
        [DisplayName("Show Milliseconds")]
        [Description("When enabled, appends milliseconds (.fff) to the format if not already present.")]
        [DefaultValue(false)]
        public bool ShowMilliseconds { get; set; } = false;

        [Category("Display")]
        [DisplayName("Prefix Text")]
        [Description("Text to display before the time (e.g., 'Time: ' or '?? ')")]
        [DefaultValue("")]
        public string PrefixText { get; set; } = "";

        [Category("Display")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the status bar clock.")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("Preview")]
        [DisplayName("Current Preview")]
        [Description("Shows how the current format will be displayed (read-only).")]
        [ReadOnly(true)]
        public string Preview
        {
            get
            {
                try
                {
                    string format = GetEffectiveFormat();
                    return PrefixText + DateTime.Now.ToString(format);
                }
                catch
                {
                    return "Invalid format string";
                }
            }
        }

        public string GetEffectiveFormat()
        {
            if (ShowMilliseconds && !DateTimeFormat.Contains(".fff") && !DateTimeFormat.Contains(".ff") && !DateTimeFormat.Contains(".f"))
            {
                return DateTimeFormat + ".fff";
            }
            return DateTimeFormat;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                // Validate update interval
                if (UpdateInterval < 10)
                {
                    UpdateInterval = 10;
                }
                else if (UpdateInterval > 60000)
                {
                    UpdateInterval = 60000;
                }

                // Test format string
                try
                {
                    DateTime.Now.ToString(GetEffectiveFormat());
                }
                catch (FormatException)
                {
                    e.ApplyBehavior = ApplyKind.CancelNoNavigate;
                    return;
                }

                SettingsSaved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
