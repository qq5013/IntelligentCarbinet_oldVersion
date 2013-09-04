using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentCarbinet
{
    public partial class frmEpcStatics : Form
    {
        DataTable dt = null;
        public frmEpcStatics()
        {
            InitializeComponent();

            dt = MemoryTable.staticsTable;
            //dt.Rows.Add(new object[] { "300833B2DDD906C001010102", "上架", "2002-12-12" });
            //dt.Rows.Add(new object[] { "300833B2DDD906C001010102", "下架", "2002-12-12" });
            //dt.Rows.Add(new object[] { "300833B2DDD906C001010103", "上架", "2002-12-12" });


            this.Shown += new EventHandler(frmEpcStatics_Shown);
        }

        void frmEpcStatics_Shown(object sender, EventArgs e)
        {
            this.lblCount.Text = string.Empty;
            ClearDataView();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string epc = dt.Rows[i]["产品编号"] as string;
                if (!this.lbEpc.Items.Contains(epc))
                {
                    this.lbEpc.Items.Add(epc);
                }
            }
        }
        void ClearDataView()
        {
            DataTable dtTemp = new DataTable("dtTemp");
            dtTemp.Columns.Add("产品编号", typeof(string));
            dtTemp.Columns.Add("事件", typeof(string));//标记是否是上架还是下架 
            dtTemp.Columns.Add("时间", typeof(string));//标记上架或者下架的时间

            this.dataGridView1.DataSource = dtTemp;
        }
        void formatDataView()
        {
            this.dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            int headerW = this.dataGridView1.RowHeadersWidth;
            int columnsW = 0;
            DataGridViewColumnCollection columns = this.dataGridView1.Columns;
            columns[0].Width = 160;
            columns[1].Width = 80;
            for (int i = 0; i < columns.Count; i++)
            {
                columnsW += columns[i].Width;
            }
            if (columnsW + headerW < this.dataGridView1.Width)
            {
                int leftTotalWidht = this.dataGridView1.Width - columnsW - headerW;
                int eachColumnAddedWidth = leftTotalWidht / (columns.Count - 1);
                for (int i = 1; i < columns.Count; i++)
                {
                    columns[i].Width += eachColumnAddedWidth;
                }
            }
        }
        private void lbEpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbEpc.SelectedItem != null)
            {
                string epc = lbEpc.SelectedItem.ToString();
                DataRow[] rows = dt.Select("产品编号 = '" + epc + "' and 事件 = '下架'");
                this.lblCount.Text = rows.Length.ToString();

                ClearDataView();
                rows = dt.Select("产品编号 = '" + epc + "'");
                if (rows.Length > 0)
                {
                    DataTable dtTemp = (DataTable)this.dataGridView1.DataSource;
                    for (int j = 0; j < rows.Length; j++)
                    {
                        DataRow dr = rows[j];
                        dtTemp.Rows.Add(new object[] { dr["产品编号"], dr["事件"], dr["时间"] });
                    }
                }
                formatDataView();
            }
        }
    }
}
