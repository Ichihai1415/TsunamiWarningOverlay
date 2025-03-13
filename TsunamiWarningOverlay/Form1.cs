using System.Drawing.Drawing2D;
using System.Text.Json.Nodes;
using TsunamiWarningOverlay.Properties;
using static TsunamiWarningOverlay.Utils;

namespace TsunamiWarningOverlay
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// ������������(1)
        /// </summary>
        public Form1()
        {
            InitializeComponent();





            //BackColor = backColor;
        }

        /// <summary>
        /// ���{�n�}�p�y��
        /// </summary>
        internal static Pen pen_border = new(Color.White, 1) { LineJoin = LineJoin.Round };

        /// <summary>
        /// �Ôg�\��p�y��
        /// </summary>
        internal static Pen pen_Forecast = new(Color.Blue, 3) { LineJoin = LineJoin.Round };

        /// <summary>
        /// �Ôg���ӕ�p�y��
        /// </summary>
        internal static Pen pen_Watch = new(Color.Yellow, 5) { LineJoin = LineJoin.Round };

        /// <summary>
        /// �Ôg�x��p�y��
        /// </summary>
        internal static Pen pen_Warning = new(Color.Red, 7) { LineJoin = LineJoin.Round };

        /// <summary>
        /// ��Ôg�x��p�y��
        /// </summary>
        internal static Pen pen_MajorWarning = new(Color.Purple, 9) { LineJoin = LineJoin.Round };

        /// <summary>
        /// ������������(2)
        /// </summary>
        private async void Form1_Load(object sender, EventArgs e)
        {
            img_noData = DrawData(new Data());

            await GetChangeP2PQData();
                

            //DEBUG

            //img_main = DrawData(P2PQ_Json2Data(File.ReadAllText("sample\\20240101-2-65926857f0f6de0007564895.json"))!);
            //img_main = DrawData(P2PQ_Json2Data(File.ReadAllText("sample\\20220116-2-61e30a7c02add671afd9648d.json"))!);
            //DisplayStart();
        }

        /// <summary>
        /// �f�[�^�Ȃ��̉摜
        /// </summary>
        internal static Bitmap img_noData = new(600, 600);

        /// <summary>
        /// �f�[�^����̉摜
        /// </summary>
        internal static Bitmap img_main = new(600, 600);

        /// <summary>
        /// ���{���F�E�����F(���g�p)
        /// </summary>
        internal static Color foreColor = Color.FromArgb(255,255,255);

        /// <summary>
        /// �w�i�F
        /// </summary>
        internal static Color backColor = Color.FromArgb(0, 0, 0);

        /// <summary>
        /// �f�[�^��`�悵�܂��B
        /// </summary>
        /// <param name="data">�`�悷��f�[�^</param>
        /// <returns>�`�悳�ꂽ�摜</returns>
        /// <exception cref="Exception"></exception>
        internal Bitmap DrawData(Data data)
        {
            GC.Collect();
            var bitmap = new Bitmap(600, 600);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(backColor);
            

            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            //�ŏ��ɒu���Ƃ��Ǝ኱������������Ȃ�
            using var gp_MajorWarning = new GraphicsPath();
            using var gp_Warning = new GraphicsPath();
            using var gp_Watch = new GraphicsPath();
            using var gp_Forecast = new GraphicsPath();
            using var gp_border = new GraphicsPath();//����͌�

            var json_tsunami = JsonNode.Parse(Resources.AreaTsunami_GIS_20240520_01) ?? throw new Exception("�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B");
            foreach (var json_1 in json_tsunami["features"]!.AsArray())
            {
                var json_1_geo = json_1!["geometry"]!;
                if (json_1_geo == null)
                    continue;
                var isArea = data.AreaDatas.TryGetValue((string)json_1["properties"]!["name"]!, out TsunamiGrade grade);
                var pointsList = new List<List<PointF>>();
                if ((string)json_1_geo["type"]! == "LineString")
                {
                    var points = new List<PointF>();
                    foreach (var json_2 in json_1_geo["coordinates"]!.AsArray())
                        points.Add(new PointF(((float)json_2![0]! - 120f) * 20f, (50f - (float)json_2[1]!) * 20f));
                    pointsList.Add(points);
                }
                else
                    foreach (var json_2 in json_1_geo["coordinates"]!.AsArray())
                    {
                        var points = new List<PointF>();
                        foreach (var json_3 in json_2!.AsArray())
                            points.Add(new PointF(((float)json_3![0]! - 120f) * 20f, (50f - (float)json_3[1]!) * 20f));
                        pointsList.Add(points);
                    }

                foreach (var points in pointsList)
                {
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
            }

            g.DrawPath(pen_Forecast, gp_Forecast);
            g.DrawPath(pen_Watch, gp_Watch);
            g.DrawPath(pen_Warning, gp_Warning);
            g.DrawPath(pen_MajorWarning, gp_MajorWarning);

            var json_map = JsonNode.Parse(Resources.AreaForecastEEW_GIS_20190125_01) ?? throw new Exception("�}�b�v�f�[�^�̓ǂݍ��݂Ɏ��s���܂����B");
            using var gp_map = new GraphicsPath();
            foreach (var json_1 in json_map["features"]!.AsArray())
            {
                var points = Array.Empty<PointF>();
                if ((string?)json_1!["geometry"]!["type"] == "Polygon")
                {
                    points = json_1["geometry"]!["coordinates"]![0]!.AsArray().Select(json_2 => new PointF(((float)json_2![0]! - 120f) * 20f, (50f - (float)json_2[1]!) * 20f)).ToArray();
                    if (points.Length > 2)
                        gp_map.AddPolygon(points);
                }
                else
                    foreach (var json_2 in json_1["geometry"]!["coordinates"]!.AsArray())
                    {
                        points = json_2![0]!.AsArray().Select(json_3 => new PointF(((float)json_3![0]! - 120f) * 20f, (50f - (float)json_3[1]!) * 20f)).ToArray();
                        if (points.Length > 2)
                            gp_map.AddPolygon(points);
                    }
            }
            g.FillPath(new SolidBrush(backColor), gp_map);

            g.DrawPath(pen_border, gp_border);

            if (DateTime.Now - data.AnnouncementTime > TimeSpan.FromDays(1))
            {
                var centerTextSize = g.MeasureString("���݂̃f�[�^�ł͂���܂���", new Font("Yu Gothic UI", 30));
                g.DrawString("���݂̃f�[�^�ł͂���܂���", new Font("Yu Gothic UI", 30), Brushes.White, 300 - centerTextSize.Width / 2, 300 - centerTextSize.Height / 2);
            }

            g.DrawString("�n�}�f�[�^: �C�ے�", new Font("Yu Gothic UI", 14), Brushes.White, 0, 0);


            La_Times.Text = $"{(data.AnnouncementTime == DateTime.MinValue ? "" : data.AnnouncementTime):yyMMddHHmm} {(data.ReceiveTime == DateTime.MinValue ? "" : data.ReceiveTime):yyMMddHHmm}";
            La_Times.Location = new Point(600 - La_Times.Size.Width, 600 - La_Times.Size.Height);

            return bitmap;
        }

        /// <summary>
        /// P2P�n�k���JSON API v2 �̃f�[�^��<see cref="Data"/>�ɕϊ����܂��B
        /// </summary>
        /// <param name="jsonText">JSON������</param>
        /// <returns>�f�[�^���L���ȏꍇ�ϊ����ꂽ<see cref="Data"/>�A����ȊO��null</returns>
        /// <exception cref="Exception"></exception>
        internal static Data? P2PQ_Json2Data(string jsonText)
        {
            var json = JsonNode.Parse(jsonText) ?? throw new Exception("�f�[�^�̉�͂Ɏ��s���܂����B");
            var json_data = jsonText.StartsWith('[') ? json.AsArray()[0] : json;
            if (json_data == null) return null;
            if ((bool)json_data["cancelled"]!) return null;

            var areaDataList = new Dictionary<string, TsunamiGrade>();
            foreach (var area in json_data["areas"]!.AsArray())
                areaDataList.Add((string)area!["name"]!,
                    (string)area!["grade"]! switch
                    {//[ MajorWarning (��Ôg�x��), Warning (�Ôg�x��), Watch (�Ôg���ӕ�), Unknown (�s��) ]
                        "Watch" => TsunamiGrade.Watch,
                        "Warning" => TsunamiGrade.Warning,
                        "MajorWarning" => TsunamiGrade.MajorWarning,
                        _ => TsunamiGrade.None,
                    });
            return new Data
            {
                ReceiveTime = DateTime.Now,
                AnnouncementTime = DateTime.Parse(json_data["issue"]!["time"]!.ToString()),
                AreaDatas = areaDataList
            };
        }

        /// <summary>
        /// HttpClient
        /// </summary>
        internal static readonly HttpClient client = new();

        /// <summary>
        /// �ŏIP2P�n�k���JSON API v2 ��JSON������
        /// </summary>
        internal static string lastP2PQ_St = "";

        /// <summary>
        /// P2P�n�k���JSON API v2 �̃f�[�^���擾���\�����܂��B
        /// </summary>
        internal async Task GetChangeP2PQData()
        {
            var res = await client.GetAsync("https://api.p2pquake.net/v2/history?codes=552&limit=1");
            //var res = await client.GetAsync("https://api-v2-sandbox.p2pquake.net/v2/history?codes=552&limit=1&offset=1");//sandbox�e�X�g
            if (res == null) return;//��{�Ȃ�
            var resSt = await res.Content.ReadAsStringAsync();
            if (resSt == null) return;//��{�Ȃ�

            if (resSt == lastP2PQ_St) return;//�ω��Ȃ��̂Ƃ�
            lastP2PQ_St = resSt;

            if (resSt == "[]")//�ω����肩�f�[�^�Ȃ���
            {
                DisplayEnd();
                return;
            }
            var data = P2PQ_Json2Data(resSt);
            if (data == null)//�ω����肩�L���f�[�^�Ȃ�(��������)
            {
                DisplayEnd();
                return;
            }

            img_main = DrawData(data);//�ω����肩�؂�ւ�
            DisplayStart();
            return;
        }

        /// <summary>
        /// �\�����J�n���܂�
        /// </summary>
        private void DisplayStart()
        {
            Ti_ViewChange.Enabled = true;
        }

        /// <summary>
        /// �\�����I�����܂��B
        /// </summary>
        private void DisplayEnd()
        {
            Ti_ViewChange.Enabled = false;
            PB_Main.Image = null;
        }

        /// <summary>
        /// �_�łŕ\�����[�h�ł��邩
        /// </summary>
        internal bool isDisplayON = false;

        /// <summary>
        /// �_�ŕ\���̐؂�ւ�
        /// </summary>
        private void Ti_ViewChange_Tick(object sender, EventArgs e)
        {
            isDisplayON = !isDisplayON;
            if (isDisplayON)
                PB_Main.Image = img_main;
            else
                PB_Main.Image = img_noData;
        }

        /// <summary>
        /// �����擾
        /// </summary>
        private async void Ti_GetP2PQ_Tick(object sender, EventArgs e)
        {
            await GetChangeP2PQData();
        }
    }
}
