using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace StatusBarClock
{
    /// <summary>
    /// Visual Studioステータスバーに時計を表示する拡張機能パッケージのメインクラス
    /// </summary>
    /// <remarks>
    /// AsyncPackageを継承し、バックグラウンド読み込みに対応しています。
    /// VS2025の統合設定UIに対応したオプションページとプロファイル同期機能を提供します。
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(StatusBarClockPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(ClockOptions), "Status Bar Clock", "General", 0, 0, false)]
    [ProvideProfile(typeof(ClockOptions), "Status Bar Clock", "General", 0, 0, true)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class StatusBarClockPackage : AsyncPackage
    {
        /// <summary>
        /// パッケージの一意識別子(GUID)
        /// </summary>
        /// <remarks>
        /// この値を変更すると既存のユーザー設定が失われます。
        /// </remarks>
        public const string PackageGuidString = "6a69641d-7d0f-4a16-bd2f-c3467d670b2e";

        /// <summary>
        /// ステータスバーに時計を表示するコントロールのインスタンス
        /// </summary>
        private ClockStatusBarControl clockControl;

        #region Package Members

        /// <summary>
        /// パッケージの非同期初期化処理
        /// </summary>
        /// <remarks>
        /// <para>
        /// UIスレッドに切り替えた後、以下の処理を実行します:
        /// </para>
        /// <list type="number">
        /// <item><description>IVsStatusbarサービスの取得</description></item>
        /// <item><description>ClockStatusBarControlの作成と初期化</description></item>
        /// <item><description>ClockOptionsのSettingsSavedイベントハンドラの登録</description></item>
        /// </list>
        /// </remarks>
        /// <param name="cancellationToken">初期化キャンセル用のトークン</param>
        /// <param name="progress">進行状況レポート用のプロバイダー</param>
        /// <returns>初期化処理を表すタスク</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            System.Diagnostics.Debug.WriteLine("==============================================");
            System.Diagnostics.Debug.WriteLine("STATUS BAR CLOCK PACKAGE - INITIALIZE START");
            System.Diagnostics.Debug.WriteLine("==============================================");
            
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            
            System.Diagnostics.Debug.WriteLine("Switched to UI thread");

            var statusBar = await GetServiceAsync(typeof(SVsStatusbar)) as IVsStatusbar;
            System.Diagnostics.Debug.WriteLine($"StatusBar service: {statusBar != null}");
            
            if (statusBar != null)
            {
                clockControl = new ClockStatusBarControl(this, statusBar);
                System.Diagnostics.Debug.WriteLine("ClockStatusBarControl created");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Could not get IVsStatusbar service!");
            }

            var options = GetDialogPage(typeof(ClockOptions)) as ClockOptions;
            if (options != null)
            {
                options.SettingsSaved += OnSettingsSaved;
                System.Diagnostics.Debug.WriteLine("Options event handler registered");
            }
            
            System.Diagnostics.Debug.WriteLine("==============================================");
            System.Diagnostics.Debug.WriteLine("STATUS BAR CLOCK PACKAGE - INITIALIZE END");
            System.Diagnostics.Debug.WriteLine("==============================================");
        }

        /// <summary>
        /// オプション設定保存時のイベントハンドラ
        /// </summary>
        /// <remarks>
        /// ClockStatusBarControl.RefreshSettings()を呼び出し、設定変更を即座に反映します。
        /// タイマー間隔、表示フォーマット、有効/無効状態などがリアルタイムで更新されます。
        /// </remarks>
        /// <param name="sender">イベント送信元(ClockOptionsインスタンス)</param>
        /// <param name="e">イベント引数</param>
        private void OnSettingsSaved(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            clockControl?.RefreshSettings();
        }

        /// <summary>
        /// パッケージのリソースを解放
        /// </summary>
        /// <remarks>
        /// <para>
        /// 以下のクリーンアップ処理を実行します:
        /// </para>
        /// <list type="bullet">
        /// <item><description>ClockOptionsのSettingsSavedイベントハンドラの登録解除</description></item>
        /// <item><description>ClockStatusBarControlの破棄(タイマー停止、ステータスバークリア)</description></item>
        /// </list>
        /// </remarks>
        /// <param name="disposing">マネージドリソースを破棄する場合はtrue</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                
                var options = GetDialogPage(typeof(ClockOptions)) as ClockOptions;
                if (options != null)
                {
                    options.SettingsSaved -= OnSettingsSaved;
                }

                clockControl?.Dispose();
                clockControl = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
