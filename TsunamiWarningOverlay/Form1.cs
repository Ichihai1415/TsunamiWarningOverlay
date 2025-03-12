using System.Drawing.Drawing2D;
using System.Text.Json.Nodes;
using TsunamiWarningOverlay.Properties;
using static TsunamiWarningOverlay.Utils;

namespace TsunamiWarningOverlay
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pen_border.LineJoin = LineJoin.Round;
            pen_Forecast.LineJoin = LineJoin.Round;
            pen_Watch.LineJoin = LineJoin.Round;
            pen_Warning.LineJoin = LineJoin.Round;
            pen_MajorWarning.LineJoin = LineJoin.Round;
            img_noData = DrawData(new Data());
        }

        internal static Pen pen_border = new(Color.Gray, 1);
        internal static Pen pen_Forecast = new(Color.Blue, 3);
        internal static Pen pen_Watch = new(Color.Yellow, 5);
        internal static Pen pen_Warning = new(Color.Red, 7);
        internal static Pen pen_MajorWarning = new(Color.Purple, 9);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        internal Bitmap img_noData = new(600, 600);
        internal Bitmap img_main = new(600, 600);


        internal static Bitmap DrawData(Data data)
        {
            var bitmap = new Bitmap(600, 600);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Green);

            //最初に置いとくと若干いいかもしれない
            using var gp_MajorWarning = new GraphicsPath();
            using var gp_Warning = new GraphicsPath();
            using var gp_Watch = new GraphicsPath();
            using var gp_Forecast = new GraphicsPath();
            using var gp_border = new GraphicsPath();//これは後

            var json_tsunami = JsonNode.Parse(Resources.AreaTsunami_GIS_20240520_01) ?? throw new Exception("マップデータの読み込みに失敗しました。");
            foreach (var json_1 in json_tsunami["features"]!.AsArray())
            {
                var json_1_geo = json_1!["geometry"]!;
                if (json_1_geo["coordinates"]!.AsArray().Count == 0)//怪しいかも
                    continue;
                var isArea = data.AreaDatas.TryGetValue((string)json_1_geo["properties"]!["name"]!, out TsunamiGrade grade);
                var points = new List<Point>();
                if ((string)json_1_geo["type"]! == "LineString")
                    foreach (var json_2 in json_1_geo["coordinates"]!.AsArray())
                        points.Add(new Point((int)(((double)json_2![0]! - 120) * 20), (int)((50 - (double)json_2[1]!) * 20)));
                else
                    foreach (var json_2 in json_1_geo["coordinates"]!.AsArray())
                        foreach (var json_3 in json_2![0]!.AsArray())
                            points.Add(new Point((int)(((double)json_3![0]! - 120) * 20), (int)((50 - (double)json_3[1]!) * 20)));

                if (points.Count < 2)
                    continue;
                if (isArea)
                    switch (grade)
                    {
                        case TsunamiGrade.Forecast:
                            gp_Forecast.StartFigure();
                            gp_Forecast.AddLines(points.ToArray());
                            break;
                        case TsunamiGrade.Watch:
                            gp_Watch.StartFigure();
                            gp_Watch.AddLines(points.ToArray());
                            break;
                        case TsunamiGrade.Warning:
                            gp_Warning.StartFigure();
                            gp_Warning.AddLines(points.ToArray());
                            break;
                        case TsunamiGrade.MajorWarning:
                            gp_MajorWarning.StartFigure();
                            gp_MajorWarning.AddLines(points.ToArray());
                            break;
                    }
                gp_border.StartFigure();
                gp_border.AddLines(points.ToArray());

            }

            g.DrawPath(pen_Forecast, gp_Forecast);
            g.DrawPath(pen_Watch, gp_Watch);
            g.DrawPath(pen_Warning, gp_Warning);
            g.DrawPath(pen_MajorWarning, gp_MajorWarning);

            var json_map = JsonNode.Parse(Resources.AreaForecastEEW_GIS_20190125_01) ?? throw new Exception("マップデータの読み込みに失敗しました。");
            using var gp_map = new GraphicsPath();
            foreach (var json_1 in json_map["features"]!.AsArray())
            {
                var points = Array.Empty<Point>();
                if ((string?)json_1!["geometry"]!["type"] == "Polygon")
                    points = json_1["geometry"]!["coordinates"]![0]!.AsArray().Select(json_2 => new Point((int)(((double)json_2![0]! - 120) * 20), (int)((50 - (double)json_2[1]!) * 20))).ToArray();
                else
                    foreach (var json_2 in json_1["geometry"]!["coordinates"]!.AsArray())
                        points = json_2![0]!.AsArray().Select(json_3 => new Point((int)(((double)json_3![0]! - 120) * 20), (int)((50 - (double)json_3[1]!) * 20))).ToArray();
                if (points.Length > 2)
                    gp_map.AddPolygon(points);
            }
            g.FillPath(new SolidBrush(Color.FromArgb(0, 255, 0)), gp_map);

            g.DrawPath(pen_border, gp_border);

            return bitmap;
        }


        internal static Data P2PQ_Json2Data(string jsonText)
        {
            var json = JsonNode.Parse(jsonText) ?? throw new Exception("データの解析に失敗しました。");
            var areaDataList = new Dictionary<string, TsunamiGrade>();
            foreach (var area in json["areas"]!.AsArray())
                areaDataList.Add((string)area!["name"]!,
                    (string)area!["grade"]! switch
                    {//[ MajorWarning (大津波警報), Warning (津波警報), Watch (津波注意報), Unknown (不明) ]
                        "Watch" => TsunamiGrade.Watch,
                        "Warning" => TsunamiGrade.Warning,
                        "MajorWarning" => TsunamiGrade.MajorWarning,
                        _ => TsunamiGrade.None,
                    });
            return new Data
            {
                ReceiveTime = DateTime.Now,
                AnnouncementTime = DateTime.Parse(json["issue"]!["time"]!.ToString()),
                AreaDatas = areaDataList
            };
        }


        private void Ti_ViewChange_Tick(object sender, EventArgs e)
        {

        }
    }
}


