using System.Text.Json;
using System.Text.Json.Serialization;

namespace TsunamiWarningOverlay
{
    /// <summary>
    /// 内部クラス等
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// 津波予報のグレード
        /// </summary>
        public enum TsunamiGrade
        {
            /// <summary>
            /// (発表無し)
            /// </summary>
            None = 0,
            /// <summary>
            /// 津波予報(jma xmlのみ)
            /// </summary>
            Forecast = 1,
            /// <summary>
            /// 津波注意報
            /// </summary>
            Watch = 2,
            /// <summary>
            /// 津波警報
            /// </summary>
            Warning = 3,
            /// <summary>
            /// 大津波警報
            /// </summary>
            MajorWarning = 4
        }

        /// <summary>
        /// データ格納クラス
        /// </summary>
        public class Data
        {
            /// <summary>
            /// 受信時刻(内部変換時刻)
            /// </summary>
            public DateTime ReceiveTime { get; set; } = DateTime.MinValue;

            /// <summary>
            /// 発表時刻
            /// </summary>
            public DateTime AnnouncementTime { get; set; } = DateTime.MinValue;

            /// <summary>
            /// エリア名とグレード
            /// </summary>
            public Dictionary<string, TsunamiGrade> AreaDatas { get; set; } = [];
        }

        /// <summary>
        /// 設定格納クラス
        /// </summary>
        public class Config
        {
            /// <summary>
            /// アンチエイリアスを有効にするか
            /// </summary>
            public bool Enable_AntiAlias { get; set; } = true;

            /// <summary>
            /// 発表・受信時刻表示を有効にするか
            /// </summary>
            public bool Enable_DisplayTime { get; set; } = false;

            /// <summary>
            /// 表示点滅を有効にするか
            /// </summary>
            public bool Enable_ViewChange { get; set; } = true;

            /// <summary>
            /// 表示点滅の間隔
            /// </summary>
            public int ViewChangeSpan { get; set; } = 2000;

            /// <summary>
            /// ウィンドウコントロールサイズ
            /// </summary>
            public int WindowSize { get; set; } = 600;

            /// <summary>
            /// 最前面に背景色を透明化して表示するか
            /// </summary>
            public bool Enable_TopMostTransparent { get; set; } = false;

            /// <summary>
            /// 日本線、文字色
            /// </summary>
            public Color Color_Foreground { get; set; } = Color.FromArgb(255, 255, 255);

            /// <summary>
            /// 背景色
            /// </summary>
            public Color Color_Background { get; set; } = Color.FromArgb(0, 0, 0);

            /// <summary>
            /// 地図売りつぶし色
            /// </summary>
            public Color Color_MapFill { get; set; } = Color.FromArgb(127, 127, 127);
        }

        /// <summary>
        /// ColorをJSONシリアライズ/デシアライズできるようにします。
        /// </summary>
        public class ColorConverter : JsonConverter<Color>
        {
            /// <inheritdoc/>
            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var colorString = reader.GetString() ?? throw new ArgumentException("値が正しくありません。");
                var argbValues = colorString.Split(',');
                if (argbValues.Length == 3)
                    return Color.FromArgb(int.Parse(argbValues[0]), int.Parse(argbValues[1]), int.Parse(argbValues[2]));
                //else if (argbValues.Length == 4)//このソフトでは透明値は無意味のためなし
                //    return Color.FromArgb(int.Parse(argbValues[0]), int.Parse(argbValues[1]), int.Parse(argbValues[2]), int.Parse(argbValues[3]));
                else
                    throw new ArgumentException("値が正しくありません。");
            }

            /// <inheritdoc/>
            public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"{value.R},{value.G},{value.B}");//{value.A}, このソフトでは透明値は無意味のためなし
            }
        }
    }
}
