using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GISPlotPointCalc
{
    class PolyLineDrawing
    {
        //总绘制方法
        internal static void Draw(List<PlotPoint> Points, PictureBox TargetPictureBox, Color PointColor, Color LineColor, Color FontColor, Font FontStyle, int LineWidth, int PointWidth)
        {
            DrawLines(Points, TargetPictureBox, LineColor, LineWidth);
            for (int i = 0; i < Points.Count; i++)
            {
                DrawPoints(Points[i], TargetPictureBox, PointColor, PointWidth);
                DrawFonts(Points[i], Convert.ToString(i + 1), TargetPictureBox, FontColor, FontStyle, PointWidth);
            }
            TargetPictureBox.Refresh();
        }

        //多边形绘制方法
        internal static void DrawPolygon(List<PlotPoint> Points, PictureBox TargetPictureBox, Color PolygonColor, int PolygonTransparency)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush PolygonBrush = new SolidBrush(Color.FromArgb(PolygonTransparency, PolygonColor.R, PolygonColor.G, PolygonColor.B));
 
            List<PointF> PolygonPoints = new List<PointF>();
            for (int i = 0; i < Points.Count; i++)
            {
                PolygonPoints.Add(new PointF { X = Points[i].X, Y = Points[i].Y });
            }
            g.FillPolygon(PolygonBrush, PolygonPoints.ToArray());
        }

        //预览多边形绘制
        internal static void DrawPolygon(List<PlotPoint> Points, PictureBox TargetPictureBox, Color PolygonColor, int PolygonTransparency, PlotPoint PreviewingPoint)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush PolygonBrush = new SolidBrush(Color.FromArgb(PolygonTransparency, PolygonColor.R, PolygonColor.G, PolygonColor.B));         

            List<PointF> PolygonPoints = new List<PointF>();
            for (int i = 0; i < Points.Count; i++)
            {
                PolygonPoints.Add(new PointF { X = Points[i].X, Y = Points[i].Y });
            }
            PolygonPoints.Add(new PointF { X = PreviewingPoint.X, Y = PreviewingPoint.Y });
            g.FillPolygon(PolygonBrush, PolygonPoints.ToArray());
        }

        //线绘制方法
        internal static void DrawLines(List<PlotPoint> Points, PictureBox TargetPictureBox, Color LineColor, int LineWidth)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen DrawLines = new Pen(LineColor, LineWidth);
            if (Points.Count > 1)
            {
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    g.DrawLine(DrawLines, Points[i].X, Points[i].Y, Points[i + 1].X, Points[i + 1].Y);
                }
            }
        }
        internal static void DrawLines(PlotPoint Point1, PlotPoint Point2, PictureBox TargetPictureBox, Color LineColor, int LineWidth)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen DrawLines = new Pen(LineColor, LineWidth);
            g.DrawLine(DrawLines, Point1.X, Point1.Y, Point2.X, Point2.Y);
        }


        //预览线绘制方法
        internal static void DrawPreviewLine(PlotPoint StartPoint, PlotPoint EndPoint, PictureBox TargetPictureBox, Color LineColor, int LineWidth)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen DrawLines = new Pen(LineColor, LineWidth)
            {
                //预览线虚线样式
                DashStyle = DashStyle.Custom,
                DashPattern = new float[] { 2, 2 }
            };
            g.DrawLine(DrawLines, StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
        }


        //点绘制方法
        internal static void DrawPoints(PlotPoint Points, PictureBox TargetPictureBox, Color PointColor, int PointWidth)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush PointBrush = new SolidBrush(PointColor);
            g.FillEllipse(PointBrush, Points.X - PointWidth / 2, Points.Y - PointWidth / 2, PointWidth, PointWidth);
        }

        //文字绘制方法
        internal static void DrawFonts(PlotPoint Points, string No, PictureBox TargetPictureBox, Color FontColor, Font FontStyle, int PointWidth)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Font PointStyle = FontStyle;
            SolidBrush PointBrush = new SolidBrush(FontColor);
            PointF FontPos = new PointF
            {
                X = Convert.ToSingle(Points.X - FontStyle.Size / 2 + FontStyle.Size * -Math.Cos(Math.Atan2(Points.Y - TargetPictureBox.Height / 2, Points.X - TargetPictureBox.Width / 2))),
                Y = Convert.ToSingle(Points.Y - FontStyle.Size / 2 + FontStyle.Size * -Math.Sin(Math.Atan2(Points.Y - TargetPictureBox.Height / 2, Points.X - TargetPictureBox.Width / 2)))
            };
            g.DrawString(No, PointStyle, PointBrush, FontPos.X, FontPos.Y);
        }

        //清空图片框
        internal static void ClearPictureBox(PictureBox TargetPictureBox)
        {
            Graphics g = Graphics.FromImage(TargetPictureBox.BackgroundImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //白色底色清空
            Brush ClearBrush = new SolidBrush(Color.White);
            Rectangle Clear = new Rectangle(0, 0, TargetPictureBox.Width, TargetPictureBox.Height);
            g.FillRectangle(ClearBrush, Clear);
            float Width = TargetPictureBox.Width;
            float Height = TargetPictureBox.Height;

            //坐标轴样式
            Color LineColor = Color.Gray;
            Pen DrawLines = new Pen(LineColor, 1.0f);

            //画x轴
            g.DrawLine(DrawLines, Width * 3 / 80, Height / 2, Width * 77 / 80, Height / 2);
            g.DrawLine(DrawLines, Width * 77 / 80, Height / 2, Width * 19 / 20, Height * 39 / 80);
            g.DrawLine(DrawLines, Width * 77 / 80, Height / 2, Width * 19 / 20, Height * 41 / 80);

            //画y轴
            g.DrawLine(DrawLines, Width / 2, Height * 3 / 80, Width / 2, Height * 77 / 80);
            g.DrawLine(DrawLines, Width / 2, Height * 3 / 80, Width * 39 / 80, Height / 20);
            g.DrawLine(DrawLines, Width / 2, Height * 3 / 80, Width * 41 / 80, Height / 20);

            //坐标轴文字
            Font FontStyle = new Font("Times New Roman", 9);
            SolidBrush FontBrush = new SolidBrush(Color.Gray);
            g.DrawString("O", FontStyle, FontBrush, Width * 41 / 80, Height * 41 / 80);
            g.DrawString("x", FontStyle, FontBrush, Width * 37 / 40, Height / 2);
            g.DrawString("y", FontStyle, FontBrush, Width * 19 / 40, Height / 20);

            TargetPictureBox.Refresh();
        }
    }
}
