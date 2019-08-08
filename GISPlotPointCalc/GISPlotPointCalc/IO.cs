using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Data;

namespace GISPlotPointCalc
{
    class IO
    {
        //获取当前时间
        internal static string DT()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        #region 点位读写
        //从文件读取点位信息
        internal static List<PlotPoint> LoadFromFile(string FilePath)
        {
            List<PlotPoint> Points = new List<PlotPoint>(); //新建点列表

            //新建XmlDocument对象，并从指定路径加载文件
            XmlDocument XD = new XmlDocument();
            XD.Load(FilePath);

            using (FileStream FS = new FileStream(FilePath, FileMode.Open))
            {
                XmlNodeList XNL = XD.GetElementsByTagName("Point"); //获取名称为Point的节点列表
                if (XNL.Count < 1)
                {
                    throw new Exception("点数过少！");
                }
                else
                {
                    //从列表中逐点添加
                    for (int i = 0; i < XNL.Count; i++)
                    {
                        Points.Add(new PlotPoint { X = Convert.ToSingle(XNL[i].ChildNodes[0].ChildNodes[0].Value), Y = Convert.ToSingle(XNL[i].ChildNodes[1].ChildNodes[0].Value) });   //至于这里为什么要取两次子节点，I don't know why it works, but it works...
                    }
                }
            }
            return Points;
        }

