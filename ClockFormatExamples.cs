using System;
using System.Collections.Generic;

namespace StatusBarClock
{
    /// <summary>
    /// 日付時刻フォーマットの例を提供する静的クラス
    /// </summary>
    /// <remarks>
    /// よく使用される日付時刻フォーマットのプリセットと、フォーマット指定子のヘルプ情報を提供します。
    /// </remarks>
    public static class ClockFormatExamples
    {
        /// <summary>
        /// 事前定義された日付時刻フォーマットの辞書
        /// </summary>
        /// <remarks>
        /// 11種類の一般的なフォーマットパターンとその出力例を含みます。
        /// キーはフォーマット文字列、値は出力例です。
        /// </remarks>
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

        /// <summary>
        /// 指定されたフォーマットで現在時刻を文字列に変換します
        /// </summary>
        /// <param name="format">日付時刻フォーマット文字列</param>
        /// <returns>フォーマットされた現在時刻。無効なフォーマットの場合は"Invalid format"</returns>
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

        /// <summary>
        /// 日付時刻フォーマット指定子のヘルプ文字列を取得します
        /// </summary>
        /// <returns>一般的なフォーマット指定子とその説明、使用例を含む複数行の文字列</returns>
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
yyyy-MM-dd (dddd) HH:mm:ss     �� 2024-01-15 (Monday) 14:30:45
yyyy-MM-dd HH:mm:ss.fff        �� 2024-01-15 14:30:45.123
MM/dd/yyyy hh:mm:ss tt         �� 01/15/2024 02:30:45 PM
HH:mm:ss                       �� 14:30:45";
        }
    }
}
