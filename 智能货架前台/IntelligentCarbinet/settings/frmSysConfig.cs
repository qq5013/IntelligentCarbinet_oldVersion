using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PublicConfig;
using System.Net;

namespace IntelligentCarbinet
{
    public partial class frmSysConfig : Form
    {
        public frmSysConfig()
        {
            InitializeComponent();

            this.cmbRfidDataFormat.Items.Clear();
            this.cmbRfidDataFormat.Items.Add("标准");
            this.cmbRfidDataFormat.Items.Add("简化");

            this.Shown += new EventHandler(frmSysConfig_Shown);
            this.Load += new EventHandler(frmSysConfig_Load);
        }

        void frmSysConfig_Load(object sender, EventArgs e)
        {
            //appConfig ac = appConfig.getDefaultConfig();
            //object oPort = null;
            //oPort = nsConfigDB.ConfigDB.getConfig("portListening");
            //if (oPort != null)
            //{
            //    this.txtPort.Text = (string)oPort;
            //}
            //else
            //{
            //    this.txtPort.Text = "5000";
            //}
            //this.txtPort.Text = ac.port.ToString();
            //object oInterval = nsConfigDB.ConfigDB.getConfig("checkStorageInterval");
            //if (oInterval != null)
            //{
            //    this.txtInterval.Text = (string)oInterval;
            //}
            //else
            //{
            //    this.txtInterval.Text = "10";
            //}
            this.txtPort.Text = staticClass.udpPort.ToString();
            this.txtInterval.Text = staticClass.storageCheckInterval.ToString();
            this.txtTagBuffer.Text = staticClass.tagsBufferTime.ToString();
            this.txtrestPort.Text = staticClass.restPort;
            this.txtIP.Text = staticClass.restIP;
            //this.txtInterval.Text = ac.interval.ToString();
            //int selectedIndex = 0;
            //selectedIndex = this.cmbRfidDataFormat.Items.IndexOf(ac.dataFormat.ToString());
            //SQLConnConfig sqlc = SQLConnConfig.getDefaultDBConfig(DBType.sqlserver);
            //if (sqlc != null)
            //{
            //    this.txtConnString.Text = sqlc.connectString;
            //}
        }

