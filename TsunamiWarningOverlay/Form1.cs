using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Text.Json.Nodes;
using TsunamiWarningOverlay.Properties;
using static TsunamiWarningOverlay.Utils;

namespace TsunamiWarningOverlay
{
    public partial class Form1 : Form
    {
        internal static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true, Converters = { new Utils.ColorConverter() } };

        /// <summary>
        /// ������������(1)
        /// </summary>
        public Form1()
        {
            config = File.Exists("config.json") ? JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"), jsonOptions)! : new Config();
            File.WriteAllText("config.json", JsonSerializer.Serialize(config, jsonOptions));
            img_noData = new Bitmap(config.WindowSize, config.WindowSize);
            img_main = new Bitmap(config.WindowSize, config.WindowSize);
            pen_border = new(config.Color_Foreground, 1) { LineJoin = LineJoin.Round };
            brush_foreground = new(config.Color_Foreground);
            brush_background = new(config.Color_Background);
            brush_mapFill = new(config.Color_MapFill);

            InitializeComponent();
        }

        /// <summary>
        /// �ݒ�
        /// </summary>
        internal static Config config = new();

        /// <summary>
        /// ���{�n�}�p�y��
        /// </summary>
        internal static Pen pen_border = new(config.Color_Foreground, 1) { LineJoin = LineJoin.Round };

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
        /// ���{���A�����F�u���V
        /// </summary>
        internal static SolidBrush brush_foreground = new(config.Color_Foreground);

        /// <summary>
        /// �w�i�F�u���V
        /// </summary>
        internal static SolidBrush brush_background = new(config.Color_Background);

        /// <summary>
        /// �}�b�v�h��Ԃ��F�u���V
        /// </summary>
        internal static SolidBrush brush_mapFill = new(config.Color_MapFill);

        /// <summary>
        /// �f�o�b�O��
        /// </summary>
        internal static bool debug = false;

        /// <summary>
        /// ������������(2)
        /// </summary>
        private async void Form1_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(config.WindowSize, config.WindowSize);
            img_noData = DrawData(new Data());
            La_Times.ForeColor = config.Color_Foreground;
            La_Times.BackColor = config.Color_Background;

            if (config.Enable_TopMostTransparent)
            {
                Ti_ViewChange.Interval = config.ViewChangeSpan;
                TopMost = true;
                FormBorderStyle = FormBorderStyle.None;
                TransparencyKey = config.Color_Background;
            }

            //normal

            await GetChangeP2PQData();


            //DEBUG

