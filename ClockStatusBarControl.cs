using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Threading;

namespace StatusBarClock
{
    /// <summary>
    /// Status bar control that displays the current time
    /// </summary>
    public class ClockStatusBarControl : IDisposable
    {
        private readonly StatusBarClockPackage package;
        private readonly IVsStatusbar statusBar;
        private DispatcherTimer timer;
        private bool isDisposed = false;
        private object icon = (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_General;

        public ClockStatusBarControl(StatusBarClockPackage package, IVsStatusbar statusBar)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.statusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));

            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("CLOCK CONTROL CONSTRUCTOR");
            System.Diagnostics.Debug.WriteLine("========================================");

            InitializeTimer();
            UpdateClock();
        }

        private void InitializeTimer()
        {
            System.Diagnostics.Debug.WriteLine(">>> InitializeTimer START");
            
            var options = GetOptions();
            
            timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(options.UpdateInterval)
            };
            timer.Tick += OnTimerTick;
            
            if (options.Enabled)
            {
                timer.Start();
                System.Diagnostics.Debug.WriteLine($"Timer STARTED with interval: {options.UpdateInterval}ms");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Timer NOT started (Enabled=false)");
            }
            
            System.Diagnostics.Debug.WriteLine(">>> InitializeTimer END");
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            try
            {
                var options = GetOptions();

                if (!options.Enabled)
                {
                    statusBar.SetText("");
                    System.Diagnostics.Debug.WriteLine("Clock is DISABLED - cleared status bar");
                    return;
                }

                string format = options.GetEffectiveFormat();
                string timeText = options.PrefixText + DateTime.Now.ToString(format);

                // IVsStatusbar.SetText を使って直接テキストを設定
                int result = statusBar.SetText(timeText);
                
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Clock updated: '{timeText}' (result={result})");
                
                // フリーズを解除（他のコンポーネントがステータスバーを使えるように）
                int frozen;
                statusBar.IsFrozen(out frozen);
                if (frozen != 0)
                {
                    statusBar.FreezeOutput(0);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! EXCEPTION in UpdateClock: {ex.Message}");
            }
        }

        private ClockOptions GetOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return package.GetDialogPage(typeof(ClockOptions)) as ClockOptions;
        }

        public void RefreshSettings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            System.Diagnostics.Debug.WriteLine(">>> RefreshSettings called");
            var options = GetOptions();
            
            if (options.Enabled)
            {
                if (timer != null && !timer.IsEnabled)
                {
                    timer.Start();
                }
                if (timer != null)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(options.UpdateInterval);
                }
                UpdateClock();
            }
            else
            {
                if (timer != null)
                {
                    timer.Stop();
                }
                statusBar.SetText("");
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                
                System.Diagnostics.Debug.WriteLine(">>> Dispose called");
                
                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= OnTimerTick;
                    timer = null;
                }

                statusBar?.SetText("");

                isDisposed = true;
            }
        }
    }
}
