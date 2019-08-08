using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace GISPlotPointCalc
{
    public partial class PolyLinePlotPoints : Form
    {
        //定义全局变量
        bool IsDrawing = false; //是否正在绘图
        bool IsPreviewing = false;  //是否正在预览
        bool IsCalculated = false;  //折线点是否已计算
        bool IsPolyLineCalculated = false;  //折线段长度是否已计算
        bool IsEditing = false; //是否在编辑模式
        bool IsPointSelected = false;   //是否选中点
        bool IsPointMoving = false; //是否正在移动点
        int SelectedPointIndex = -1;    //选中的点索引值
        int CurrentPoint = 0;   //当前绘制点索引值

        readonly Cursor CrossCur = new Cursor(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CustomCur.cur");    //自定义鼠标
        PropertyTableWindow PTW; //= new PropertyTableWindow();    //属性表窗口
        //Cursor CrossCur = Cursors.Cross;

        //新建单根折线
        internal PolyLine Line = new PolyLine
        {
            Points = new List<PlotPoint>()
        };

        public PolyLinePlotPoints()
        {
            InitializeComponent();
        }

        //窗口加载，进行初始化操作
        private void PolyLinePlotPoints_Load(object sender, EventArgs e)
        {
            try
            {
                FilePathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                PolyLineCalcGroupBox.Enabled = false;
                SaveToPicButton.Enabled = false;
                PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);    //启动时清空画板
                SettingsOperation(-1);
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }

        //窗口关闭中
        private void SaveUserSettings(object sender, FormClosingEventArgs e)
        {
            try
            {
                SettingsOperation(1);
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }

        //退出应用
        private void PolyLinePlotPoints_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #region 画图
        //开始画图，更改文本及状态量
        private void StartDrawingButton_Click(object sender, EventArgs e)
        {
            try
            {
                PolyLineLengthLabel.Text = "折线长度：NaN (m)";
                ResultLabel.Text = "点位：";
                DrawingTipLabel.Text = "左键绘点，右键撤销，双击或按F2结束绘制。请绘制第1个点，按Esc取消";
                DrawGroupBox.Enabled = false;
                PolyLineCalcGroupBox.Enabled = false;
                FileGroupBox.Enabled = false;
                PlotPointPictureBox.Cursor = CrossCur;
                Line.Points.Clear();
                EditStateButton.CheckState = CheckState.Unchecked;
                PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
                PlotPointPictureBox.Image = new Bitmap(PlotPointPictureBox.Width, PlotPointPictureBox.Height);
                IsDrawing = true;
                CurrentPoint = 0;
                PlotPointPictureBox.Cursor = CrossCur;
                EditStateButton.Enabled = false;
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }

        //多边形绘制状态选择
        private void PolygonState(object sender, EventArgs e)
        {
            if (Line.Points.Count > 0)
            {
                ClearAndDraw();
            }
            if (IsPolygonCheckBox.Checked)
            {
                PolygonCalcGroupBox.Visible = true;
                PolyLineCalcGroupBox.Visible = false;
                DrawingTipLabel.Text = "多边形绘制模式";
            }
            else
            {
                PolygonCalcGroupBox.Visible = false;
                PolyLineCalcGroupBox.Visible = true;
                DrawingTipLabel.Text = "折线绘制模式";
            }
        }

        //画图
        private void PlotPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            //正在画图，根据鼠标点击键位增删点
            if (IsDrawing)
            {
                //左键添加
                if (e.Button == MouseButtons.Left)
                {
                    Line.Points.Add(new PlotPoint { X = e.X, Y = e.Y });
                    CurrentPoint++;
                    DrawingTipLabel.Text = "请绘制第" + Convert.ToString(CurrentPoint + 1) + "个点，或按Esc取消绘制，双击或按F2结束绘制";
                    ClearAndDraw();
                }

                //右键删除
                if (e.Button == MouseButtons.Right)
                {
                    if (CurrentPoint == 0)
                    {
                        DrawingTipLabel.Text = "绘制被用户取消";
                        PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
                        Line.Points.Clear();
                        PolyLineLengthLabel.Text = "折线长度：NaN (m)";
                        ResultLabel.Text = "点位：";
                        Restore();
                    }
                    else
                    {
                        Line.Points.RemoveAt(CurrentPoint - 1);
                        CurrentPoint--;
                        DrawingTipLabel.Text = "撤销绘制，请绘制第" + Convert.ToString(CurrentPoint + 1) + "个点，或按Esc取消绘制";
                        ClearAndDraw();
                    }
                }
            }

            //编辑状态右键删除点
            if (IsEditing && IsPointSelected && !IsPointMoving)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DeletePoint();
                    if(Line.Points.Count<1)
                    {
                        SelectedPointIndex = -1;
                        IsPointSelected = false;
                        PlotPointPictureBox.Cursor = Cursors.No;
                        DrawingTipLabel.Text = "点已被用户全部删除，请重新绘制或加载";
                    }
                }
            }
        }

        //开始预览
        private void MouseEntering(object sender, EventArgs e)
        {
            if (IsDrawing)
            {
                IsPreviewing = true;
            }
        }

        //停止预览
        private void MouseLeaving(object sender, EventArgs e)
        {
            if (IsDrawing)
            {
                IsPreviewing = false;
            }
        }

        //预览或编辑
        private void PreviewOrEdit(object sender, MouseEventArgs e)
        {
            try
            {
                //预览
                if (IsDrawing && IsPreviewing)
                {
                    PlotPoint Preview = new PlotPoint
                    {
                        X = e.X,
                        Y = e.Y
                    };
                    ClearAndDraw();
                    if (Line.Points.Count > 0)
                    {
                        if (PolyLineCalcGroupBox.Visible)
                        {
                            PolyLineLengthLabel.Text = "折线长度：" + (((Algorithm.GetPolyLineLength(Line.Points) + Algorithm.GetSingleLineLength(Line.Points[Line.Points.Count - 1], Preview)) / (PlotPointPictureBox.Width)) * Convert.ToDouble(RatioTextBox.Text)).ToString("0.0000") + " (m)";
                        }
                        if (IsPolygonCheckBox.Checked)
                        {
                            if (Line.Points.Count > 1)
                            {
                                PolyLineDrawing.DrawPolygon(Line.Points, PlotPointPictureBox, PolygonColorPictureBox.BackColor, PolygonTransparencyTrackBar.Value * 255 / (PolygonTransparencyTrackBar.Maximum - PolygonTransparencyTrackBar.Minimum), Preview);
                                List<PlotPoint> PreviewArea = new List<PlotPoint>();
                                for (int i = 0; i < Line.Points.Count(); i++)
                                {
                                    PreviewArea.Add(Line.Points[i]);
                                }
                                PreviewArea.Add(Preview);
                                PolygonAreaLabel.Text = "多边形面积：" + (Algorithm.GetPolygonArea(PreviewArea, PlotPointPictureBox, Convert.ToSingle(RatioTextBox.Text))).ToString("0.0000") + "（m ^ 2）";
                            }
                            PolyLineDrawing.DrawPreviewLine(Line.Points[0], Preview, PlotPointPictureBox, Algorithm.InverseColor(LineColorPictureBox.BackColor), LineWidthTrackBar.Value);
                        }
                        PolyLineDrawing.DrawPreviewLine(Line.Points[Line.Points.Count - 1], Preview, PlotPointPictureBox, Algorithm.InverseColor(LineColorPictureBox.BackColor), LineWidthTrackBar.Value);
                    }
                    PolyLineDrawing.DrawPoints(Preview, PlotPointPictureBox, Algorithm.InverseColor(PointColorPictureBox.BackColor), PointWidthTrackBar.Value);
                    PolyLineDrawing.DrawFonts(Preview, Convert.ToString(Line.Points.Count + 1), PlotPointPictureBox, FontColorPictureBox.BackColor, FontStyleDisplayLabel.Font, PointWidthTrackBar.Value);
                    PlotPointPictureBox.Invalidate();
                }

                //编辑
                if (!IsDrawing && IsEditing && !IsPointMoving && Line.Points.Count != 0)
                {
                    PlotPointPictureBox.Cursor = Cursors.Arrow;
                    List<double> Dis = new List<double>();
                    PlotPoint EditPos = new PlotPoint
                    {
                        X = e.X,
                        Y = e.Y
                    };

                    foreach (PlotPoint Point in Line.Points)
                    {
                        Dis.Add(Algorithm.GetSingleLineLength(EditPos, Point));
                    }

                    //找最小距离点
                    double MinDistance = Dis[0];
                    int MinDistanceIndex = 0;
                    for (int i = 0; i < Dis.Count; i++)
                    {
                        if (Dis[i] < MinDistance)
                        {
                            MinDistance = Dis[i];
                            MinDistanceIndex = i;
                        }
                    }

                    //小于容差则选中
                    if (MinDistance < Convert.ToDouble(ToleranceTextBox.Text))
                    {
                        IsPointSelected = true;
                        PlotPointPictureBox.Cursor = CrossCur;
                        SelectedPointIndex = MinDistanceIndex;
                        DrawingTipLabel.Text = "已选中第" + Convert.ToString(SelectedPointIndex + 1) + "个点，按住鼠标左键拖动以改变位置，按Del或鼠标右键删除";
                        PolyLineDrawing.DrawPoints(Line.Points[SelectedPointIndex], PlotPointPictureBox, Algorithm.InverseColor(PointColorPictureBox.BackColor), Convert.ToInt32(PointWidthTrackBar.Value * 1.5));
                        PlotPointPictureBox.Refresh();
                    }
                    else
                    {
                        if (SelectedPointIndex != -1)
                        {
                            ClearAndDraw();
                        }
                        DisSelect();
                        DrawingTipLabel.Text = "未选中任何点";
                    }
                }

                //移动选中点
                if (!IsDrawing && IsEditing && IsPointMoving && Line.Points.Count != 0)
                {
                    if (e.X >= 0 && e.Y >= 0 && e.X <= PlotPointPictureBox.Width && e.Y <= PlotPointPictureBox.Height)
                    {
                        Line.Points[SelectedPointIndex].X = e.X;
                        Line.Points[SelectedPointIndex].Y = e.Y;
                        ClearAndDraw();
                        DrawingTipLabel.Text = "正在移动第" + Convert.ToString(SelectedPointIndex + 1) + "个点";
                    }
                    else
                    {
                        DisSelect();
                        DrawingTipLabel.Text = "焦点丢失";
                    }
                }
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }

        //开始移动点
        private void MovePoints(object sender, MouseEventArgs e)
        {
            if (IsEditing && SelectedPointIndex != -1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    IsPointMoving = true;
                    DrawingTipLabel.Text = "开始移动第" + Convert.ToString(SelectedPointIndex + 1) + "个点";
                }
            }
        }

        //停止移动点
        private void StopMovingPoints(object sender, MouseEventArgs e)
        {
            if (IsEditing && SelectedPointIndex != -1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    IsPointMoving = false;
                    DrawingTipLabel.Text = "已移动第" + Convert.ToString(SelectedPointIndex + 1) + "个点";
                }
            }
        }

        //双击停止绘制
        private void DoubleClickToStopDrawing(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsDrawing)
            {
                StopDrawing();
            }
        }

        //清空画板
        private void ClearPictureBoxButton_Click(object sender, EventArgs e)
        {
            PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
            Line.Points.Clear();
            DrawingTipLabel.Text = "已清空，点击“开始画线”开始绘制，点击“从文件读取折线”或直接拖拽数据文件到窗体以加载数据";
            Restore();
        }

        //编辑折点模式
        private void EditStateChange(object sender, EventArgs e)
        {
            SelectedPointIndex = -1;
            if (EditStateButton.CheckState == CheckState.Checked)
            {
                IsEditing = true;
                DrawingTipLabel.Text = "开始编辑";
            }
            else
            {
                IsEditing = false;
                IsPointMoving = false;
                IsPointSelected = false;
                SelectedPointIndex = -1;
                DrawingTipLabel.Text = "结束编辑";
                PlotPointPictureBox.Cursor = Cursors.No;
            }
        }
        #endregion

        #region 折线段长度及比例
        //计算线段对应比例点
        private void CalcButton_Click(object sender, EventArgs e)
        {
            try
            {
                float Ratio = (float)(PolyLineRatioScrollBar.Value / 100.0);
                double ScaleRatio = Convert.ToDouble(RatioTextBox.Text);
                if (Ratio < 0 || Ratio > 1)
                {
                    throw new Exception("所求线段比例非法！");
                }
                else
                {
                    PlotPoint Target = new PlotPoint(); //为所求点新建对象
                    Target = Algorithm.GetPointPos(Line.Points, Ratio);  //计算所求点位置
                    ClearAndDraw();
                    PolyLineDrawing.DrawPoints(Target, PlotPointPictureBox, Algorithm.InverseColor(PointColorPictureBox.BackColor), PointWidthTrackBar.Value);
                    PolyLineDrawing.DrawFonts(Target, "P", PlotPointPictureBox, FontColorPictureBox.BackColor, FontStyleDisplayLabel.Font, PointWidthTrackBar.Value);
                    PlotPointPictureBox.Invalidate();
                    //计算并输出实际坐标系点位
                    PlotPoint Corrected = Algorithm.GetActualPos(PlotPointPictureBox, Target);
                    ResultLabel.Text = "点位：(" + (Corrected.X * ScaleRatio / PlotPointPictureBox.Width).ToString("0.0000") + ", " + (Corrected.Y * ScaleRatio / PlotPointPictureBox.Width).ToString("0.0000") + ") (m)";
                    IsCalculated = true;
                }
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }

        //改变所求线段比例
        private void RatioChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == PolyLineRatioScrollBar)
                {
                    PolyLineRatioTextBox.Text = Convert.ToString(PolyLineRatioScrollBar.Value);
                    CalcButton.PerformClick();
                }
                if (sender == PolyLineRatioTextBox)
                {
                    PolyLineRatioScrollBar.Value = Convert.ToInt32(PolyLineRatioTextBox.Text);
                }
                IsCalculated = true;
            }
            catch (Exception)
            {
                PolyLineRatioScrollBar.Value = 0;
                PolyLineRatioTextBox.Text = "0";
            }
        }

        //实时改变比例
        private void PolyLineRatioChange(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RatioTextBox.Text))
                {
                    RatioTextBox.Text = "500";
                }
                if (IsPolyLineCalculated)
                {

                    if (IsCalculated)
                    {
                        CalcButton.PerformClick();
                    }
                }
                UpdateLengthOrArea();
            }
            catch (Exception)
            {
                RatioTextBox.Text = "500";
            }
        }

        //改变捕捉容差
        private void CaptureToleranceChange(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ToleranceTextBox.Text))
                {
                    ToleranceTextBox.Text = "5";
                }
            }
            catch (Exception)
            {
                ToleranceTextBox.Text = "5";
            }
        }

        #endregion

        #region 文件操作

        //DragEnter设置
        private void PreDragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        //拖拽打开
        private void DragDropToOpenFile(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files.GetLength(0) > 1)
                {
                    throw new Exception("每次仅可加载一个数据文件！");
                }
                var Extension = System.IO.Path.GetExtension(files[0]);
                if (Extension.Equals(".xml", StringComparison.CurrentCultureIgnoreCase))
                {
                    FilePathTextBox.Text = System.IO.Path.GetDirectoryName(files[0]);   //定位到当前文件夹
                    Line.Points.Clear();
                    Line.Points = IO.LoadFromFile(System.IO.Path.GetFullPath(files[0]));
                    FileLoad(System.IO.Path.GetFullPath(files[0]));
                }
                else
                {
                    throw new Exception("请选择.xml文件！");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //按钮管理类
        private void FileManagement(object sender, EventArgs e)
        {
            try
            {
                //存储
                if (sender == SaveToFileButton)
                {
                    string st = FilePathTextBox.Text + "\\PlotPoints.xml";
                    IO.SaveToFile(st, Line.Points);
                    DrawingTipLabel.Text = "成功保存点位信息至" + st;
                }

                //读取
                if (sender == ReadFromFileButton)
                {
                    string st = FilePathTextBox.Text + "\\PlotPoints.xml";
                    Line.Points.Clear();
                    Line.Points = IO.LoadFromFile(st);
                    FileLoad(st);
                }

                //改路径
                if (sender == ChangeFilePathButton)
                {
                    FolderBrowserDialog ChangeFilePath = new FolderBrowserDialog();
                    if (ChangeFilePath.ShowDialog() == DialogResult.OK)
                    {
                        FilePathTextBox.Text = ChangeFilePath.SelectedPath;
                        DrawingTipLabel.Text = "已更改工作目录至" + FilePathTextBox.Text;
                    }
                }

                //开文件夹
                if (sender == OpenFileLocation)
                {
                    System.Diagnostics.Process.Start("explorer.exe", FilePathTextBox.Text);
                }

                //存图
                if (sender == SaveToPicButton)
                {
                    string st = FilePathTextBox.Text + "\\PolyLine_" + IO.DT() + ".png";
                    IO.Save2Pic(PlotPointPictureBox, st);
                    DrawingTipLabel.Text = "成功保存当前绘图至" + st;
                }
            }
            catch (Exception err)
            {
                MessageBoxes.Error(err.Message);
            }
        }
        #endregion

        #region 样式操作
        //改变颜色及点线型
        private void ChangeStyle(object sender, EventArgs e)
        {
            if (sender == LineWidthTrackBar || sender == PointWidthTrackBar || sender == PolygonTransparencyTrackBar)
            {
                if (Line.Points.Count > 0)
                {
                    ClearAndDraw();
                }
                if (!IsPolygonCheckBox.Checked && IsCalculated)
                {
                    CalcButton.PerformClick();
                }
            }
            else
            {
                Color CurrentColor = Color.White;   //获取当前颜色
                if (sender == PointColorPictureBox)
                {
                    CurrentColor = PointColorPictureBox.BackColor;
                }
                if (sender == LineColorPictureBox)
                {
                    CurrentColor = LineColorPictureBox.BackColor;
                }
                if (sender == FontColorPictureBox)
                {
                    CurrentColor = FontColorPictureBox.BackColor;
                }
                if (sender == PolygonColorPictureBox)
                {
                    CurrentColor = PolygonColorPictureBox.BackColor;
                }
                ColorDialog ChooseColorDialog = new ColorDialog
                {
                    FullOpen = true,
                    Color = CurrentColor,
                };
                if (ChooseColorDialog.ShowDialog() == DialogResult.OK)
                {
                    if (sender == PointColorPictureBox)
                    {
                        PointColorPictureBox.BackColor = ChooseColorDialog.Color;
                    }
                    if (sender == LineColorPictureBox)
                    {
                        LineColorPictureBox.BackColor = ChooseColorDialog.Color;
                    }
                    if (sender == FontColorPictureBox)
                    {
                        FontColorPictureBox.BackColor = ChooseColorDialog.Color;
                    }
                    if (sender == PolygonColorPictureBox)
                    {
                        PolygonColorPictureBox.BackColor = ChooseColorDialog.Color;
                    }
                    if (Line.Points.Count > 0)
                    {
                        ClearAndDraw();
                    }
                    if (IsCalculated)
                    {
                        CalcButton.PerformClick();
                    }
                }
            }
        }

        //改变字体
        private void PickFontStyle(object sender, EventArgs e)
        {
            FontDialog FontStyleDialog = new FontDialog()
            {
                Font = new Font(FontStyleDisplayLabel.Font.FontFamily, FontStyleDisplayLabel.Font.Size)
            };
            if (FontStyleDialog.ShowDialog() == DialogResult.OK)
            {
                FontStyleDisplayLabel.ForeColor = FontStyleDialog.Color;
                FontStyleDisplayLabel.Font = new Font(FontStyleDialog.Font.FontFamily, FontStyleDialog.Font.Size);
                if (Line.Points.Count > 0)
                {
                    ClearAndDraw();
                }
            }
        }
        #endregion

        #region 按键操作
        private void KeyAction(object sender, KeyEventArgs e)
        {
            //ESC取消画图
            if (e.KeyCode == Keys.Escape && IsDrawing)
            {
                PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
                Line.Points.Clear();
                DrawingTipLabel.Text = "绘制被用户取消";
                Restore();
            }

            //F2结束绘图
            if (e.KeyCode == Keys.F2 && IsDrawing)
            {
                if (CurrentPoint == 0)
                {
                    DrawingTipLabel.Text = "绘制被用户取消";
                    PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
                    Line.Points.Clear();
                    PolyLineLengthLabel.Text = "折线长度：NaN (m)";
                    ResultLabel.Text = "点位：";
                    Restore();
                }
                else
                {
                    StopDrawing();
                }
            }

            //S开始绘图
            if (e.KeyCode == Keys.S && !IsDrawing)
            {
                StartDrawingButton.PerformClick();
            }

            //C清空画板
            if (e.KeyCode == Keys.C && !IsDrawing)
            {
                ClearPictureBoxButton.PerformClick();
            }

            //E编辑
            if (e.KeyCode == Keys.E && !IsDrawing)
            {
                EditStateButton.PerformClick();
            }

            //编辑状态下Del删除点
            if (e.KeyCode == Keys.Delete && IsEditing && IsPointSelected && !IsPointMoving)
            {
                DeletePoint();
            }
        }

        //文本框输入限制
        private void InputRestrictions(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (byte)(e.KeyChar) == 8)
            {
            }
            else
            {
                e.Handled = true;
            }
        }
        #endregion

        #region 公共方法
        //停止画图
        internal void StopDrawing()
        {
            IsDrawing = false;
            DrawGroupBox.Enabled = true;
            PolyLineCalcGroupBox.Enabled = true;
            FileGroupBox.Enabled = true;
            SaveToFileButton.Enabled = true;
            SaveToPicButton.Enabled = true;
            IsPolyLineCalculated = true;
            PlotPointPictureBox.Cursor = Cursors.No;
            EditStateButton.CheckState = CheckState.Unchecked;
            ClearAndDraw();
            UpdateLengthOrArea();
            DrawingTipLabel.Text = "绘制结束，共绘制了" + Convert.ToString(CurrentPoint) + "个点";
            EditStateButton.Enabled = true;
        }

        //清空并重绘
        internal void ClearAndDraw()
        {
            PolyLineDrawing.ClearPictureBox(PlotPointPictureBox);
            if (IsPolygonCheckBox.Checked)
            {
                if (Line.Points.Count > 1)
                {
                    PolyLineDrawing.DrawPolygon(Line.Points, PlotPointPictureBox, PolygonColorPictureBox.BackColor, PolygonTransparencyTrackBar.Value * 255 / (PolygonTransparencyTrackBar.Maximum - PolygonTransparencyTrackBar.Minimum));
                }
                if (Line.Points.Count > 0)
                {
                    PolyLineDrawing.DrawLines(Line.Points[Line.Points.Count - 1], Line.Points[0], PlotPointPictureBox, LineColorPictureBox.BackColor, LineWidthTrackBar.Value);
                }
            }
            PolyLineDrawing.Draw(Line.Points, PlotPointPictureBox, PointColorPictureBox.BackColor, LineColorPictureBox.BackColor, FontColorPictureBox.BackColor, FontStyleDisplayLabel.Font, LineWidthTrackBar.Value, PointWidthTrackBar.Value);
            //PlotPointPictureBox.Invalidate();
            PlotPointPictureBox.Update();
            //PlotPointPictureBox.Refresh();
            UpdateLengthOrArea();
            UpdateDataGridView();
        }

        //刷新折线长度
        internal void UpdateLengthOrArea()
        {
            if (PolyLineCalcGroupBox.Visible)
            {
                if (Line.Points.Count > 1)
                {
                    PolyLineLengthLabel.Text = "折线长度：" + ((Algorithm.GetPolyLineLength(Line.Points) / (PlotPointPictureBox.Width)) * Convert.ToDouble(RatioTextBox.Text)).ToString("0.0000") + " (m)";
                }
                else
                {
                    PolygonAreaLabel.Text = "折线长度：NaN (m)";
                }
            }
            if (IsPolygonCheckBox.Checked)
            {
                if (Line.Points.Count > 2)
                {
                    PolygonAreaLabel.Text = "多边形面积：" + (Algorithm.GetPolygonArea(Line.Points, PlotPointPictureBox, Convert.ToSingle(RatioTextBox.Text))).ToString("0.0000") + "（m ^ 2）";
                }
                else
                {
                    PolygonAreaLabel.Text = "多边形面积：NaN（m ^ 2）";
                }
            }
        }

        //重置界面状态
        internal void Restore()
        {
            DrawGroupBox.Enabled = true;
            PolyLineCalcGroupBox.Enabled = false;
            FileGroupBox.Enabled = true;
            SaveToFileButton.Enabled = false;
            SaveToPicButton.Enabled = false;
            IsDrawing = false;
            IsPreviewing = false;
            IsCalculated = false;
            IsPolyLineCalculated = false;
            UpdateDataGridView();
            PlotPointPictureBox.Cursor = Cursors.No;
            CurrentPoint = 0;
            EditStateButton.CheckState = CheckState.Unchecked;
            EditStateButton.Enabled = true;
            PolyLineLengthLabel.Text = "折线长度：NaN (m)";
            PolygonAreaLabel.Text = "多边形面积：NaN（m ^ 2）";
            ResultLabel.Text = "点位：";
        }

        //删除点
        internal void DeletePoint()
        {
            Line.Points.RemoveAt(SelectedPointIndex);
            ClearAndDraw();
            UpdateLengthOrArea();
            DrawingTipLabel.Text = "已删除第" + Convert.ToString(SelectedPointIndex + 1) + "个点";
        }

        //取消选择
        internal void DisSelect()
        {
            IsPointSelected = false;
            SelectedPointIndex = -1;
            IsPointMoving = false;
        }

        //文件加载
        internal void FileLoad(string st)
        {
            ResultLabel.Text = "点位：";
            DrawingTipLabel.Text = "成功从文件" + st + "加载" + Convert.ToString(Line.Points.Count) + "个点数据并绘制";
            PolyLineCalcGroupBox.Enabled = true;
            SaveToFileButton.Enabled = true;
            SaveToPicButton.Enabled = true;
            ClearAndDraw();
            IsCalculated = false;
            EditStateButton.CheckState = CheckState.Unchecked;
        }

        //设置存取
        internal void SettingsOperation(int Method)
        {
            string SettingPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\PlotSettings.xml";
            if (Method == 1)    //存
            {
                IO.SaveSettings(SettingPath, FilePathTextBox, IsPolygonCheckBox, RatioTextBox, ToleranceTextBox, PointColorPictureBox, LineColorPictureBox, FontColorPictureBox, PolygonColorPictureBox, FontStyleDisplayLabel, PointWidthTrackBar, LineWidthTrackBar, PolygonTransparencyTrackBar);
            }
            if (Method == -1)    //取
            {
                //加载配置文件
                if (!File.Exists(SettingPath))   //不存在创建
                {
                    SettingsOperation(1);
                }
                else   //存在读取
                {
                    IO.LoadSettings(SettingPath, ref FilePathTextBox, ref IsPolygonCheckBox, ref RatioTextBox, ref ToleranceTextBox, ref PointColorPictureBox, ref LineColorPictureBox, ref FontColorPictureBox, ref PolygonColorPictureBox, ref FontStyleDisplayLabel, ref PointWidthTrackBar, ref LineWidthTrackBar, ref PolygonTransparencyTrackBar);
                }
                DrawingTipLabel.Text = "点击“开始画线”开始绘制，点击“从文件读取折线”或直接拖拽数据文件到窗体以加载数据";
            }
        }

        //更新属性表
        internal void UpdateDataGridView()
        {
            if (PropertyTableButton.Checked)
            {
                if (PTW != null || !PTW.IsDisposed)
                {
                    PTW.UpdateDataGridView(Line);
                }
            }
        }
        #endregion

        //属性表
        private void PropertyTable(object sender, EventArgs e)
        {
            if (PropertyTableButton.Checked)
            {
                if (PTW == null || PTW.IsDisposed)  //窗体不存在
                {
                    PTW = new PropertyTableWindow();
                }

                //设置窗体位置
                if (this.Location.X + 1035 > Screen.PrimaryScreen.Bounds.Width * 0.95)
                {
                    PTW.Location = new Point { X = this.Location.X - 200, Y = this.Location.Y };
                }
                else
                {
                    PTW.Location = new Point { X = this.Location.X + 1035, Y = this.Location.Y };
                }

                PTW.Show();
                PTW.UpdateDataGridView(Line);
            }
            else
            {
                if (PTW != null || !PTW.IsDisposed)
                {
                    PTW.Hide();
                    PTW.Dispose();
                }
            }
        }

        //作者信息
        private void AboutButton_Click(object sender, EventArgs e)
        {
            MessageBoxes.Author();
        }
    }
}