            //debug = true;
            //img_main = DrawData(P2PQ_Json2Data(File.ReadAllText("sample\\20240101-2-65926857f0f6de0007564895.json"))!);
            //img_main = DrawData(P2PQ_Json2Data(File.ReadAllText("sample\\20220116-2-61e30a7c02add671afd9648d.json"))!);
            //DisplayStart();
            //Ti_GetP2PQ.Enabled = false;
        }

        /// <summary>
        /// �f�[�^�Ȃ��̉摜
        /// </summary>
        internal static Bitmap img_noData = new(config.WindowSize, config.WindowSize);

        /// <summary>
        /// �f�[�^����̉摜
        /// </summary>
        internal static Bitmap img_main = new(config.WindowSize, config.WindowSize);

        /// <summary>
        /// �f�[�^��`�悵�܂��B
        /// </summary>
        /// <param name="data">�`�悷��f�[�^</param>
        /// <returns>�`�悳�ꂽ�摜</returns>
        /// <exception cref="Exception"></exception>
        internal Bitmap DrawData(Data data)
        {
            GC.Collect();
            var bitmap = new Bitmap(config.WindowSize, config.WindowSize);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(config.Color_Background);

            if (config.Enable_AntiAlias)
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            //�ŏ��ɒu���Ƃ��Ǝ኱������������Ȃ�
            using var gp_MajorWarning = new GraphicsPath();
            using var gp_Warning = new GraphicsPath();
            using var gp_Watch = new GraphicsPath();
            using var gp_Forecast = new GraphicsPath();
            using var gp_border = new GraphicsPath();//����͌�
            var z = config.WindowSize / 30f;

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
                        points.Add(new PointF(((float)json_2![0]! - 120f) * z, (50f - (float)json_2[1]!) * z));
                    pointsList.Add(points);
                }
                else
                    foreach (var json_2 in json_1_geo["coordinates"]!.AsArray())
                    {
                        var points = new List<PointF>();
                        foreach (var json_3 in json_2!.AsArray())
                            points.Add(new PointF(((float)json_3![0]! - 120f) * z, (50f - (float)json_3[1]!) * z));
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
                    points = [.. json_1["geometry"]!["coordinates"]![0]!.AsArray().Select(json_2 => new PointF(((float)json_2![0]! - 120f) * z, (50f - (float)json_2[1]!) * z))];
                    if (points.Length > 2)
                        gp_map.AddPolygon(points);
                }
                else
                    foreach (var json_2 in json_1["geometry"]!["coordinates"]!.AsArray())
                    {
                        points = [.. json_2![0]!.AsArray().Select(json_3 => new PointF(((float)json_3![0]! - 120f) * z, (50f - (float)json_3[1]!) * z))];
                        if (points.Length > 2)
                            gp_map.AddPolygon(points);
                    }
            }
            g.FillPath(brush_mapFill, gp_map);

            g.DrawPath(pen_border, gp_border);

            if (debug)
            {
                var centerTextSize = g.MeasureString("���݂̃f�[�^�ł͂���܂���", new Font("Yu Gothic UI", z * 2f, GraphicsUnit.Pixel));
                g.DrawString("���݂̃f�[�^�ł͂���܂���", new Font("Yu Gothic UI", z * 2f, GraphicsUnit.Pixel), brush_foreground, (config.WindowSize - centerTextSize.Width) / 2, (config.WindowSize - centerTextSize.Height) / 2);
            }

            g.DrawString("���n�}�f�[�^: �C�ے�", new Font("Yu Gothic UI", z, GraphicsUnit.Pixel), brush_foreground, 0, 0);

            if (config.Enable_DisplayTime)
            {
                La_Times.Text = $"{(data.AnnouncementTime == DateTime.MinValue ? "" : data.AnnouncementTime):yyMMddHHmm} {(data.ReceiveTime == DateTime.MinValue ? "" : data.ReceiveTime):yyMMddHHmm}";
                La_Times.Font = new Font("Yu Gothic UI", z / 2f);
                La_Times.Location = new Point(config.WindowSize - La_Times.Size.Width, config.WindowSize - La_Times.Size.Height);
            }

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
            try
            {
                var res = await client.GetAsync("https://api.p2pquake.net/v2/history?codes=552&limit=1");
                //DEBUG(sandbox)
                //var res = await client.GetAsync("https://api-v2-sandbox.p2pquake.net/v2/history?codes=552&limit=1&offset=1");
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
            }
            catch (Exception ex)
            {
                File.WriteAllText("log-error.txt", ex.ToString());
            }
            return;
        }

        /// <summary>
        /// �\�����J�n���܂��B
        /// </summary>
        private void DisplayStart()
        {
            Ti_ViewChange.Enabled = true;
            PB_Main.Image = img_main;
            isDisplayON = true;
        }

        /// <summary>
        /// �\�����I�����܂��B
        /// </summary>
        private void DisplayEnd()
        {
            Ti_ViewChange.Enabled = false;
            PB_Main.Image = null;
            isDisplayON = false;
        }

        /// <summary>
        /// �\�����[�h�ł��邩
        /// </summary>
        internal bool isDisplayON = false;

        /// <summary>
        /// �_�ŕ\���̐؂�ւ�
        /// </summary>
        private void Ti_ViewChange_Tick(object sender, EventArgs e)
        {
            if (!config.Enable_ViewChange)
                return;
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

        /// <summary>
        /// �}�E�X�N���b�N�ʒu
        /// </summary>
        internal static Point mousePoint;

        /// <summary>
        /// �ړ��J�n
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PB_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                mousePoint = new Point(e.X, e.Y);
        }

        /// <summary>
        /// �ړ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PB_Main_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Left += e.X - mousePoint.X;
                Top += e.Y - mousePoint.Y;
            }
        }

        /// <summary>
        /// �E�N���b�N���j���[ - �ċN���N���b�N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TSMI_reboot_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        /// <summary>
        /// �E�N���b�N���j���[ - �I���N���b�N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TSMI_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// �ړ��J�n
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void La_Times_MouseDown(object sender, MouseEventArgs e)
        {
            PB_Main_MouseDown(sender, e);
        }

        /// <summary>
        /// �ړ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void La_Times_MouseMove(object sender, MouseEventArgs e)
        {
            PB_Main_MouseMove(sender, e);
        }
    }
}
