using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISPlotPointCalc
{
    //定义线段端点对象
    public class PlotPoint
    {
        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }
    }
    
    //定义折线对象
    public class PolyLine
    {
        public List<PlotPoint> Points
        {
            get;
            set;
        }
    }
}