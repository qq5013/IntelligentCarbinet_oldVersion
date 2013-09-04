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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmMain_Load);

            staticClass.setScreenPara();
        }

        void frmMain_Load(object sender, EventArgs e)
        {
            //SQLConnConfig sqlc = SQLConnConfig.getDefaultDBConfig(DBType.sqlserver);
            //if (sqlc != null)
            //{
            //    staticClass.currentDBConnectString = sqlc.connectString;
            //}
        }

        private void 产品参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProductConfig frm = new frmProductConfig();
            frm.ShowDialog();
        }

        private void 货架产品监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.ShowDialog();
        }

        private void 缺货监控ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRemind frm = new frmRemind();
            frm.ShowDialog();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 frm = new AboutBox1();
            frm.ShowDialog();
        }

        private void 系统参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSysConfig frm = new frmSysConfig();
            frm.ShowDialog();
        }

    }
}
