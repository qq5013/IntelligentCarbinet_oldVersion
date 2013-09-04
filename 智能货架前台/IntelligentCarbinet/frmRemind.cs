using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using CarbinetNM;
using PublicConfig;

namespace IntelligentCarbinet
{
    public partial class frmRemind : Form
    {
        Timer _timer = new Timer();
        public frmRemind()
        {
            InitializeComponent();

            //统一初始化
            //if (MemoryTable.isInitialized == false)
            //{
            //    MemoryTable.initializeTabes();
            //}


            GraphPane myPane = this.zedGraphControl1.GraphPane;

            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

            myPane.Title.Text = "库存预警商品信息";
            myPane.XAxis.Title.Text = "商品名称";
            myPane.YAxis.Title.Text = "商品数量";
            myPane.YAxis.MinSpace = 1;

            //appConfig ac = appConfig.getDefaultConfig();
            _timer.Interval = staticClass.storageCheckInterval;//5000;
            _timer.Tick += new EventHandler(_timer_Tick);

            this.Shown += new EventHandler(frmRemind_Shown);
            this.FormClosing += new FormClosingEventHandler(frmRemind_FormClosing);
        }

        void frmRemind_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Tick -= (_timer_Tick);
            _timer.Enabled = false;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            CreateGraph_Chart(this.zedGraphControl1);  //图表
        }

        void frmRemind_Shown(object sender, EventArgs e)
        {
            CreateGraph_Chart(this.zedGraphControl1);  //图表
            _timer.Enabled = true;
        }


        // Build the Chart
        private void CreateGraph_Chart(ZedGraphControl zg1)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zg1.GraphPane;
            // Make up some random data points
            myPane.RemoveAllCurve();

            DataRow[] rows = MemoryTable.typeMapTable.Select("minCount > count");
            if (rows.Length <= 0)
            {
                return;
            }

            string[] labels = new string[rows.Length];
            double[] y = new double[rows.Length];
            double[] y2 = new double[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                labels[i] = (string)rows[i]["productName"];
                int minCount = int.Parse(rows[i]["minCount"].ToString());
                if (minCount <= 0)
                {
                    y[i] = 0.1;
                }
                else
                {
                    y[i] = (double)minCount;
                }
                int count = int.Parse(rows[i]["count"].ToString());
                if (count <= 0)
                {
                    y2[i] = 0.1;
                }
                else
                {
                    y2[i] = (double)count;
                }
            }


            // Generate a red bar with "Curve 1" in the legend
            BarItem myBar = myPane.AddBar("安全库存", null, y, Color.Red);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);

            // Generate a blue bar with "Curve 2" in the legend
            myBar = myPane.AddBar("当前库存", null, y2, Color.Blue);
            myBar.Bar.Fill = new Fill(Color.Blue, Color.White, Color.Blue);

            // Draw the X tics between the labels instead of 
            // at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = labels;
            //// Set the XAxis to Text type
            //myPane.XAxis.Type = AxisType.Text;

            //// Fill the Axis and Pane backgrounds
            //myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            //myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

            //myPane.Title.Text = "库存预警商品信息";
            //myPane.XAxis.Title.Text = "商品名称";
            //myPane.YAxis.Title.Text = "商品数量";
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zg1.AxisChange();
            this.zedGraphControl1.Refresh();

        }
    }
}
