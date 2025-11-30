using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StatusBarClock
{
    /// <summary>
    /// bool値とWPF Visibility列挙値を相互変換するIValueConverter実装
    /// </summary>
    /// <remarks>
    /// XAMLバインディングで使用され、bool値に基づいてUI要素の表示/非表示を制御します。
    /// </remarks>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// bool値をVisibility値に変換します
        /// </summary>
        /// <param name="value">変換元のbool値</param>
        /// <param name="targetType">変換先の型</param>
        /// <param name="parameter">バインディングパラメータ（未使用）</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>trueの場合Visibility.Visible、falseまたは非bool値の場合Visibility.Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Visibility値をbool値に逆変換します
        /// </summary>
        /// <param name="value">変換元のVisibility値</param>
        /// <param name="targetType">変換先の型</param>
        /// <param name="parameter">バインディングパラメータ（未使用）</param>
        /// <param name="culture">カルチャ情報</param>
        /// <returns>Visibility.Visibleの場合true、それ以外のVisibility値または非Visibility値の場合false</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
