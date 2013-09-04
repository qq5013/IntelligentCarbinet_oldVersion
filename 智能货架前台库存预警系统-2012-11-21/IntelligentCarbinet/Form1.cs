using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CarbinetNM;
using System.Diagnostics;
using PublicConfig;
using httpHelper;
using fastJSON;

namespace IntelligentCarbinet
{
    delegate void deleControlInvoke(object o);
    public partial class Form1 : Form, ITagListener
    {
        #region 类成员

        List<Carbinet> groups = new List<Carbinet>();
        List<Button> legendButtonList = new List<Button>();
        List<Label> legendLabelList = new List<Label>();
        //X=0,Y=0,Width=1280,Height=770
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        EpcProductHelper current_helper = null;
        List<EpcProductHelper> helperList = new List<EpcProductHelper>();
        int listening_port1 = 5001;
        int listening_port2 = 5002;
        int listening_port3 = 5003;
        int listening_port4 = 5004;
        int alpha = 160;

        string strClickedEpc = string.Empty;
        #endregion

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);

            initialHelperList();

            this.cmbProductName.Items.Clear();
            this.cmbProductName.Items.Add("无");
            this.cmbProductName.SelectedIndex = 0;

            this.cmbCarbinetIndex.Items.Add("货架一");
            this.cmbCarbinetIndex.Items.Add("货架二");
            //this.cmbCarbinetIndex.Items.Add("货架三");
            //this.cmbCarbinetIndex.Items.Add("货架四");
            this.cmbCarbinetIndex.SelectedIndex = 0;

            this.FormClosing += new FormClosingEventHandler(SGSserverForm_FormClosing);
            this.Shown += new EventHandler(Form1_Shown);


