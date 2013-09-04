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
            if (tagID != null && tagID.Length > 0)
            {
                DataRow[] drs1 = MemoryTable.unitTable.Select("id='" + tagID + "'");
                if (drs1.Length > 0)
                {
                    string epc = (string)drs1[0]["epc"];
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

                    }
                }
            }
            else
            {
                this.lblName.Text = "";
                this.lblID.Text = "";
            }

        }
    }
}
