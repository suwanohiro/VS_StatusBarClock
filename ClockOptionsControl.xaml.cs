using System.Windows.Controls;

namespace StatusBarClock
{
    /// <summary>
    /// 時計オプション設定画面のUserControl
    /// </summary>
    /// <remarks>
    /// XAMLで定義されたUIコンポーネントのコードビハインドです。
    /// DataContextとしてClockOptionsViewModelを使用します。
    /// </remarks>
    public partial class ClockOptionsControl : UserControl
    {
        /// <summary>
        /// ClockOptionsControlの新しいインスタンスを初期化します
        /// </summary>
        public ClockOptionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataContextをClockOptionsViewModelとして取得または設定します
        /// </summary>
        internal ClockOptionsViewModel ViewModel
        {
            get { return DataContext as ClockOptionsViewModel; }
            set { DataContext = value; }
        }
    }
}