            //this.lblLocation.Text = string.Format("hp = {0} wp = {1}", staticClass.heightPara, staticClass.widthPara);
            this.lblStatus.Text = "";

            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.DoWork +=
               new DoWorkEventHandler(backgroundWorker1_DoWork);
            //this.helper.listener = this;



        }
        void initialHelperList()
        {
            EpcProductHelper helper1 = new EpcProductHelper(this.listening_port1);
            this.helperList.Add(helper1);
            EpcProductHelper helper2 = new EpcProductHelper(this.listening_port2);
            this.helperList.Add(helper2);
            EpcProductHelper helper3 = new EpcProductHelper(this.listening_port3);
            this.helperList.Add(helper3);
            EpcProductHelper helper4 = new EpcProductHelper(this.listening_port4);
            this.helperList.Add(helper4);
        }
        void ClearListenerAndStopRunning()
        {
            if (this.current_helper != null)
            {
                this.current_helper.listener = null;
                this.current_helper.stop();
            }
            //foreach (EpcProductHelper eph in helperList)
            //{
            //    eph.listener = null;
            //    eph.stop();
            //}
        }
        public void NewTagMessage()
        {
            deleControlInvoke dele = delegate(object o)
            {
                List<EpcProduct> tagList = current_helper.GetCurrentProductList();
                if (tagList.Count > 0)
                {
                    foreach (EpcProduct ep in tagList)
                    {
                        Debug.WriteLine(string.Format("epc => {0}   antena => {1}", ep.tagInfo.epc, ep.tagInfo.antennaID));
                    }
                    #region 检查标签是否应该放在放在的正确的货架
                    //int count4InWrongCarbinet = 0;
                    //foreach (EpcProduct t in tagList)
                    //{
                    //    string tEpc = t.epc;
                    //    if (tEpc.Substring(16, 2) != carbinet_epc_flag)
                    //    {
                    //        count4InWrongCarbinet++;
                    //    }
                    //}
                    //if (count4InWrongCarbinet > 0)
                    //{
                    //    this.lblStatus.Text = "有不属于本货架的产品放置在本货架中，请检查！";
                    //}
                    //else
                    //{
                    //    this.lblStatus.Text = string.Empty;
                    //}
                    #endregion
                }
                else
                {
                    Debug.WriteLine("no products now!!!");
                }

                #region 首先把不再存在的标签从列表中删除
                //
                DataRow[] allRows = MemoryTable.unitTable.Select();
                for (int j = 0; j < allRows.Length; j++)
                {
                    DataRow dr = allRows[j];
                    string epc = (string)dr["epc"];
                    int floor = int.Parse(dr["floor"].ToString());
                    if (epc != null && epc.Length > 0)
                    {
                        bool bFinded = false;
                        foreach (EpcProduct ti in tagList)
                        {
                            if ((ti.tagInfo.epc == epc) && (this.getFloorByAntennaID(ti.tagInfo.antennaID) == floor))//这里的不存在应该和位置相关
                            {
                                bFinded = true;
                                break;
                            }
                        }
                        if (bFinded == false)//如果列表中不再存在这个标签，就把之前标签占据的位置的状态设为delete
                        {
                            dr["action"] = "delete";
                            MemoryTable.staticsTable.Rows.Add(new object[] { epc, "下架", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                        }
                    }
                }
                #endregion

                #region 添加新出现的标签到显示列表中
                // 
                for (int i = 0; i < tagList.Count; i++)
                {
                    EpcProduct ti = tagList[i];
                    string epc = ti.tagInfo.epc;
                    //if (epc.Substring(16, 2) != carbinet_epc_flag)
                    //{
                    //    continue;
                    //}
                    int anticipatedFloor = 0;
                    try
                    {
                        string strFloor = this.getAnticipatedFloorByEpc(epc);// 18,2
                        anticipatedFloor = int.Parse(strFloor);
                    }
                    catch
                    {

                    }
                    if (anticipatedFloor == 0)
                    {
                        continue;
                    }
                    int tagFloor = this.getFloorByAntennaID(ti.tagInfo.antennaID);

                    bool bOnWrongFloor = false;//是否放置在错误的货架层上
                    DataRow[] drs = MemoryTable.unitTable.Select("epc='" + epc + "' and floor=" + tagFloor.ToString());
                    if (drs.Length <= 0)//说明这个标签还不存在
                    {
                        //Debug.WriteLine("antinna To floor ->  antenna = " + ti.antennaID);
                        int antenna = int.Parse(ti.tagInfo.antennaID);//todo  暂时把设备的天线和货架的层数挂钩
                        int floor = -1;
                        switch (antenna)
                        {
                            case 1:
                                floor = 1;
                                break;
                            case 2:
                                floor = 2;
                                break;
                            case 4:
                                floor = 3;
                                break;
                            case 8:
                                floor = 4;
                                break;
                        }
                        if (floor != anticipatedFloor)
                        {
                            Debug.WriteLine(string.Format("antinna To floor ->  floor = {0}   anticipatedFloor = {1}", floor.ToString(), anticipatedFloor.ToString()));
                            bOnWrongFloor = true;//设计放产品的层和实际的层不一致
                        }
                        if (floor != -1)
                        {
                            DataRow[] emptyRows = MemoryTable.unitTable.Select("status = 'empty' and floor=" + floor.ToString());
                            if (emptyRows.Length > 0)
                            {
                                Debug.WriteLine(
                                    string.Format("Form1._timer_Tick  -> equipID = {0}"
                                    , emptyRows[0]["id"]));
                                emptyRows[0]["epc"] = epc;
                                emptyRows[0]["status"] = "occupied";
                                if (bOnWrongFloor == true)
                                {
                                    emptyRows[0]["action"] = "addOnWrongFloor";
                                }
                                else
                                {
                                    emptyRows[0]["action"] = "add";
                                }
                                if (epc != null && epc.Length >= 23)
                                {
                                    string type = epc.Substring(20, 2);

                                    emptyRows[0]["type"] = type;
                                }

                            }
                        }
                        MemoryTable.staticsTable.Rows.Add(new object[] { epc, "上架", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    }
                }

                #endregion
                #region 其它逻辑处理

                //
                // 1 搜索某类商品
                string productName = this.cmbProductName.Text;
                //            string productName = this.txtProductName.Text;
                if (productName != null && productName.Length > 0 && productName != "无")
                {
                    DataRow[] rowsProduct = MemoryTable.typeMapTable.Select("productName <>'" + productName + "'");
                    for (int j = 0; j < rowsProduct.Length; j++)
                    {
                        DataRow dr = rowsProduct[j];
                        string type = (string)dr["type"];
                        DataRow[] rowsFilter = MemoryTable.unitTable.Select("type = '" + type + "'");
                        for (int k = 0; k < rowsFilter.Length; k++)
                        {
                            DataRow drFilter = rowsFilter[k];

                            //drFilter["status"] = "empty";
                            drFilter["action"] = "delete";
                        }
                    }
                }

                #endregion


                #region 处理显示
                //
                //需要删除的
                DataRow[] rows = MemoryTable.unitTable.Select("action = 'delete'");
                if (rows.Length > 0)
                {
                    List<epcInfo> epcList = new List<epcInfo>();
                    for (int i = 0; i < rows.Length; i++)
                    {

                        string id = (string)rows[i]["id"];
                        string epc = (string)rows[i]["epc"];
                        string type = (string)rows[i]["type"];
                        epcInfo epcinfo = new epcInfo(epc, type);
                        epcList.Add(epcinfo);
                        if (id != null && id.Length > 0)
                        {
                            int groupIndex = int.Parse(rows[i]["group"].ToString());
                            Carbinet _carbinet = this.groups[groupIndex];
                            DocumentFile df = _carbinet.getDoc(id);
                            if (df != null)
                            {
                                df.setBackgroundImage(null);
                                df.setBorderWidth(0);
                                df.indexBase = "z" + df.columnNumber.ToString("0000");//重置为基础的排序依据
                            }
                        }

                        rows[i]["action"] = "normal";
                        rows[i]["epc"] = "";
                        rows[i]["type"] = "";
                        rows[i]["status"] = "empty";
                    }

                    //TODO 处于防盗模式下需要语音提醒，处于讲解模式下需要对产品进行讲解
                    if (staticClass.mode == MonitorMode.防盗模式)
                    {
                        this.SpeakAboutSomething(epcList);
                    }
                    //else
                    //    if (staticClass.mode == MonitorMode.讲解模式)
                    //    {
                    //        for (int i = 0; i < epcList.Count; i++)
                    //        {
                    //            epcInfo ei = epcList[i];
                    //            MemoryTable.staticsTable.Rows.Add(new object[] { ei.epc, "下架", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    //        }
                    //    }
                }
                // 需要增加的
                rows = MemoryTable.unitTable.Select("action = 'add' OR action = 'addOnWrongFloor'");
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {

                        int groupIndex = int.Parse(rows[i]["group"].ToString());
                        string id = (string)rows[i]["id"];
                        string epc = (string)rows[i]["epc"];
                        string action = (string)rows[i]["action"];
                        bool bOnWrongFloor = false;
                        if (action == "addOnWrongFloor")
                        {
                            bOnWrongFloor = true;
                        }
                        Carbinet _carbinet = this.groups[groupIndex];
                        // 不同种类的物品设置成不同的大小
                        string type = MemoryTable.getProductTypeID(epc);

                        DataRow[] drsType = MemoryTable.typeMapTable.Select("type='" + type + "'");
                        if (drsType.Length > 0)
                        {
                            int width = int.Parse(drsType[0]["width"].ToString());
                            int height = int.Parse(drsType[0]["height"].ToString());
                            string path = (string)drsType[0]["picname"];
                            path = PublicConfig.staticClass.PicturePath + path;
                            Image newImage = null;
                            try
                            {
                                newImage = Image.FromFile(path);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine(
                                    string.Format("Form1._timer_Tick  -> loadImage error = {0}"
                                    , ex.Message));
                            }
                            DocumentFile df = _carbinet.getDoc(id);
                            if (df != null)
                            {
                                df.setBackgroundImage(newImage);
                                if (bOnWrongFloor == true)
                                {
                                    df.setBorderWidth(5);
                                }
                                //df.setBorderWidth(0);//边框重置为0
                                df.indexBase = type;
                            }
                            //_carbinet.setDocBGImage(id, newImage);

                            _carbinet.setDocSize(id, width, height);

                            //drsType[0]["count"] = ((int)drsType[0]["count"]) + 1;

                        }
                        rows[i]["action"] = "normal";
                    }
                }

                #endregion



                #region 设置饼图

                //
                DataRow[] typeRows = MemoryTable.typeMapTable.Select();
                decimal[] decimals = new decimal[typeRows.Length];
                string[] strs = new string[typeRows.Length];
                Color[] colors = new Color[typeRows.Length];
                for (int i = 0; i < typeRows.Length; i++)
                {
                    DataRow[] RowsTemp = MemoryTable.unitTable.Select("type = '" + (string)typeRows[i]["type"] + "'");
                    decimals[i] = RowsTemp.Length;
                    typeRows[i]["count"] = RowsTemp.Length;
                    //                decimals[i] = (int)typeRows[i]["count"];
                    strs[i] = RowsTemp.Length.ToString();
                    //                strs[i] = typeRows[i]["count"].ToString();
                    //strs[i] = typeRows[i]["productName"] + ":" + typeRows[i]["count"].ToString();
                    colors[i] = Color.FromArgb(alpha,
                        int.Parse(typeRows[i]["red"].ToString()),
                        int.Parse(typeRows[i]["green"].ToString()),
                        int.Parse(typeRows[i]["blue"].ToString()));
                }
                m_panelDrawing.Values = decimals;
                m_panelDrawing.Texts = strs;
                m_panelDrawing.Colors = colors;
                #endregion

            };

            this.Invoke(dele, new object());
        }
        #region 内部逻辑处理
        void ClearControls()
        {
            #region 清空货架 清空table
            DataRow[] rows = MemoryTable.unitTable.Select("status = 'occupied'");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    string id = (string)rows[i]["id"];
                    if (id != null && id.Length > 0)
                    {
                        int groupIndex = int.Parse(rows[i]["group"].ToString());
                        Carbinet _carbinet = this.groups[groupIndex];
                        DocumentFile df = _carbinet.getDoc(id);
                        if (df != null)
                        {
                            df.setBackgroundImage(null);
                            df.setBorderWidth(0);
                            df.indexBase = "z" + df.columnNumber.ToString("0000");//重置为基础的排序依据
                        }
                    }

                    rows[i]["action"] = "normal";
                    rows[i]["epc"] = "";
                    rows[i]["type"] = "";
                    rows[i]["status"] = "empty";
                }
            }
            #endregion

            #region 初始化饼图
            m_panelDrawing.Values = new decimal[] { 0, 0 };
            // m_panelDrawing.Values = new decimal[] { 0 };

            m_panelDrawing.Colors = new Color[] { Color.FromArgb(alpha, Color.Gray), Color.FromArgb(alpha, Color.GreenYellow) };
            //m_panelDrawing.SliceRelativeDisplacements = new float[] { 0.1F, 0.2F, 0.2F, 0.2F };
            m_panelDrawing.Texts = new string[] { " ", " " };
            //            m_panelDrawing.Texts = new string[] { "未知", "A", "B", "C", "D" };
            m_panelDrawing.ToolTips = new string[] { "", "" };
            #endregion
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
            string id = (string)e.Argument;
            if (id == null || id.Length <= 0)
            {
                return;
            }
            DataRow[] drs1 = MemoryTable.unitTable.Select("id='" + id + "'");
            if (drs1.Length > 0)
            {

                string epc = (string)drs1[0]["epc"];
                if (epc == null || epc.Length <= 0)
                {
                    return;
                }
                string type = (string)drs1[0]["type"];
                DataRow[] drs2 = MemoryTable.typeMapTable.Select("type='" + type + "'");
                if (drs2.Length > 0)
                {
                    string description = (string)drs2[0]["description"];
                    CommandInfo c = new CommandInfo("tts", description, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
                    string jsonString = string.Empty;
                    jsonString = JSON.Instance.ToJSON(c);
                    HttpWebConnect helper = new HttpWebConnect();
                    //helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
                    string url = staticClass.getRestAddress();
                    //string url = staticClass.RestAddress;
                    helper.TryPostData(url, jsonString);
                }
            }

        }

        string getAnticipatedFloorByEpc(string epc)
        {
            if (epc == null && epc.Length < 24)
            {
                return string.Empty;
            }
            return epc.Substring(18, 2);
        }
        int getFloorByAntennaID(string _antenna)
        {
            int floor = -1;
            int antenna = int.Parse(_antenna);//todo  暂时把设备的天线和货架的层数挂钩
            switch (antenna)
            {
                case 1:
                    floor = 1;
                    break;
                case 2:
                    floor = 2;
                    break;
                case 4:
                    floor = 3;
                    break;
                case 8:
                    floor = 4;
                    break;
            }
            return floor;
        }
        void SpeakAboutSomething(List<epcInfo> epcList)
        {
            if (epcList == null || epcList.Count <= 0)
            {
                return;
            }
            string jsonString = string.Empty;
            string tempToSpeak = string.Empty;
            CommandInfo c = null;
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_addLedInfo);
            string url = staticClass.getRestAddress();
            //string url = staticClass.RestAddress;
            switch (staticClass.mode)
            {
                case MonitorMode.防盗模式:
                    tempToSpeak = "编号为 ";
                    foreach (epcInfo s in epcList)
                    {
                        tempToSpeak = tempToSpeak + s.epc + ",";
                    }
                    tempToSpeak += "的产品 已不在货架，请检查";
                    Debug.WriteLine(
                                    string.Format("Form1.SpeakAboutSomething  -> 防盗模式 = {0}"
                                    , tempToSpeak));
                    c = new CommandInfo("tts", tempToSpeak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
                    jsonString = JSON.Instance.ToJSON(c);
                    helper.TryPostData(url, jsonString);
                    break;
                case MonitorMode.讲解模式:
                    epcInfo epc = epcList[0];
                    DataRow[] rows = null;
                    string type = epc.type;
                    rows = MemoryTable.typeMapTable.Select("type = '" + type + "'");
                    if (rows.Length > 0)
                    {
                        tempToSpeak += (string)rows[0]["description"];
                        Debug.WriteLine(
                            string.Format("Form1.SpeakAboutSomething  -> 讲解模式 = {0}"
                            , tempToSpeak));
                        c = new CommandInfo("tts", tempToSpeak, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "192.168.0.98");
                        jsonString = JSON.Instance.ToJSON(c);
                        helper.TryPostData(url, jsonString);
                    }
                    break;
            }

        }
        void helper_RequestCompleted_addLedInfo(object o)
        {
            string strLedInfo = (string)o;
            Debug.WriteLine(
                string.Format("Form1.helper_RequestCompleted_addLedInfo  ->  = {0}"
                , strLedInfo));
            CommandInfo u2 = fastJSON.JSON.Instance.ToObject<CommandInfo>(strLedInfo);
            Debug.WriteLine(
                string.Format("Form1.helper_RequestCompleted_addLedInfo  ->  = {0}"
                , u2.info));
        }

        void update_cabinet_data(UDPServer server, TDJ_RFIDHelper helper, int carbinet_index)
        {
            string carbinet_epc_flag = (carbinet_index + 1).ToString("D2");//  01 02
            server.Manualstate.WaitOne();
            server.Manualstate.Reset();
            string temp1 = server.sbuilder.ToString();

            server.sbuilder.Remove(0, temp1.Length);

            server.Manualstate.Set();

            //if (temp1 != null && temp1.Length >= 0)
            //{
            //this.txtLog.Text = str + "\r\n" + this.txtLog.Text;
            //Debug.WriteLine(
            //    string.Format(".  _timer_Tick -> string = {0}"
            //    , str));
            //}
            //处理列表
            List<TagInfo> tagList = helper.ParseDataToTag(temp1);

            #region 检查标签是否应该放在放在的正确的货架
            int count4InWrongCarbinet = 0;
            foreach (TagInfo t in tagList)
            {
                string tEpc = t.epc;
                if (tEpc.Substring(16, 2) != carbinet_epc_flag)
                {
                    count4InWrongCarbinet++;
                }
            }
            if (count4InWrongCarbinet > 0)
            {
                this.lblStatus.Text = "有不属于本货架的产品放置在本货架中，请检查！";
            }
            else
            {
                this.lblStatus.Text = string.Empty;
            }
            #endregion


            #region 首先把不再存在的标签从列表中删除


            //
            DataRow[] allRows = MemoryTable.unitTable.Select("group = " + carbinet_index.ToString());
            for (int j = 0; j < allRows.Length; j++)
            {
                DataRow dr = allRows[j];
                string epc = (string)dr["epc"];
                int floor = int.Parse(dr["floor"].ToString());
                if (epc != null && epc.Length > 0)
                {
                    bool bFinded = false;
                    foreach (TagInfo ti in tagList)
                    {
                        if ((ti.epc == epc) && (this.getFloorByAntennaID(ti.antennaID) == floor))//这里的不存在应该和位置相关
                        {
                            bFinded = true;
                            break;
                        }
                    }
                    if (bFinded == false)//如果列表中不再存在这个标签，就把之前标签占据的位置的状态设为delete
                    {
                        dr["action"] = "delete";
                    }
                }

            }


            #endregion

            #region 添加新出现的标签到显示列表中

            // 
            for (int i = 0; i < tagList.Count; i++)
            {
                TagInfo ti = tagList[i];
                string epc = ti.epc;
                if (epc.Substring(16, 2) != carbinet_epc_flag)
                {
                    continue;
                }
                int anticipatedFloor = 0;
                try
                {
                    string strFloor = this.getAnticipatedFloorByEpc(epc);// 18,2
                    anticipatedFloor = int.Parse(strFloor);
                }
                catch (System.Exception ex)
                {

                }
                if (anticipatedFloor == 0)
                {
                    continue;
                }
                int tagFloor = this.getFloorByAntennaID(ti.antennaID);

                bool bOnWrongFloor = false;//是否放置在错误的货架层上
                DataRow[] drs = MemoryTable.unitTable.Select("epc='" + epc + "' and floor=" + tagFloor.ToString() + " and group = " + carbinet_index.ToString());
                if (drs.Length <= 0)//说明这个标签还不存在
                {
                    Debug.WriteLine("antinna To floor ->  antenna = " + ti.antennaID);
                    int antenna = int.Parse(ti.antennaID);//todo  暂时把设备的天线和货架的层数挂钩
                    int floor = -1;
                    switch (antenna)
                    {
                        case 1:
                            floor = 1;
                            break;
                        case 2:
                            floor = 2;
                            break;
                        case 4:
                            floor = 3;
                            break;
                        case 8:
                            floor = 4;
                            break;
                    }
                    if (floor != anticipatedFloor)
                    {
                        Debug.WriteLine(string.Format("antinna To floor ->  floor = {0}   anticipatedFloor = {1}", floor.ToString(), anticipatedFloor.ToString()));
                        bOnWrongFloor = true;//设计放产品的层和实际的层不一致
                    }
                    if (floor != -1)
                    {
                        DataRow[] emptyRows = MemoryTable.unitTable.Select("status = 'empty' and floor=" + floor.ToString() + " and group = " + carbinet_index.ToString());
                        if (emptyRows.Length > 0)
                        {
                            Debug.WriteLine(
                                string.Format("Form1._timer_Tick  -> equipID = {0}"
                                , emptyRows[0]["id"]));
                            emptyRows[0]["epc"] = epc;
                            emptyRows[0]["status"] = "occupied";
                            if (bOnWrongFloor == true)
                            {
                                emptyRows[0]["action"] = "addOnWrongFloor";
                            }
                            else
                            {
                                emptyRows[0]["action"] = "add";
                            }
                            if (epc != null && epc.Length >= 23)
                            {
                                string type = epc.Substring(20, 2);

                                emptyRows[0]["type"] = type;
                            }

                        }


                    }
                }
            }

            #endregion


            #region 其它逻辑处理

            //
            // 1 搜索某类商品
            string productName = this.cmbProductName.Text;
            //            string productName = this.txtProductName.Text;
            if (productName != null && productName.Length > 0 && productName != "无")
            {
                DataRow[] rowsProduct = MemoryTable.typeMapTable.Select("productName <>'" + productName + "'");
                for (int j = 0; j < rowsProduct.Length; j++)
                {
                    DataRow dr = rowsProduct[j];
                    string type = (string)dr["type"];
                    DataRow[] rowsFilter = MemoryTable.unitTable.Select("type = '" + type + "'");
                    for (int k = 0; k < rowsFilter.Length; k++)
                    {
                        DataRow drFilter = rowsFilter[k];

                        //drFilter["status"] = "empty";
                        drFilter["action"] = "delete";
                    }
                }
            }

            #endregion

        }

        #endregion
        #region 事件处理

        void SGSserverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.current_helper.stop();

            DataRow[] rows = MemoryTable.unitTable.Select();
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i]["action"] = "normal";
                rows[i]["epc"] = "";
                rows[i]["type"] = "";
                rows[i]["status"] = "empty";
            }
        }
        void Form1_Shown(object sender, EventArgs e)
        {
            this.InitializePanelControl();

            //饼图添加图例
            int initialTop = staticClass.getRealHeight(500);
            DataRow[] typeRows = MemoryTable.typeMapTable.Select();
            for (int i = 0; i < typeRows.Length; i++, initialTop = initialTop + staticClass.getRealHeight(45))
            {
                Button btnLegend = new Button();
                btnLegend.FlatStyle = FlatStyle.Flat;
                btnLegend.FlatAppearance.BorderSize = 0;
                btnLegend.Width = staticClass.getRealWidth(70);
                btnLegend.Height = staticClass.getRealHeight(23);
                btnLegend.BackColor = Color.FromArgb(alpha,
                    int.Parse(typeRows[i]["red"].ToString()),
                    int.Parse(typeRows[i]["green"].ToString()),
                    int.Parse(typeRows[i]["blue"].ToString()));
                btnLegend.Left = staticClass.getRealWidth(900);
                btnLegend.Top = initialTop;
                this.Controls.Add(btnLegend);
                this.legendButtonList.Add(btnLegend);

                Label lblLegend = new Label();
                lblLegend.Left = staticClass.getRealWidth(990);
                lblLegend.Top = initialTop + 8;
                //this.lblLocation.Location = new System.Drawing.Point(855, initialTop + 5);
                lblLegend.Text = (string)typeRows[i]["productName"];
                this.Controls.Add(lblLegend);
                this.legendLabelList.Add(lblLegend);
            }


            //在下拉列表中增加内容
            for (int i = 0; i < typeRows.Length; i++)
            {
                this.cmbProductName.Items.Add((string)typeRows[i]["productName"]);
            }

            this.current_helper.start(this.listening_port1);

        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            this.lblLocation.Text = string.Format("X = {0}   Y = {1}", e.X, e.Y);
        }

        // 29 137
        // 166 282
        // 311 427
        // 456 562
        // 25 553

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int width1 = this.pictureBox1.Width;
            int height1 = this.pictureBox1.Height;
            this.paint_carbinet_background(e.Graphics, number_of_row, width1, height1);

            this.InitialCarbinet(this.pictureBox1.Controls, 0);
        }
        void df_Click(object sender, EventArgs e)
        {
            DocumentFile df = (DocumentFile)sender;

            //播放语音
            backgroundWorker1.RunWorkerAsync(df.name);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmRemind rmd = new frmRemind();
            rmd.ShowDialog();
        }
        void txtProductName_Click(object sender, System.EventArgs e)
        {
            if (this.txtProductName.Text == "产品名称")
            {
                this.txtProductName.Text = string.Empty;
            }
        }

        private void cmbCarbinetIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ClearListenerAndStopRunning();
            this.ClearControls();

            this.current_helper = helperList[this.cmbCarbinetIndex.SelectedIndex];
            this.current_helper.start(this);
        }

        #endregion
        private void InitializePanelControl()
        {
            m_panelDrawing.LeftMargin = 10;
            m_panelDrawing.RightMargin = 10;
            m_panelDrawing.TopMargin = 10;
            m_panelDrawing.BottomMargin = 10;
            m_panelDrawing.FitChart = true;
            m_panelDrawing.EdgeLineWidth = 1;
            m_panelDrawing.Values = new decimal[] { 0, 0 };
            // m_panelDrawing.Values = new decimal[] { 0 };

            m_panelDrawing.Colors = new Color[] { Color.FromArgb(alpha, Color.Gray), Color.FromArgb(alpha, Color.GreenYellow) };
            //m_panelDrawing.SliceRelativeDisplacements = new float[] { 0.1F, 0.2F, 0.2F, 0.2F };
            m_panelDrawing.Texts = new string[] { " ", " " };
            //            m_panelDrawing.Texts = new string[] { "未知", "A", "B", "C", "D" };
            m_panelDrawing.ToolTips = new string[] { "", "" };
            m_panelDrawing.Font = new Font("Arial", 10F);
            m_panelDrawing.ForeColor = SystemColors.WindowText;
            m_panelDrawing.SliceRelativeHeight = 0.1F;
            m_panelDrawing.InitialAngle = -90F;
        }

        #region 货架绘制
        /// <summary>
        /// 绘制货架背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="number_of_row">货架层数</param>
        /// <param name="width">货架总宽度</param>
        /// <param name="height">货架总高度</param>
        void paint_carbinet_background(Graphics g, int number_of_row, int width, int height)
        {
            //设定顶层、底层以及中间的隔层高度相同
            int top_height = 30;
            int bottom_height = 30;
            //int height_of_gap = 20;
            //侧边厚度
            int broadslide_width = this.broadslide_width;
            // Make a big red pen.
            Pen p_top = new Pen(Color.FromArgb(160, 160, 160), top_height);
            Pen p_bottom = new Pen(Color.FromArgb(160, 160, 160), bottom_height);
            Pen p = new Pen(Color.FromArgb(160, 160, 160), height_of_gap);
            Pen p_slide = new Pen(Color.FromArgb(180, 180, 180), broadslide_width);
            g.DrawLine(p_slide, broadslide_width / 2, 1, broadslide_width / 2, height);//左边
            g.DrawLine(p_slide, width - broadslide_width / 2, 1, width - broadslide_width / 2, height);//右边
            g.DrawLine(p_top, 0, height_of_gap / 2, width, height_of_gap / 2);//顶
            g.DrawLine(p_bottom, 1, height - height_of_gap / 2, width, height - height_of_gap / 2);//底

            //可用宽度
            work_width = width - broadslide_width * 2;
            //可用高度
            work_height = height - top_height - bottom_height;
            //柜子的层数
            //int number_of_row = 3;
            //需要在中间绘制  number_of_row -1 个隔层
            // 计算隔层的位置
            // 首先从可用高度中减去隔层所占的高度
            work_height = work_height - (number_of_row - 1) * height_of_gap;
            //每层的高度
            height_of_row = work_height / number_of_row;
            //层的top属性，第二层的就是加上每层的高度和隔层的高度
            this.top_of_first_row = top_height;
            int row_top = top_height + height_of_row + height_of_gap / 2;//每层的top属性
            //中间隔层
            for (int i = 1; i < number_of_row; i++, row_top = row_top + height_of_row + height_of_gap)
            {
                g.DrawLine(p, broadslide_width, row_top, width - broadslide_width, row_top);
            }
        }
        #region 货架样式参数
        int work_width = 0;
        int work_height = 0;
        int height_of_row = 0;
        int height_of_gap = 20;
        int number_of_row = 4;
        int top_of_first_row = 0;
        int broadslide_width = 20;
        #endregion

        private void InitialCarbinet(Control.ControlCollection controls, int carbinet_index)
        {
            //防止重复操作
            if (controls.Count <= 0)
            //if (this.pictureBox1.Controls.Count <= 0)
            {

                //Control.ControlCollection controls = this.pictureBox1.Controls;
                //货架数目
                int numberOfGroup = 1;
                //整个可用空间的宽度
                int widthOfRoom = this.work_width;
                //隔层的高度
                int floorGap = height_of_gap;
                //总共有多少列
                int totalColumns = 10;
                //总共要分多少列，包括两个货架之间，货架之间的间隔暂时采用和普通列同一宽度的做法
                int numberOfUnit = totalColumns + numberOfGroup - 1;
                // 可以计算出没一列的宽度
                int widthOfUnit = widthOfRoom / numberOfUnit;
                // 货架上的物品区域距离货架左边缘的距离
                int groupInitialLeft = 0;//货架距离整个区域的左边缘的距离
                int groupWidth = (widthOfRoom - widthOfUnit * (numberOfGroup - 1)) / numberOfGroup;//计算每一个货架的宽度,需要减去货架之间间隔的宽度

                //for (int i = 0; i < numberOfGroup; i++)
                {
                    int numberofColumn = 20;//预设值，之后需要作为参数提取
                    int numberOfRow = number_of_row;

                    Carbinet group = new Carbinet(controls);
                    //Carbinet group = new Carbinet(this.pictureBox1.Controls);
                    group.Left = groupInitialLeft;
                    group.Top = 0;
                    if (carbinet_index == 0 && this.groups.Count > 0)//保证货架的顺序对应
                    {
                        this.groups.Insert(0, group);
                    }
                    else
                        this.groups.Add(group);
                    //初始化每一排的行
                    int initialTop = top_of_first_row;

                    for (int irow = 1; irow <= numberOfRow; irow++, initialTop = initialTop + (int)(height_of_row + floorGap))
                    {
                        CarbinetFloor row = new CarbinetFloor(group, irow, controls);
                        //CarbinetFloor row = new CarbinetFloor(group, irow, this.pictureBox1.Controls);
                        row.Width = groupWidth;
                        row.Height = height_of_row;
                        row.relativeTop = initialTop;
                        row.relativeLeft = this.broadslide_width;

                        group.AddFloor(row);

                        for (int k = 1; k <= numberofColumn; k++)
                        {
                            string _equipmentID = carbinet_index.ToString() + "," + irow.ToString() + "," + k.ToString();

                            DocumentFile df = new DocumentFile(_equipmentID, irow);
                            df.Width = widthOfUnit;
                            df.Height = height_of_row;
                            df.carbinetIndex = carbinet_index;
                            df.floorNumber = irow;
                            df.columnNumber = k;
                            df.indexBase = "z" + k.ToString("0000");

                            MemoryTable.unitTable.Rows.Add(new object[] { _equipmentID, "", carbinet_index, irow, "empty", "normal" });

                            df.Click += new EventHandler(df_Click);
                            group.AddDocFile(df);
                        }

                    }
                    groupInitialLeft += groupWidth + widthOfUnit;
                }
            }
        }

        #endregion
    }

}