        void frmSysConfig_Shown(object sender, EventArgs e)
        {

        }
        bool checkIntervalValidation()
        {
            bool bR = true;
            string strInterval = this.txtInterval.Text;
            try
            {
                int interval = int.Parse(strInterval);
                if (interval <= 0)
                {
                    MessageBox.Show("安全库存检查间隔设置有误，必须为数字且大于0！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

            }
            catch
            {
                MessageBox.Show("安全库存检查间隔设置有误，必须为数字且大于0！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return bR;
        }
        bool checkValidation()
        {
            bool bR = true;
            string strBuffer = this.txtTagBuffer.Text;
            try
            {
                int iBuffer = int.Parse(strBuffer);
                if (iBuffer < 1000)
                {
                    MessageBox.Show("缓冲时间设置太短，请重新设置！");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("缓冲时间设置不符合规定，请重新设置！");
                return false;
            }
            string strPort = this.txtPort.Text;
            try
            {
                int port = int.Parse(strPort);
                if (port <= 0)
                {
                    MessageBox.Show("监听端口设置有误！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

            }
            catch
            {
                MessageBox.Show("监听端口设置有误！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            string strInterval = this.txtInterval.Text;
            try
            {
                int interval = int.Parse(strInterval);
                if (interval <= 0)
                {
                    MessageBox.Show("安全库存检查间隔设置有误，必须为数字且大于0！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

            }
            catch
            {
                MessageBox.Show("安全库存检查间隔设置有误，必须为数字且大于0！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (this.txtIP.Text == null || this.txtIP.Text == string.Empty)
            {
                MessageBox.Show("必须填写读写器IP地址!", "异常提示");
                return false;
            }
            else
            {
                try
                {
                    string str = this.txtIP.Text;
                    IPAddress ip = IPAddress.Parse(str);
                    //MessageBox.Show("IP地址填写不符合规定!", "异常提示");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("IP地址填写不符合规定，" + ex.Message, "异常提示");
                    return false;
                }

            }
            if (this.txtrestPort.Text == null || this.txtrestPort.Text == string.Empty)
            {
                MessageBox.Show("必须填写读写器IP地址!", "异常提示");
                return false;
            }
            else
            {
                try
                {
                    string str = this.txtrestPort.Text;
                    int port = int.Parse(str);
                    //MessageBox.Show("端口填写不符合规定!", "异常提示");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("端口填写不符合规定，" + ex.Message, "异常提示");
                    return false;
                }

            }
            return bR;
        }
        bool checkTagBufferTime()
        {
            string strBuffer = this.txtTagBuffer.Text;
            try
            {
                int iBuffer = int.Parse(strBuffer);
                if (iBuffer < 1000)
                {
                    MessageBox.Show("缓冲时间设置太短，请重新设置！");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("缓冲时间设置不符合规定，请重新设置！");
                return false;
            }
            return true;
        }
        bool checkPortValidation()
        {
            bool bR = true;
            string strPort = this.txtPort.Text;
            try
            {
                int port = int.Parse(strPort);
                if (port <= 0)
                {
                    MessageBox.Show("监听端口设置有误！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

            }
            catch
            {
                MessageBox.Show("监听端口设置有误！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return bR;
        }
        bool checkConnectValidation()
        {
            bool bR = true;
            string connectString = this.txtConnString.Text;
            if (connectString == null || connectString.Length <= 0)
            {
                MessageBox.Show("未填写有效的数据库连接字符串", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                bR = false;
            }
            return bR;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //if (this.checkConnectValidation() == false)
            //{
            //    return;
            //}
            //if (SQLConnConfig.testConnection(DBType.sqlite, this.txtConnString.Text))
            ////if (SQLConnConfig.testConnection(DBType.sqlserver, this.txtConnString.Text))
            //{
            //    MessageBox.Show("连接测试成功！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("连接测试失败！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.checkValidation() == true)
            {
                nsConfigDB.ConfigDB.saveConfig("checkStorageInterval", this.txtInterval.Text);
                nsConfigDB.ConfigDB.saveConfig("portListening", this.txtPort.Text);
                nsConfigDB.ConfigDB.saveConfig("tagsBufferTime", this.txtTagBuffer.Text);
                nsConfigDB.ConfigDB.saveConfig("restIP", this.txtIP.Text);
                nsConfigDB.ConfigDB.saveConfig("restPort", this.txtrestPort.Text);

                staticClass.udpPort = int.Parse(this.txtPort.Text);
                staticClass.storageCheckInterval = int.Parse(this.txtInterval.Text);
                staticClass.tagsBufferTime = int.Parse(this.txtTagBuffer.Text);
                staticClass.restPort = this.txtrestPort.Text;
                staticClass.restIP = this.txtIP.Text;
            }
            return;
            //if (this.checkConnectValidation() == false)
            //{
            //    //return;
            //}
            //else
            //{
            //    SQLConnConfig sqlc = new SQLConnConfig(DBType.sqlserver, this.txtConnString.Text);
            //    SQLConnConfig.saveConfig(sqlc);
            //    staticClass.currentDBConnectString = this.txtConnString.Text;
            //}
            //appConfig ac = new appConfig();
            //if (this.checkIntervalValidation() == false)
            //{
            //    //return;
            //}
            //else
            //{
            //    //ac.interval = int.Parse(this.txtInterval.Text);
            //    nsConfigDB.ConfigDB.saveConfig("checkStorageInterval", this.txtInterval.Text);
            //}
            //if (this.checkPortValidation() == false)
            //{
            //    //return;
            //}
            //else
            //{
            //    //ac.port = int.Parse(this.txtPort.Text);
            //    nsConfigDB.ConfigDB.saveConfig("portListening", this.txtPort.Text);
            //}
            ////appConfig.saveConfig(ac);
            //if (this.checkTagBufferTime() == false)
            //{
            //    MessageBox.Show("智能货架标签缓冲时间未能正常保存！");
            //}
            //else
            //{
            //    nsConfigDB.ConfigDB.saveConfig("tagsBufferTime", this.txtTagBuffer.Text);
            //}

            Program.resetConfig();

            MessageBox.Show("新设置需要重启系统生效！", "提示");

            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
