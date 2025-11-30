using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace StatusBarClock
{
    /// <summary>
    /// 時計オプションコントロールのViewModel
    /// </summary>
    /// <remarks>
    /// INotifyPropertyChangedを実装し、プロパティ変更通知とプレビュー表示機能を提供します。
    /// リアルタイムでフォーマットの妥当性を検証し、プレビューを更新します。
    /// </remarks>
    public class ClockOptionsViewModel : INotifyPropertyChanged
    {
        private string dateTimeFormat = "yyyy-MM-dd (dddd) HH:mm:ss";
        private int updateInterval = 1000;
        private bool showMilliseconds = false;
        private string prefixText = "";
        private bool enabled = true;
        private string preview = "";
        private string validationMessage = "";
        private bool hasValidationError = false;
        private DispatcherTimer previewTimer;

        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ClockOptionsViewModelの新しいインスタンスを初期化
        /// </summary>
        /// <remarks>
        /// プレビュー用のDispatcherTimerを100ms間隔で起動し、リアルタイムプレビューを開始します。
        /// </remarks>
        public ClockOptionsViewModel()
        {
            // Initialize preview timer
            previewTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            previewTimer.Tick += (s, e) => UpdatePreview();
            previewTimer.Start();
        }

        /// <summary>
        /// 日付と時刻の表示フォーマット文字列
        /// </summary>
        /// <remarks>
        /// 変更時にプレビューを自動更新します。
        /// </remarks>
        public string DateTimeFormat
        {
            get => dateTimeFormat;
            set
            {
                if (dateTimeFormat != value)
                {
                    dateTimeFormat = value;
                    OnPropertyChanged();
                    UpdatePreview();
                }
            }
        }

        /// <summary>
        /// 時計表示の更新間隔(ミリ秒)
        /// </summary>
        /// <remarks>
        /// 有効範囲は10～60000ms。ValidateSettings()メソッドで検証されます。
        /// </remarks>
        public int UpdateInterval
        {
            get => updateInterval;
            set
            {
                if (updateInterval != value)
                {
                    updateInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// ミリ秒表示の有効/無効
        /// </summary>
        /// <remarks>
        /// trueの場合、自動的に".fff"をフォーマットに追加します。
        /// 変更時にプレビューを自動更新します。
        /// </remarks>
        public bool ShowMilliseconds
        {
            get => showMilliseconds;
            set
            {
                if (showMilliseconds != value)
                {
                    showMilliseconds = value;
                    OnPropertyChanged();
                    UpdatePreview();
                }
            }
        }

        /// <summary>
        /// 時刻の前に表示するプレフィックステキスト
        /// </summary>
        /// <remarks>
        /// 変更時にプレビューを自動更新します。
        /// </remarks>
        public string PrefixText
        {
            get => prefixText;
            set
            {
                if (prefixText != value)
                {
                    prefixText = value;
                    OnPropertyChanged();
                    UpdatePreview();
                }
            }
        }

        /// <summary>
        /// ステータスバー時計の有効/無効
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 現在の設定でのプレビュー表示文字列
        /// </summary>
        /// <remarks>
        /// 100msごとに自動更新され、リアルタイムで時刻表示を確認できます。
        /// </remarks>
        public string Preview
        {
            get => preview;
            private set
            {
                if (preview != value)
                {
                    preview = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 検証エラーメッセージ
        /// </summary>
        /// <remarks>
        /// フォーマット文字列が無効な場合や、設定値が範囲外の場合にメッセージが設定されます。
        /// </remarks>
        public string ValidationMessage
        {
            get => validationMessage;
            private set
            {
                if (validationMessage != value)
                {
                    validationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 検証エラーの有無を示すフラグ
        /// </summary>
        /// <remarks>
        /// trueの場合、UIでエラー表示を行います。
        /// </remarks>
        public bool HasValidationError
        {
            get => hasValidationError;
            private set
            {
                if (hasValidationError != value)
                {
                    hasValidationError = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// プレビュー表示を更新します
        /// </summary>
        /// <remarks>
        /// 現在のフォーマット設定で日時を表示し、フォーマットエラーがあれば検証メッセージを設定します。
        /// 100msタイマーから定期的に呼び出されます。
        /// </remarks>
        private void UpdatePreview()
        {
            try
            {
                string format = GetEffectiveFormat();
                Preview = PrefixText + DateTime.Now.ToString(format);
                ValidationMessage = "";
                HasValidationError = false;
            }
            catch (FormatException ex)
            {
                Preview = "Invalid format";
                ValidationMessage = $"Format error: {ex.Message}";
                HasValidationError = true;
            }
            catch (Exception ex)
            {
                Preview = "Error";
                ValidationMessage = $"Error: {ex.Message}";
                HasValidationError = true;
            }
        }

        /// <summary>
        /// ミリ秒表示を含む最終的なフォーマット文字列を取得します
        /// </summary>
        /// <returns>ShowMillisecondsの設定を反映したフォーマット文字列</returns>
        /// <remarks>
        /// ShowMillisecondsがtrueで、フォーマットに".f"、".ff"、".fff"が含まれていない場合、自動的に".fff"を追加します。
        /// </remarks>
        public string GetEffectiveFormat()
        {
            if (ShowMilliseconds && !DateTimeFormat.Contains(".fff") && !DateTimeFormat.Contains(".ff") && !DateTimeFormat.Contains(".f"))
            {
                return DateTimeFormat + ".fff";
            }
            return DateTimeFormat;
        }

        /// <summary>
        /// 設定値の妥当性を検証します
        /// </summary>
        /// <returns>すべての設定が有効な場合true、それ以外はfalse</returns>
        /// <remarks>
        /// 更新間隔が10～60000msの範囲内であることと、フォーマット文字列が有効であることを検証します。
        /// エラーがある場合はValidationMessageとHasValidationErrorを設定します。
        /// </remarks>
        public bool ValidateSettings()
        {
            // Validate update interval
            if (UpdateInterval < 10 || UpdateInterval > 60000)
            {
                ValidationMessage = "Update interval must be between 10 and 60000 milliseconds.";
                HasValidationError = true;
                return false;
            }

            // Validate format string
            try
            {
                DateTime.Now.ToString(GetEffectiveFormat());
                ValidationMessage = "";
                HasValidationError = false;
                return true;
            }
            catch (FormatException ex)
            {
                ValidationMessage = $"Invalid date/time format: {ex.Message}";
                HasValidationError = true;
                return false;
            }
        }

        /// <summary>
        /// プロパティ変更イベントを発生させます
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名。省略時は呼び出し元のメンバー名が使用されます</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
