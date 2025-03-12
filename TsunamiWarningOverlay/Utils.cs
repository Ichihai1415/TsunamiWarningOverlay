namespace TsunamiWarningOverlay
{
    internal static class Utils
    {
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





        public class Data
        {
            public DateTime ReceiveTime { get; set; } = DateTime.MinValue;
            public DateTime AnnouncementTime { get; set; } = DateTime.MinValue;

            public Dictionary<string, TsunamiGrade> AreaDatas { get; set; } = [];

            /*
            public AreaData[] AreaDatas { get; set; } = []; 

            public class AreaData
            {
                public string AreaName { get; set; } = string.Empty;

                public TsunamiGrade Grade { get; set; }

            }*/


        }

        public class Config
        {

        }
    }
}
