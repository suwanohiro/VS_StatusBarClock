using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace StatusBarClock
{
    /// <summary>
    /// ステータスバー時計のオプション設定を管理するクラス
    /// </summary>
    /// <remarks>
    /// DialogPageを継承し、Visual Studio 2025の統合設定UIに対応しています。
    /// プロパティグリッドによる自動UI生成とプロパティの永続化機能を提供します。
    /// </remarks>
    [ComVisible(true)]
    public class ClockOptions : DialogPage
    {
        /// <summary>
        /// 設定が保存された時に発生するイベント
        /// </summary>
        /// <remarks>
        /// StatusBarClockPackageがこのイベントをサブスクライブし、
        /// ClockStatusBarControlに設定の更新を通知します。
        /// </remarks>
        public event EventHandler SettingsSaved;

        // デフォルト値定数
        private const string DefaultFormat = "yyyy-MM-dd (dddd) HH:mm:ss";
        private const int DefaultUpdateInterval = 1000;
        private const bool DefaultShowMilliseconds = false;
        private const string DefaultPrefixText = "";
        private const bool DefaultEnabled = true;

        /// <summary>
        /// 日付と時刻の表示フォーマット文字列
        /// </summary>
        /// <remarks>
        /// .NET標準のDateTime.ToString()メソッドで使用されるフォーマット文字列を指定します。
        /// 例: "yyyy-MM-dd HH:mm:ss", "HH:mm", "yyyy年MM月dd日 HH:mm:ss"
        /// ShowMillisecondsがtrueの場合、自動的に".fff"が追加されます。
        /// </remarks>
        [Category("Display")]
        [DisplayName("Date/Time Format")]
        [Description("Format string for the date/time display. Examples: yyyy-MM-dd HH:mm:ss or MM/dd/yyyy hh:mm:ss tt")]
        [Browsable(true)]
        public string DateTimeFormat { get; set; } = DefaultFormat;

        /// <summary>
        /// 時計表示の更新間隔(ミリ秒)
        /// </summary>
        /// <remarks>
        /// 有効範囲は10～60000ms。OnApplyメソッドで自動的に範囲内に調整されます。
        /// 推奨値: 通常使用は1000ms、ミリ秒表示は100ms、省電力は5000ms以上
        /// </remarks>
        [Category("Display")]
        [DisplayName("Update Interval (ms)")]
        [Description("How often the clock updates in milliseconds. Default is 1000 (1 second). Set to 100 for smoother millisecond display.")]
        [Browsable(true)]
        public int UpdateInterval { get; set; } = DefaultUpdateInterval;

        /// <summary>
        /// ミリ秒表示の有効/無効
        /// </summary>
        /// <remarks>
        /// trueに設定すると、DateTimeFormatに".fff"が自動的に追加されます(既に含まれていない場合)。
        /// 滑らかな表示にするには、UpdateIntervalを100ms程度に設定することを推奨します。
        /// </remarks>
        [Category("Display")]
        [DisplayName("Show Milliseconds")]
        [Description("When enabled, appends milliseconds (.fff) to the format if not already present.")]
        [Browsable(true)]
        public bool ShowMilliseconds { get; set; } = DefaultShowMilliseconds;

        /// <summary>
        /// 時刻の前に表示するプレフィックステキスト
        /// </summary>
        /// <remarks>
        /// 任意のテキストを時刻の前に追加できます。
        /// 例: "Time: ", "🕐 ", "現在時刻: "
        /// </remarks>
        [Category("Display")]
        [DisplayName("Prefix Text")]
        [Description("Text to display before the time")]
        [Browsable(true)]
        public string PrefixText { get; set; } = DefaultPrefixText;

        /// <summary>
        /// ステータスバー時計の有効/無効
        /// </summary>
        /// <remarks>
        /// falseに設定すると、タイマーが停止しステータスバーのテキストがクリアされます。
        /// 設定変更は即座に反映されます(Visual Studioの再起動不要)。
        /// </remarks>
        [Category("Display")]
        [DisplayName("Enabled")]
        [Description("Enable or disable the status bar clock.")]
        [Browsable(true)]
        public bool Enabled { get; set; } = DefaultEnabled;

        /// <summary>
        /// オプションダイアログで適用またはOKボタンがクリックされた時の処理
        /// </summary>
        /// <remarks>
        /// <para>
        /// 以下の検証を行います:
        /// </para>
        /// <list type="bullet">
        /// <item><description>UpdateIntervalを10～60000msの範囲に自動調整</description></item>
        /// <item><description>DateTimeFormatの妥当性をDateTime.Now.ToString()で確認</description></item>
        /// </list>
        /// <para>
        /// 検証成功時はSettingsSavedイベントを発行し、StatusBarClockPackageに通知します。
        /// </para>
        /// </remarks>
        /// <param name="e">適用イベントの引数</param>
        protected override void OnApply(PageApplyEventArgs e)
        {
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

                base.OnApply(e);
                SettingsSaved?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                base.OnApply(e);
            }
        }

        /// <summary>
        /// ShowMillisecondsの設定を考慮した実際の日時フォーマット文字列を取得
        /// </summary>
        /// <remarks>
        /// ShowMillisecondsがtrueで、DateTimeFormatにミリ秒指定(.f, .ff, .fff)が
        /// 含まれていない場合、自動的に".fff"を末尾に追加します。
        /// </remarks>
        /// <returns>実際に使用される日時フォーマット文字列</returns>
        public string GetEffectiveFormat()
        {
            if (ShowMilliseconds && !DateTimeFormat.Contains(".fff") && !DateTimeFormat.Contains(".ff") && !DateTimeFormat.Contains(".f"))
            {
                return DateTimeFormat + ".fff";
            }
            return DateTimeFormat;
        }

        /// <summary>
        /// すべての設定をデフォルト値にリセット
        /// </summary>
        /// <remarks>
        /// オプションダイアログの「既定値にリセット」ボタンがクリックされた時に呼び出されます。
        /// </remarks>
        public override void ResetSettings()
        {
            DateTimeFormat = DefaultFormat;
            UpdateInterval = DefaultUpdateInterval;
            ShowMilliseconds = DefaultShowMilliseconds;
            PrefixText = DefaultPrefixText;
            Enabled = DefaultEnabled;

            base.ResetSettings();
        }
    }
}