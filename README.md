# LineAndPolygonDrawing
折线和多边形绘制小程序

一个GDI+绘图练习程序

## 程序设计及说明
### 一、图形界面元素的组织
程序界面分为左右两大部分。左侧为绘图区，右侧为参数设定区。<br>
界面左侧的上方为一工具栏。其左侧有使用提示，能够根据当前状态提示用户操作，其右侧为“编辑”按钮，用于控制编辑状态；使用提示的下方为绘图区，用户可在此绘制折线及多边形。<br>
界面右侧的上方可以设置绘制比例尺、编辑捕获容差以及是否绘制多边形，“绘制与编辑”组框的右上角为“关于”按钮，用于显示作者信息；中部可以设置多边形填充、点、线、文字的绘制样式；界面右侧中下方根据是否为多边形绘制模式可计算折线段长度及目标点位或多边形面积；界面右侧下方能够进行文件的存取、存储路径的更改、打开存储文件夹及绘图的保存。

### 二、用户交互的设计
#### 2.1 绘制折线（多边形）
用户设置完比例尺后，点击“开始绘制”按钮或按键盘上的“S”键开始绘图，右侧界面将被部分禁用直至绘图结束，左上方使用提示会显示将要进行的操作。用户可使用左键在图片框上绘制点，使用右键撤销绘制上一个点。在绘制过程中，可以按下“Esc”键终止绘制、按下“F2”键或双击鼠标左键结束绘制。在绘制的过程中，会有点与线（多边形）的预览，预览部分的颜色为其样式框中颜色的反色。折线长度或多边形面积也会随着预览同步变化。

#### 2.2 样式更改
用户点击相应的图片框或“更改文字样式”标签，即可打开颜色对话框或字体对话框，更改目标的样式，也可调整滚动条来更改点、线的尺寸及多边形的透明度。在已绘制折线段或多边形的前提下，所有样式的更改都将实时显示。

#### 2.3 计算目标点位
在绘制完成后或成功读取点位坐标后，“计算”组框将会被启用。同时，折线段长度将会被计算并显示。点击“计算”按钮，即可显示所求点位信息。用户可通过拖动滚动条或更改文本框中的文字来改变所求点在折线上的比例。所有更改将会被实时计算并绘制。

#### 2.4 计算多边形面积
在绘制过程中、绘制完成后及编辑过程中，多边形面积都将实时计算显示。

#### 2.5 文件操作
通过点击“保存折线到文件” 按钮，用户可将当前所绘制折线的点位信息保存至.xml文件。通过点击“从文件读取折线” 按钮，用户可从指定位置加载之前绘制的折线或多边形。通过点击“更改文件夹”按钮，用户能够更改文件的存取路径与图片的存储路径。通过点击“打开文件夹”按钮，用户能够打开数据文件与图片存储的文件夹。通过点击“保存绘图到文件”按钮，用户能够将当前绘图存储至.png文件。<br>
除此之外，用户还能够通过拖拽数据文件到应用程序窗体来打开数据文件。

#### 2.6 点的交互编辑
点击“开始编辑”按钮或按下“E”键，程序将进入编辑状态。此时，程序根据用户设置的容差尝试捕捉满足条件的点，并实时提示选中状态。在捕捉到点的前提下，用户可以按下鼠标左键并移动来编辑点位；按下鼠标右键或“Delete”键删除该点。编辑结束后，再次点击按钮或按“E”键来退出编辑模式。工具栏中的使用提示将会实时显示用户当前的操作。

#### 2.7 属性表
点击“属性表”按钮，能够打开属性表窗体查看当前点位坐标信息（控件坐标系）。在可见情况下，属性表内信息将会随用户对点位的更改实时更新。当属性表被用户隐藏时，其内容不会更新，且占用内存被释放，直至用户再次点击按钮将其显示。

#### 2.8 配置文件
每次关闭程序时，用户当前的设定都会丢失而需在下一次使用时重新设置。为此，程序引入了配置文件。窗体内存取方法如下：

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

该方法在程序启动和结束时分别调取一次。

#### 2.9 异常处理
为了应对运行中可能出现的异常并防止系统级错误提示框的出现，程序在编写时采用了“try…catch…”语句。对于异常的处理有两种方法：重置初始值（容差、比例尺设置错误等异常）而不给用户提示或弹出消息框提示用户解决异常（文件读取问题、索引越界等异常）。
