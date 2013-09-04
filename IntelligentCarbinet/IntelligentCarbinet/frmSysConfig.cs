using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PublicConfig;

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
            appConfig ac = appConfig.getDefaultConfig();
            this.txtPort.Text = ac.port.ToString();
            this.txtInterval.Text = ac.interval.ToString();
            int selectedIndex = 0;
            selectedIndex = this.cmbRfidDataFormat.Items.IndexOf(ac.dataFormat.ToString());
            SQLConnConfig sqlc = SQLConnConfig.getDefaultDBConfig(DBType.sqlserver);
            if (sqlc != null)
            {
                this.txtConnString.Text = sqlc.connectString;
            }
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
            if (this.checkConnectValidation() == false)
            {
                return;
            }
            if (SQLConnConfig.testConnection(DBType.sqlite, this.txtConnString.Text))
                //if (SQLConnConfig.testConnection(DBType.sqlserver, this.txtConnString.Text))
            {
                MessageBox.Show("连接测试成功！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接测试失败！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.checkConnectValidation() == false)
            {
                //return;
            }
            else
            {
                SQLConnConfig sqlc = new SQLConnConfig(DBType.sqlserver, this.txtConnString.Text);
                SQLConnConfig.saveConfig(sqlc);
                staticClass.currentDBConnectString = this.txtConnString.Text;
            }
            appConfig ac = new appConfig();
            if (this.checkIntervalValidation() == false)
            {
                //return;
            }
            else
            {
                ac.interval = int.Parse(this.txtInterval.Text);

            }
            if (this.checkPortValidation() == false)
            {
                //return;
            }

            else
            {
                ac.port = int.Parse(this.txtPort.Text);

            }

            appConfig.saveConfig(ac);

            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
