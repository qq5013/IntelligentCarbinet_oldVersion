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
using Server;
using InventoryMSystem;
using System.Diagnostics;
using fastJSON;
using httpHelper;
using Log;

namespace IntelligentCarbinet
{
    public delegate void deleControlInvokeNull();
    public partial class frmRemind : Form
    {

        Timer _timer = new Timer();
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

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
            _timer.Interval = 2000;//5000;
            //_timer.Interval = staticClass.storageCheckInterval;//5000;
            _timer.Tick += new EventHandler(_timer_Tick);

            this.Shown += new EventHandler(frmRemind_Shown);
            this.FormClosing += new FormClosingEventHandler(frmRemind_FormClosing);



            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.DoWork +=
               new DoWorkEventHandler(backgroundWorker1_DoWork);
        }

        void frmRemind_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Tick -= (_timer_Tick);
            _timer.Enabled = false;
        }
        TDJ_RFIDHelper helper = new TDJ_RFIDHelper();

        void _timer_Tick(object sender, EventArgs e)
        {
            //要把数据处理一下
            UDPServer.Manualstate.WaitOne();
            UDPServer.Manualstate.Reset();
            string str = UDPServer.sbuilder.ToString();
            UDPServer.sbuilder.Remove(0, str.Length);

            UDPServer.Manualstate.Set();
            this.helper.ParseDataToTag(str);
            if (str != null && str.Length >= 0)
            {
                //this.txtLog.Text = str + "\r\n" + this.txtLog.Text;
                //Debug.WriteLine(
                //    string.Format(".  _timer_Tick -> string = {0}"
                //    , str));
            }
            //处理列表
            List<TagInfo> tagList = this.helper.getTagList();//先拿到读取到的标签
            if (tagList.Count <= 0)
            {
                return;
            }
            MemoryTable.remindTable.Rows.Clear();
            //首先把不再存在的标签从列表中删除
            //DataRow[] allRows = MemoryTable.remindTable.Select();
            //for (int j = 0; j < allRows.Length; j++)
            //{
            //    DataRow dr = allRows[j];
            //    string epc = (string)dr["epc"];
            //    if (epc != null && epc.Length > 0)
            //    {
            //        bool bFinded = false;
            //        foreach (TagInfo ti in tagList)
            //        {
            //            if (ti.epc == epc)
            //            {
            //                bFinded = true;
            //                break;
            //            }
            //        }
            //        if (bFinded == false)
            //        {
            //            dr["action"] = "delete";
            //        }
            //    }
            //}
            ////删除掉标识为 delete 的行
            //DataRow[] deletedRows = MemoryTable.remindTable.Select("action = 'delete'");
            //if (deletedRows.Length > 0)
            //{
            //    for (int i = 0; i < deletedRows.Length; i++)
            //    {
            //        MemoryTable.remindTable.Rows.Remove(deletedRows[i]);
            //    }
            //}

            // 添加新出现的标签到显示列表中
            for (int i = 0; i < tagList.Count; i++)
            {
                TagInfo ti = tagList[i];
                string epc = ti.epc;
                if (epc != null && epc.Length >= 23)
                {
                    string type = epc.Substring(20, 2);//取得标签标识的产品类型
                    DataRow[] drs = MemoryTable.remindTable.Select("epc='" + epc + "'");
                    if (drs.Length <= 0)//说明这个标签还不存在
                    {
                        DataRow[] unitTypeRows = MemoryTable.typeMapTable.Select("type = '" + type + "'");
                        if (unitTypeRows.Length > 0)
                        {
                            string productName = (string)unitTypeRows[0]["productName"];
                            MemoryTable.remindTable.Rows.Add(new object[] { epc, "add", type, productName });
                        }
                    }
                }
            }


            //计算每种产品的数量
            DataRow[] typeRows = MemoryTable.typeMapTable.Select();
            decimal[] decimals = new decimal[typeRows.Length];
            string[] strs = new string[typeRows.Length];
            //Color[] colors = new Color[typeRows.Length];
            for (int i = 0; i < typeRows.Length; i++)
            {
                DataRow[] RowsTemp = MemoryTable.remindTable.Select("type = '" + (string)typeRows[i]["type"] + "'");
                decimals[i] = RowsTemp.Length;
                typeRows[i]["count"] = RowsTemp.Length;
                strs[i] = RowsTemp.Length.ToString();
            }
            string strToSend = "当前库存：";
            string strToSpeakOut = "库存提醒：";
            bool blackOfInventory = false;
            for (int i = 0; i < MemoryTable.typeMapTable.Rows.Count; i++)
            {
                //if (int.Parse(MemoryTable.typeMapTable.Rows[i]["count"].ToString()) > 0)
                //{
                DataRow dr = MemoryTable.typeMapTable.Rows[i];
                string tmp = string.Format("{0} ：{1}", dr["productName"].ToString(), dr["count"].ToString());
                Debug.WriteLine(
                    string.Format("frmRemind._timer_Tick  -> {0}"
                    , tmp));
                strToSend = strToSend + "    " + tmp;
                int minCount = int.Parse(dr["minCount"].ToString());
                int crtCount = int.Parse(dr["count"].ToString());
                if (minCount > crtCount)
                {
                    blackOfInventory = true;
                    strToSpeakOut = strToSpeakOut + " " + dr["productName"].ToString() + " 缺货数量：" + (minCount - crtCount).ToString();
                }
                //}
            }

            string url = staticClass.addCommandInfo;
            if (blackOfInventory == true)
            {
                //CommandInfo ctts = new CommandInfo("tts", strToSpeakOut, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
                //string jsonStringToSpeak = string.Empty;
                //jsonStringToSpeak = JSON.Instance.ToJSON(ctts);
                //HttpWebConnect helper1 = new HttpWebConnect();
                //helper1.RequestCompleted += new deleGetRequestObject(helper1_RequestCompleted);
                ////helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
                //helper1.TryPostData(url, jsonStringToSpeak);

                //strToSend = strToSpeakOut + " " + strToSend;

                backgroundWorker1.RunWorkerAsync(strToSpeakOut);

            }
            CreateGraph_Chart(this.zedGraphControl1);  //图表
            //LedInfo c = new LedInfo(strToSend, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
            CommandInfo c = new CommandInfo("led", strToSend, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "172.16.13.99");
            string jsonString = string.Empty;
            jsonString = JSON.Instance.ToJSON(c);
            HttpWebConnect helper = new HttpWebConnect();
            //helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
            helper.TryPostData(url, jsonString);
            //if (this.InvokeRequired)
            //{
            //    deleControlInvokeNull dele = delegate()
            //    {
            //        CreateGraph_Chart(this.zedGraphControl1);  //图表
            //    };
            //    this.Invoke(dele);
            //}
            //else
            //{
            //    CreateGraph_Chart(this.zedGraphControl1);  //图表
            //}
        }
        private void backgroundWorker1_DoWork(object sender,
DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            string strToSpeakOut = (string)e.Argument;
            if (strToSpeakOut == null || strToSpeakOut.Length <= 0)
            {
                return;
            }
            CommandInfo ctts = new CommandInfo("tts", strToSpeakOut, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
            string jsonStringToSpeak = string.Empty;
            jsonStringToSpeak = JSON.Instance.ToJSON(ctts);
            HttpWebConnect helper1 = new HttpWebConnect();
            helper1.RequestCompleted += new deleGetRequestObject(helper1_RequestCompleted);
            string url = staticClass.addCommandInfo;
#if LOG_OUTPUT
            txtLog.LogError("frmRemind -> backgroundWorker1_DoWork  发送tts命令"
                            , null);
#endif
            helper1.TryPostData(url, jsonStringToSpeak);

            //strToSend = strToSpeakOut + " " + strToSend;

        }

        void helper1_RequestCompleted(object o)
        {
            string strLedInfo = (string)o;
            Debug.WriteLine(
                string.Format("Form1.helper1_RequestCompleted  ->  = {0}"
                , strLedInfo));
            int index = strLedInfo.IndexOf("{");
            if (index >= 0)
            {
                strLedInfo = strLedInfo.Substring(index);
            }
            CommandInfo u2 = fastJSON.JSON.Instance.ToObject<CommandInfo>(strLedInfo);
            Debug.WriteLine(
                string.Format("Form1.helper_RequestCompleted_addLedInfo  ->  = {0}    {1}"
                , u2.info, u2.state));
        }


        void frmRemind_Shown(object sender, EventArgs e)
        {
            UDPServer.startUDPListening();
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

            //设置的安全库存大于当前数量
            DataRow[] rows = MemoryTable.typeMapTable.Select("minCount > count");
            if (rows.Length > 0)
            {

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
                //this.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            this.zedGraphControl1.Refresh();
        }
    }
}
