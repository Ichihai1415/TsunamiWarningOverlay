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
            public bool Enable_DisplayTime { get; set; } = true;

            /// <summary>
            /// 表示点滅を有効にするか
            /// </summary>
            public bool Enable_ViewChange { get; set; } = true;

            /// <summary>
            /// 表示点滅の間隔
            /// </summary>
            public int ViewChangeSpan { get; set; } = 2000;
        }
    }
}
