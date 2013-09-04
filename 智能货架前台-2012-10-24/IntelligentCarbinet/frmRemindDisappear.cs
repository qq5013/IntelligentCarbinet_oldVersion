using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using httpHelper;
using fastJSON;
using PublicConfig;
using System.Diagnostics;

namespace IntelligentCarbinet
{
    public partial class frmRemindDisappear : Form
    {
        Timer _timer = new Timer();
        TDJ_RFIDHelper helper = new TDJ_RFIDHelper();
        UDPServer udp_server = new UDPServer();

        public frmRemindDisappear()
        {
            InitializeComponent();

            _timer.Interval = 2000;//5000;
            _timer.Tick += new EventHandler(_timer_Tick);

            this.FormClosing += new FormClosingEventHandler(frmRemindDisappear_FormClosing);
        }

        void frmRemindDisappear_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.udp_server.stop_listening();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            //要把数据处理一下
            this.udp_server.Manualstate.WaitOne();
            this.udp_server.Manualstate.Reset();
            string str = this.udp_server.sbuilder.ToString();
            this.udp_server.sbuilder.Remove(0, str.Length);

            this.udp_server.Manualstate.Set();
            this.helper.ParseDataToTag(str);
            if (str != null && str.Length >= 0)
            {
                //this.txtLog.Text = str + "\r\n" + this.txtLog.Text;
                //Debug.WriteLine(
                //    string.Format(".  _timer_Tick -> string = {0}"
                //    , str));
            }
            //处理列表
            //List<TagInfo> tagList = this.helper.getTagList();//先拿到读取到的标签
            //if (tagList.Count <= 0)
            //{
            //    return;
            //}
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
            //for (int i = 0; i < tagList.Count; i++)
            //{
            //    TagInfo ti = tagList[i];
            //    string epc = ti.epc;
            //    if (epc != null && epc.Length >= 23)
            //    {
            //        string type = epc.Substring(20, 2);//取得标签标识的产品类型
            //        DataRow[] drs = MemoryTable.remindTable.Select("epc='" + epc + "'");
            //        if (drs.Length <= 0)//说明这个标签还不存在
            //        {
            //            DataRow[] unitTypeRows = MemoryTable.typeMapTable.Select("type = '" + type + "'");
            //            if (unitTypeRows.Length > 0)
            //            {
            //                string productName = (string)unitTypeRows[0]["productName"];
            //                MemoryTable.remindTable.Rows.Add(new object[] { epc, "add", type, productName });
            //            }
            //        }
            //    }
            //}


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

            string url = staticClass.getRestAddress();
            //string url = staticClass.RestAddress;
            if (blackOfInventory == true)
            {
                CommandInfo ctts = new CommandInfo("tts", strToSpeakOut, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
                string jsonStringToSpeak = string.Empty;
                jsonStringToSpeak = JSON.Instance.ToJSON(ctts);
                HttpWebConnect helper1 = new HttpWebConnect();
                //helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
                helper1.TryPostData(url, jsonStringToSpeak);

                strToSend = strToSpeakOut + " " + strToSend;
            }
            //LedInfo c = new LedInfo(strToSend, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
            CommandInfo c = new CommandInfo("led", strToSend, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
            string jsonString = string.Empty;
            jsonString = JSON.Instance.ToJSON(c);
            HttpWebConnect helper = new HttpWebConnect();
            //helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
            helper.TryPostData(url, jsonString);
        }
    }
}
