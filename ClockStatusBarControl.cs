using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Threading;

namespace StatusBarClock
{
    /// <summary>
    /// Visual Studioのステータスバーに現在時刻を表示するコントロール
    /// </summary>
    /// <remarks>
    /// DispatcherTimerを使用して定期的にステータスバーの時刻表示を更新します。
    /// ClockOptionsの設定に基づいてリアルタイムで動作し、設定変更は即座に反映されます。
    /// </remarks>
    public class ClockStatusBarControl : IDisposable
    {
        /// <summary>親パッケージへの参照(ClockOptions取得に使用)</summary>
        private readonly StatusBarClockPackage package;
        
        /// <summary>Visual StudioのIVsStatusbarサービスへの参照</summary>
        private readonly IVsStatusbar statusBar;
        
        /// <summary>定期更新用のタイマー</summary>
        private DispatcherTimer timer;
        
        /// <summary>破棄済みフラグ(二重Dispose防止)</summary>
        private bool isDisposed = false;
        
        /// <summary>ステータスバーアイコン(現在未使用、将来の拡張用)</summary>
        private object icon = (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_General;

        /// <summary>
        /// ClockStatusBarControlの新しいインスタンスを初期化
        /// </summary>
        /// <remarks>
        /// DispatcherTimerを初期化し、初回の時刻表示を設定します。
        /// Enabledがtrueの場合のみタイマーを開始します。
        /// </remarks>
        /// <param name="package">親パッケージインスタンス</param>
        /// <param name="statusBar">IVsStatusbarサービスインスタンス</param>
        /// <exception cref="ArgumentNullException">packageまたはstatusBarがnullの場合</exception>
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

        /// <summary>
        /// DispatcherTimerを初期化し、設定に基づいて開始
        /// </summary>
        /// <remarks>
        /// ClockOptionsから更新間隔と有効/無効状態を取得してタイマーを構成します。
        /// Enabledがtrueの場合のみタイマーを開始します。
        /// </remarks>
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

        /// <summary>
        /// タイマーのTickイベントハンドラ
        /// </summary>
        /// <remarks>
        /// 設定された更新間隔ごとに呼び出され、UpdateClock()メソッドを実行します。
        /// </remarks>
        /// <param name="sender">イベント送信元(DispatcherTimer)</param>
        /// <param name="e">イベント引数</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        /// <summary>
        /// ステータスバーの時計表示を更新
        /// </summary>
        /// <remarks>
        /// <para>
        /// ClockOptionsから現在の設定を取得し、以下の処理を実行します:
        /// </para>
        /// <list type="number">
        /// <item><description>Enabled設定の確認(falseの場合はステータスバーをクリア)</description></item>
        /// <item><description>GetEffectiveFormat()で実際のフォーマット文字列を取得</description></item>
        /// <item><description>PrefixText + DateTime.Now.ToString(format)で時刻文字列を生成</description></item>
        /// <item><description>IVsStatusbar.SetText()でステータスバーに設定</description></item>
        /// <item><description>ステータスバーがフリーズしている場合は解除</description></item>
        /// </list>
        /// </remarks>
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

                // IVsStatusbar.SetText ���g���Ē��ڃe�L�X�g��ݒ�
                int result = statusBar.SetText(timeText);
                
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Clock updated: '{timeText}' (result={result})");
                
                // �t���[�Y�������i���̃R���|�[�l���g���X�e�[�^�X�o�[���g����悤�Ɂj
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

        /// <summary>
        /// 現在のClockOptionsインスタンスを取得
        /// </summary>
        /// <remarks>
        /// StatusBarClockPackage.GetDialogPage()を使用してClockOptionsを取得します。
        /// Visual Studioによって設定の永続化が自動的に処理されます。
        /// </remarks>
        /// <returns>現在のClockOptionsインスタンス</returns>
        private ClockOptions GetOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return package.GetDialogPage(typeof(ClockOptions)) as ClockOptions;
        }

        /// <summary>
        /// 設定変更時に呼び出され、時計表示を更新
        /// </summary>
        /// <remarks>
        /// <para>
        /// StatusBarClockPackageのOnSettingsSavedイベントハンドラから呼び出されます。
        /// </para>
        /// <para>
        /// Enabledがtrueの場合: タイマー開始、更新間隔設定、即座の表示更新
        /// </para>
        /// <para>
        /// Enabledがfalseの場合: タイマー停止、ステータスバークリア
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// ClockStatusBarControlが使用するリソースを解放
        /// </summary>
        /// <remarks>
        /// <para>
        /// IDisposableインターフェースの実装です。以下の処理を実行します:
        /// </para>
        /// <list type="bullet">
        /// <item><description>タイマーの停止</description></item>
        /// <item><description>OnTimerTickイベントハンドラの登録解除</description></item>
        /// <item><description>ステータスバーのテキストクリア</description></item>
        /// </list>
        /// <para>
        /// isDisposedフラグにより、複数回呼び出しても安全です。
        /// </para>
        /// </remarks>
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
