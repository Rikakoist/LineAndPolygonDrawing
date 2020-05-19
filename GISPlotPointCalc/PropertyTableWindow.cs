using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISPlotPointCalc
{
    public partial class PropertyTableWindow : Form
    {
        public PropertyTableWindow()
        {
            InitializeComponent();
        }

        //更新线段
        private void SaveCurrentValue(object sender, DataGridViewCellCancelEventArgs e)
        {

        }
        
        //刷新视图
        internal void UpdateDataGridView(PolyLine Line)
        {
            PointsDataGridView.Rows.Clear();
            for (int i = 0; i < Line.Points.Count; i++)
            {
                PointsDataGridView.Rows.Add();
                PointsDataGridView[0, i].Value = i + 1;
                PointsDataGridView[1, i].Value = Line.Points[i].X;
                PointsDataGridView[2, i].Value = Line.Points[i].Y;
            }
        }
    }
}
