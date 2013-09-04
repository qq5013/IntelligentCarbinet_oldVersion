using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CarbinetNM;
using System.IO;

namespace IntelligentCarbinet
{
    public partial class frmShowInfo : Form
    {
        public frmShowInfo(string tagID)
        {
            InitializeComponent();
            this.Click += new EventHandler(frmShowInfo_Click);
            if (tagID != null && tagID.Length > 0)
            {
                DataRow[] drs1 = MemoryTable.unitTable.Select("id='" + tagID + "'");
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
                        string productName = (string)drs2[0]["productName"];
                        string path = PublicConfig.staticClass.PicturePath + (string)drs2[0]["picname"];
                        this.lblName.Text = productName;
                        this.lblID.Text = epc;
                        Image newImage = null;
                        try
                        {
                            if (File.Exists(path))
                            {
                                newImage = Image.FromFile(path);
                            }
                        }
                        catch (System.Exception ex)
                        {

                        }
                        this.picInfo.Image = newImage;
                        this.lblClass.Text = "北京溢润伟业软件科技有限公司";

                    }
                }
            }
            else
            {
                this.lblName.Text = "";
                this.lblID.Text = "";
            }

        }

        void frmShowInfo_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