        //保存点位信息到文件
        internal static void SaveToFile(string FilePath, List<PlotPoint> Points)
        {
            try
            {
                Stream XmlStream = new MemoryStream();
                using (XmlTextWriter xw = new XmlTextWriter(FilePath, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    xw.WriteStartDocument(true);    //书写XML声明

                    //书写XML注释
                    xw.WriteComment("本文件存储折线点位信息，坐标系为控件坐标系。");
                    xw.WriteComment("保存时间" + DT());

                    xw.WriteStartElement("PolyLine");   //书写折线节点

                    //对XML文件逐节点写入
                    for (int i = 0; i < Points.Count; i++)
                    {
                        xw.WriteStartElement("Point");
                        xw.WriteElementString("x", Convert.ToString(Points[i].X));
                        xw.WriteElementString("y", Convert.ToString(Points[i].Y));
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }
        #endregion

        #region 配置文件读写
        //保存配置文件
        internal static void SaveSettings(string FilePath, TextBox WorkFolder, CheckBox Polygon, TextBox Ratio, TextBox Tolerance, PictureBox Point, PictureBox Line, PictureBox Font, PictureBox Fill, Label TextStyle, TrackBar PointT, TrackBar LineT, TrackBar FillT)
        {
            Stream XmlStream = new MemoryStream();
            using (XmlTextWriter xw = new XmlTextWriter(FilePath, Encoding.UTF8) { Formatting = Formatting.Indented })
            {
                xw.WriteStartDocument(true);    //书写XML声明

                //书写XML注释
                xw.WriteComment("本文件存储窗体配置信息。");
                xw.WriteComment("保存时间" + DT());

                xw.WriteStartElement("Settings");   //根节点

                //对XML文件逐节点写入
                xw.WriteElementString("WorkFolder", WorkFolder.Text); //工作文件夹

                //多边形绘制
                if (Polygon.Checked)
                {
                    xw.WriteElementString("Polygon", "true");
                }
                else
                {
                    xw.WriteElementString("Polygon", "false");
                }
                xw.WriteElementString("Ratio", Ratio.Text); //比例尺
                xw.WriteElementString("Tolerance", Tolerance.Text); //容差

                //颜色
                xw.WriteStartElement("Color");
                xw.WriteStartElement("Point");
                xw.WriteElementString("A", Point.BackColor.A.ToString());
                xw.WriteElementString("R", Point.BackColor.R.ToString());
                xw.WriteElementString("G", Point.BackColor.G.ToString());
                xw.WriteElementString("B", Point.BackColor.B.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("Line");
                xw.WriteElementString("A", Line.BackColor.A.ToString());
                xw.WriteElementString("R", Line.BackColor.R.ToString());
                xw.WriteElementString("G", Line.BackColor.G.ToString());
                xw.WriteElementString("B", Line.BackColor.B.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("Font");
                xw.WriteElementString("A", Font.BackColor.A.ToString());
                xw.WriteElementString("R", Font.BackColor.R.ToString());
                xw.WriteElementString("G", Font.BackColor.G.ToString());
                xw.WriteElementString("B", Font.BackColor.B.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("Fill");
                xw.WriteElementString("A", Fill.BackColor.A.ToString());
                xw.WriteElementString("R", Fill.BackColor.R.ToString());
                xw.WriteElementString("G", Fill.BackColor.G.ToString());
                xw.WriteElementString("B", Fill.BackColor.B.ToString());
                xw.WriteEndElement();
                xw.WriteEndElement();

                //大小及透明度
                xw.WriteStartElement("TrackBars");
                xw.WriteElementString("Point", PointT.Value.ToString());
                xw.WriteElementString("Line", LineT.Value.ToString());
                xw.WriteElementString("TransParency", FillT.Value.ToString());
                xw.WriteEndElement();

                //字体
                xw.WriteStartElement("TextStyle");
                xw.WriteElementString("Size", TextStyle.Font.Size.ToString());
                xw.WriteElementString("FontFamily", TextStyle.Font.Name);
                //xw.WriteElementString("FontFamily", (/*(Convert.ToString(*/TextStyle.Font.Name/*).Remove(0, 18)).Trim(']')).Replace("&amp","&"))*/));
                xw.WriteEndElement();
                xw.WriteEndElement();
            }
        }

        internal static void LoadSettings(string FilePath, ref TextBox WorkFolder, ref CheckBox Polygon, ref TextBox Ratio, ref TextBox Tolerance, ref PictureBox Point, ref PictureBox Line, ref PictureBox Font, ref PictureBox Fill, ref Label TextStyle, ref TrackBar PointT, ref TrackBar LineT, ref TrackBar FillT)
        {
            //新建XmlDocument对象，并从指定路径加载文件
            XmlDocument XD = new XmlDocument();
            XD.Load(FilePath);

            using (FileStream FS = new FileStream(FilePath, FileMode.Open))
            {
                //工作文件夹
                XmlNodeList XNL = XD.GetElementsByTagName("WorkFolder");
                if (XNL.Count == 1)
                {
                    WorkFolder.Text = XNL[0].ChildNodes[0].Value;
                }

                //多边形模式？
                XNL = XD.GetElementsByTagName("Polygon");
                if (XNL.Count == 1)
                {
                    Polygon.Checked = Convert.ToBoolean(XNL[0].ChildNodes[0].Value);
                }

                //比例尺
                XNL = XD.GetElementsByTagName("Ratio");
                if (XNL.Count == 1)
                {
                    Ratio.Text = XNL[0].ChildNodes[0].Value;
                }

                //容差
                XNL = XD.GetElementsByTagName("Tolerance");
                if (XNL.Count == 1)
                {
                    if (Convert.ToInt32(XNL[0].ChildNodes[0].Value) > 0 && Convert.ToInt32(XNL[0].ChildNodes[0].Value) < 100)
                    {
                        Tolerance.Text = XNL[0].ChildNodes[0].Value;
                    }
                }

                //颜色
                XNL = XD.GetElementsByTagName("Color");
                if (XNL.Count >= 1)
                {
                    Point.BackColor = Color.FromArgb(Convert.ToInt32(XNL[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[0].ChildNodes[1].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[0].ChildNodes[2].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[0].ChildNodes[3].ChildNodes[0].Value));
                    Line.BackColor = Color.FromArgb(Convert.ToInt32(XNL[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[1].ChildNodes[2].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[1].ChildNodes[3].ChildNodes[0].Value));
                    Font.BackColor = Color.FromArgb(Convert.ToInt32(XNL[0].ChildNodes[2].ChildNodes[0].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[2].ChildNodes[1].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[2].ChildNodes[2].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[2].ChildNodes[3].ChildNodes[0].Value));
                    Fill.BackColor = Color.FromArgb(Convert.ToInt32(XNL[0].ChildNodes[3].ChildNodes[0].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[3].ChildNodes[1].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[3].ChildNodes[2].ChildNodes[0].Value), Convert.ToInt32(XNL[0].ChildNodes[3].ChildNodes[3].ChildNodes[0].Value));
                }

                //大小
                XNL = XD.GetElementsByTagName("TrackBars");
                if (XNL.Count >= 1)
                {
                    PointT.Value = Convert.ToInt32(XNL[0].ChildNodes[0].ChildNodes[0].Value);
                    LineT.Value = Convert.ToInt32(XNL[0].ChildNodes[1].ChildNodes[0].Value);
                    FillT.Value = Convert.ToInt32(XNL[0].ChildNodes[2].ChildNodes[0].Value);
                }

                //字体
                XNL = XD.GetElementsByTagName("TextStyle");
                if (XNL.Count >= 1)
                {
                    TextStyle.Font = new Font(XNL[0].ChildNodes[1].ChildNodes[0].Value, Convert.ToSingle(XNL[0].ChildNodes[0].ChildNodes[0].Value));
                }
            }
        }
        #endregion

        //自定义光标
        internal static void LoadCursor()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CustomCur.cur";
            if (!File.Exists(FilePath))
            {
                System.Reflection.Assembly CurAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                var CurStream = CurAssembly.GetManifestResourceStream("GISPlotPointCalc.CustomCur.cur");
                byte[] CurResource = new Byte[CurStream.Length];
                CurStream.Read(CurResource, 0, (int)CurStream.Length);
                var CurFileStream = new FileStream(FilePath, FileMode.Create);
                CurFileStream.Write(CurResource, 0, (int)CurStream.Length);
                CurFileStream.Close();
            }
        }

        //保存图片
        internal static void Save2Pic(PictureBox TargetPictureBox, string FilePath)
        {
            Bitmap IntersectionPic = new Bitmap(TargetPictureBox.BackgroundImage);
            IntersectionPic.Save(FilePath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
