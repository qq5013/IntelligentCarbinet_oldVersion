using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CarbinetNM;
using Server;
using InventoryMSystem;
using System.Diagnostics;

namespace IntelligentCarbinet
{
    public partial class Form1 : Form
    {
        List<Carbinet> groups = new List<Carbinet>();
        Timer _timer;
        TDJ_RFIDHelper helper = new TDJ_RFIDHelper();
        List<Button> legendButtonList = new List<Button>();
        List<Label> legendLabelList = new List<Label>();

        public Form1()
        {
            InitializeComponent();

            _timer = new Timer();
            _timer.Interval = 2000;
            _timer.Tick += new EventHandler(_timer_Tick);

            this.cmbProductName.Items.Clear();
            this.cmbProductName.Items.Add("无");
            this.cmbProductName.SelectedIndex = 0;

            this.initialInfoTable();

            this.FormClosing += new FormClosingEventHandler(SGSserverForm_FormClosing);
            this.Shown += new EventHandler(Form1_Shown);

        }
        void SGSserverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Tick -= _timer_Tick;
            _timer.Enabled = false;
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
        void _timer_Tick(object sender, EventArgs e)
        {

            UDPServer.Manualstate.WaitOne();
            UDPServer.Manualstate.Reset();
            string str = UDPServer.sbuilder.ToString();
            UDPServer.sbuilder.Remove(0, str.Length);

            UDPServer.Manualstate.Set();
            helper.ParseDataToTag(str);
            if (str != null && str.Length >= 0)
            {
                //this.txtLog.Text = str + "\r\n" + this.txtLog.Text;
                Debug.WriteLine(
                    string.Format(".  _timer_Tick -> string = {0}"
                    , str));
            }
            //处理列表
            List<TagInfo> tagList = helper.getTagList();

            //首先把不再存在的标签从列表中删除
            DataRow[] allRows = MemoryTable.unitTable.Select();
            for (int j = 0; j < allRows.Length; j++)
            {
                DataRow dr = allRows[j];
                string epc = (string)dr["epc"];
                int floor = (int)dr["floor"];
                //int tagFloor = -1;
                //tagFloor=this.getFloorByAntennaID()
                bool bFinded = false;
                foreach (TagInfo ti in tagList)
                {
                    if ((ti.epc == epc) && (this.getFloorByAntennaID(ti.antennaID) == floor))//这里的不存在应该和位置相关
                    //if (ti.epc == epc)
                    {
                        bFinded = true;
                        break;
                    }
                }
                if (bFinded == false)//如果列表中不再存在这个标签，就把之前标签占据的位置的状态设为empty
                {
                    //dr["epc"] = "";
                    //dr["status"] = "empty";
                    dr["action"] = "delete";
                }
            }
            // 添加新出现的标签到显示列表中
            for (int i = 0; i < tagList.Count; i++)
            {
                TagInfo ti = tagList[i];
                string epc = ti.epc;
                int tagFloor = this.getFloorByAntennaID(ti.antennaID);

                DataRow[] drs = MemoryTable.unitTable.Select("epc='" + epc + "' and floor=" + tagFloor.ToString());
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
                    Debug.WriteLine("antinna To floor ->  floor = " + floor.ToString());

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
                            emptyRows[0]["action"] = "add";
                            if (epc != null && epc.Length >= 23)
                            {
                                string type = epc.Substring(20, 2);

                                emptyRows[0]["type"] = type;
                            }

                        }


                    }
                }
            }

            //其它逻辑处理
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

            //处理显示
            //需要删除的
            DataRow[] rows = MemoryTable.unitTable.Select("action = 'delete'");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {

                    string id = (string)rows[i]["id"];
                    if (id != null && id.Length > 0)
                    {
                        int groupIndex = (int)rows[i]["group"];
                        Carbinet _carbinet = this.groups[groupIndex];
                        DocumentFile df = _carbinet.getDoc(id);
                        if (df != null)
                        {
                            df.setBackgroundImage(null);
                            df.indexBase = "z" + df.columnNumber.ToString("0000");//重置为基础的排序依据
                        }
                        //_carbinet.setDocBGImage(id, null);
                        // _carbinet.setDocBGColor(id, Color.Gray);
                    }

                    // 不同种类的物品减去消失标签的数量
                    //string epc = (string)rows[i]["epc"];
                    //if (epc.Length >= 22)
                    //{
                    //    string type = epc.Substring(20, 2);
                    //    DataRow[] drsType = MemoryTable.typeMapTable.Select("type='" + type + "'");
                    //    if (drsType.Length > 0)
                    //    {

                    //        drsType[0]["count"] = ((int)drsType[0]["count"]) - 1;

                    //    }
                    //}
                    rows[i]["action"] = "normal";
                    rows[i]["epc"] = "";
                    rows[i]["type"] = "";
                    rows[i]["status"] = "empty";

                }
            }
            // 需要增加的
            rows = MemoryTable.unitTable.Select("action = 'add'");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {

                    int groupIndex = (int)rows[i]["group"];
                    string id = (string)rows[i]["id"];
                    string epc = (string)rows[i]["epc"];
                    Carbinet _carbinet = this.groups[groupIndex];
                    // 不同种类的物品设置成不同的大小
                    string type = MemoryTable.getProductTypeID(epc);

                    DataRow[] drsType = MemoryTable.typeMapTable.Select("type='" + type + "'");
                    if (drsType.Length > 0)
                    {
                        int width = (int)drsType[0]["width"];
                        int height = (int)drsType[0]["height"];
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
                            df.indexBase = type;
                        }
                        //_carbinet.setDocBGImage(id, newImage);

                        _carbinet.setDocSize(id, width, height);

                        //drsType[0]["count"] = ((int)drsType[0]["count"]) + 1;

                    }
                    rows[i]["action"] = "normal";
                }
            }

            //设置饼图
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
                colors[i] = Color.FromArgb(alpha, (int)typeRows[i]["red"], (int)typeRows[i]["green"], (int)typeRows[i]["blue"]);
            }
            m_panelDrawing.Values = decimals;
            m_panelDrawing.Texts = strs;
            m_panelDrawing.Colors = colors;

        }

        void Form1_Shown(object sender, EventArgs e)
        {
            UDPServer.startUDPListening();

            this.InitialClassRoom();
            this.InitializePanelControl();

            //饼图添加图例
            int initialTop = 350;
            DataRow[] typeRows = MemoryTable.typeMapTable.Select();
            for (int i = 0; i < typeRows.Length; i++, initialTop = initialTop + 40)
            {
                Button btnLegend = new Button();
                btnLegend.FlatStyle = FlatStyle.Flat;
                btnLegend.FlatAppearance.BorderSize = 0;
                btnLegend.Width = 61;
                btnLegend.Height = 23;
                btnLegend.BackColor = Color.FromArgb(alpha, (int)typeRows[i]["red"], (int)typeRows[i]["green"], (int)typeRows[i]["blue"]);
                btnLegend.Left = 780;
                btnLegend.Top = initialTop;
                this.Controls.Add(btnLegend);
                this.legendButtonList.Add(btnLegend);

                Label lblLegend = new Label();
                lblLegend.Left = 855;
                lblLegend.Top = initialTop + 5;
                lblLegend.Text = (string)typeRows[i]["productName"];
                this.Controls.Add(lblLegend);
                this.legendLabelList.Add(lblLegend);
            }


            //在下拉列表中增加内容
            for (int i = 0; i < typeRows.Length; i++)
            {
                this.cmbProductName.Items.Add((string)typeRows[i]["productName"]);
            }

            _timer.Enabled = true;

        }
        private void initialInfoTable()
        {
            //统一初始化
            if (MemoryTable.isInitialized == false)
            {
                MemoryTable.initializeTabes();
            }


        }
        void updateStatus()
        {

        }
        int alpha = 160;

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
        // 29 137
        // 166 282
        // 311 427
        // 456 562
        // 25 553

        private void InitialClassRoom()
        {

            int numberOfGroup = 1;
            int widthOfRoom = this.pictureBox1.Width;
            int heightofRoom = this.pictureBox1.Height;
            int floorGap = 35;
            int heightOfRow = 140;

            int totalColumns = numberOfGroup;
            totalColumns = 10;
            int numberOfUnit = totalColumns + numberOfGroup - 1;
            int widthOfUnit = widthOfRoom / numberOfUnit;
            int groupInitialLeft = 0;

            for (int i = 0; i < numberOfGroup; i++)
            {
                int numberofColumn = 10;
                int numberOfRow = 3;
                //heightOfRow = (heightofRoom - floorGap * numberOfRow) / numberOfRow;
                int groupWidth = 528;
                //                int groupWidth = numberofColumn * widthOfUnit;

                Carbinet group = new Carbinet(this.pictureBox1.Controls);
                group.Left = groupInitialLeft;
                group.Top = 0;
                this.groups.Add(group);
                //初始化每一排的行
                int initialTop = 32;
                for (int irow = 1; irow <= numberOfRow; irow++, initialTop = initialTop + (int)(heightOfRow + floorGap))
                {
                    CarbinetFloor row = new CarbinetFloor(group, irow, this.pictureBox1.Controls);
                    row.Width = groupWidth;
                    row.Height = heightOfRow;
                    row.relativeTop = initialTop;
                    row.relativeLeft = 25;

                    group.AddFloor(row);

                    for (int k = 1; k <= numberofColumn; k++)
                    {
                        string _equipmentID = i.ToString() + "," + irow.ToString() + "," + k.ToString();

                        DocumentFile df = new DocumentFile(_equipmentID, irow);
                        df.Width = widthOfUnit;
                        df.Height = heightOfRow;
                        df.carbinetIndex = i;
                        df.floorNumber = irow;
                        df.columnNumber = k;
                        df.indexBase = "z" + k.ToString("0000");

                        MemoryTable.unitTable.Rows.Add(new object[] { _equipmentID, "", i, irow, "empty", "normal" });

                        df.Click += new EventHandler(df_Click);
                        group.AddDocFile(df);
                    }

                }
                groupInitialLeft += groupWidth + widthOfUnit;
            }
        }

        void df_Click(object sender, EventArgs e)
        {
            DocumentFile df = (DocumentFile)sender;
            frmShowInfo frm = new frmShowInfo(df.name);
            frm.ShowDialog();
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

    }
}
