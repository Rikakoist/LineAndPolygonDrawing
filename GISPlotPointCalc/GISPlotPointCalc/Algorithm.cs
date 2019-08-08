using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISPlotPointCalc
{
    class Algorithm
    {
        //将点列表坐标转换，移至控件中心为原点坐标系
        internal static List<PlotPoint> GetActualPos(PictureBox TargetPictureBox, List<PlotPoint> Points, float Ratio)
        {
            float[,] tmp = new float[Points.Count, 2];
            
            for (int i = 0; i < Points.Count; i++)
            {
                tmp[i, 0] = (Points[i].X - (TargetPictureBox.Width / 2)) * Ratio / TargetPictureBox.Width;
                tmp[i, 1] = ((TargetPictureBox.Width / 2) - Points[i].Y) * Ratio / TargetPictureBox.Height;
            }
            List<PlotPoint> CorrectedPoints = new List<PlotPoint>();
            for (int i = 0; i < tmp.GetLength(0); i++)
            {
                CorrectedPoints.Add(new PlotPoint { X = tmp[i, 0], Y = tmp[i, 1] });
            }
            return CorrectedPoints;
        }

        //将点移至控件中心为原点坐标系
        internal static PlotPoint GetActualPos(PictureBox TargetPictureBox, PlotPoint Points)
        {
            Points.X -= (TargetPictureBox.Width / 2);
            Points.Y = (TargetPictureBox.Width / 2) - Points.Y;         
            return Points;
        }

        //获取所求比例点坐标
        internal static PlotPoint GetPointPos(List<PlotPoint> Points, float Proportion)
        {
            if (Points.Count <= 1)
            {
                throw new Exception("点位过少");
            }
            if(Proportion == 0)
            {
                float xx = Points[0].X;
                float yy = Points[0].Y;
                PlotPoint PointPos = new PlotPoint { X = xx, Y = yy };
                return PointPos;
            }
            else
            {
                double TargetLength = GetPolyLineLength(Points) * Proportion;   //获取目标长度      
                int i = 0;
                double CurrentLength = 0.0;
                //获取超过指定比例长度的第一个点
                while (CurrentLength < TargetLength)
                {                   
                    CurrentLength += GetSingleLineLength(Points[i], Points[i + 1]);
                    i++;
                }
                
                //获取超过指定比例长度的前一个点
                int j = i - 1;
                double PreLength = CurrentLength - GetSingleLineLength(Points[j], Points[i]);   //获取至前一点的长度
                double RemainLength = TargetLength - PreLength; //获取在目标线段上的剩余距离
                double CurrentLineLength = GetSingleLineLength(Points[j], Points[i]);   //获取目标点所在线段长度
                float Pro = (float)(RemainLength / CurrentLineLength);  //计算目标点在线段上的比例

                PlotPoint PointPos = new PlotPoint { X = Points[j].X + (Points[i].X - Points[j].X) * Pro, Y = Points[j].Y + (Points[i].Y - Points[j].Y) * Pro };
                return PointPos;
            }
        }

        //计算单条线段长度
        internal static double GetSingleLineLength(PlotPoint StartPoint, PlotPoint EndPoint)
        {
            return Math.Sqrt(Math.Pow((EndPoint.X - StartPoint.X), 2) + Math.Pow((EndPoint.Y - StartPoint.Y), 2));
        }

        //计算整条折线段长度
        internal static double GetPolyLineLength(List<PlotPoint> Points)
        {
            double Total = 0.0;
            for(int i = 1; i < Points.Count; i++)
            {
                Total += GetSingleLineLength(Points[i - 1], Points[i]);
            }
            return Total;
        }

        //反色
        internal static Color InverseColor(Color OriginalColor)
        {
            const int RGBMAX = 255;
            return Color.FromArgb(RGBMAX - OriginalColor.R, RGBMAX - OriginalColor.G, RGBMAX - OriginalColor.B);
        }

        //获取多边形面积
        internal static double GetPolygonArea(List<PlotPoint> Points, PictureBox TargetPictureBox, float Ratio)
        {
            double area = 0;
            List<PlotPoint> CPoints = GetActualPos(TargetPictureBox, Points, Ratio);

            for (int i = 0; i < CPoints.Count - 1; i++)
            {
                area += (CPoints[i + 1].X - CPoints[i].X) * (CPoints[i + 1].Y + CPoints[i].Y);
            }
            area += (CPoints[0].X - CPoints[CPoints.Count - 1].X) * (CPoints[0].Y + CPoints[CPoints.Count - 1].Y);  //闭合多边形

            // Return the result.
            return Math.Abs(area / 2);
        }
    }
}
