using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace IntelligentCarbinet
{
    public partial class frmProductConfig : Form
    {
        bool bPicChanged = false;
        public frmProductConfig()
        {
            InitializeComponent();
            this.refreshTypeList();
        }
        void refreshTypeList()
        {
            this.lbProductTypes.Items.Clear();

            List<string> list = MemoryTable.getAllConfigTypes();
            foreach (string s in list)
            {
                this.lbProductTypes.Items.Add(s);
            }
            if (lbProductTypes.Items.Count > 0)
            {
                lbProductTypes.SelectedIndex = 0;
            }
            //DataTable dt = MemoryTable.typeMapTable;
            ////DataTable dt = MemoryTable.getAllProductConfigInfo();
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        DataRow dr = dt.Rows[i];
            //        this.lbProductTypes.Items.Add((string)dr["type"]);
            //    }
            //    lbProductTypes.SelectedIndex = 0;
            //}

        }
        private void btnShowColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cdialog = new ColorDialog())
            {
                cdialog.AnyColor = true;
                if (cdialog.ShowDialog() == DialogResult.OK)
                {
                    this.btnShowColor.BackColor = cdialog.Color;
                }//if
            }//using
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        string picName = string.Empty;
        string picFullPath = string.Empty;
        private void btnChangePic_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "图片文件|*.png;*jpg";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.bPicChanged = true;

                picFullPath = openFileDialog1.FileName;
                this.picName = openFileDialog1.SafeFileName;
                Image newImage = null;
                try
                {
                    newImage = Image.FromFile(picFullPath);
                }
                catch
                {

                }
                this.pictureBox1.Image = newImage;
                //newImage.Dispose();
            }
        }
        int red = 0;
        int green = 0;
        int blue = 0;
        int productWidth = 0;
        int productHeight = 0;
        int minCount = 0;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.checkConfigValid())
            {

                if (!this.copyProductPic())
                {
                    return;
                }

                bool bR = MemoryTable.AddNewConfig(this.txtProductType.Text, this.picName, productWidth, productHeight, minCount, this.txtProductName.Text, red, green, blue);
                if (bR == true)
                {
                    this.btnAdd.Enabled = false;
                    this.refreshTypeList();
                }
            }

        }
        bool checkConfigValid()
        {
            if (this.txtProductName.Text == null || this.txtProductName.Text.Length <= 0)
            {
                MessageBox.Show("必须填写一个产品名称");
                return false;
            }
            if (this.txtWidth.Text == null || this.txtWidth.Text.Length <= 0)
            {
                MessageBox.Show("必须设定产品宽度");
                return false;
            }
            try
            {
                productWidth = int.Parse(this.txtWidth.Text);
                if (productWidth <= 0)
                {
                    MessageBox.Show("产品宽度不能为0或者小于0");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("产品宽度填写不合法");
                return false;
            }
            if (this.txtHeight.Text == null || this.txtHeight.Text.Length <= 0)
            {
                MessageBox.Show("必须设定产品高度");
                return false;
            }
            try
            {
                productHeight = int.Parse(this.txtHeight.Text);
                if (productHeight <= 0)
                {
                    MessageBox.Show("产品高度不能为0或者小于0");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("产品高度填写不合法");
                return false;
            }

            if (this.txtMinCount.Text == null || this.txtMinCount.Text.Length <= 0)
            {
                MessageBox.Show("必须设定产品安全库存");
                return false;
            }
            try
            {
                minCount = int.Parse(this.txtMinCount.Text);
                if (minCount < 0)
                {
                    MessageBox.Show("产品安全库存不能小于0");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("产品安全库存填写不合法");
                return false;
            }

            if (this.picName == string.Empty)
            {
                MessageBox.Show("请设置产品显示图片");
                return false;
            }
            Color color = this.btnShowColor.BackColor;

            red = (int)color.R;
            green = (int)color.G;
            blue = (int)color.B;

            return true;
        }
        private void txtProductType_TextChanged(object sender, EventArgs e)
        {
            string str = this.txtProductType.Text;
            // 如果根本没有指定类型，那么三个按钮都不可用
            if (str == null || str.Length <= 0)
            {
                this.btnAdd.Enabled = false;
                this.btnDelete.Enabled = false;
                this.btnUpdate.Enabled = false;
                return;
            }
            bool bFinded = false;
            for (int i = 0; i < this.lbProductTypes.Items.Count; i++)
            {
                string item = (string)this.lbProductTypes.Items[i];
                if (item.Equals(str))
                {
                    bFinded = true;
                    break;
                }
            }
            // 如果设置的类型已经存在，那么添加按钮不可用，删除和更新按钮可用
            if (bFinded == true)
            {
                this.btnAdd.Enabled = false;
                this.btnDelete.Enabled = true;
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnAdd.Enabled = true;
                this.btnDelete.Enabled = false;
                this.btnUpdate.Enabled = false;
            }
        }
        bool copyProductPic()
        {
            if (this.bPicChanged == false)//如果产品图片没有变化，直接返回
            {
                return true;
            }
            //将设置的图片复制到指定目录
            if (!File.Exists(picFullPath))
            {
                MessageBox.Show("设置的图片不存在，无法保存产品信息");
                return false;

            }
            else
            {
                try
                {
                    //this.pictureBox1.Image.Dispose();
                    File.Copy(picFullPath, PublicConfig.staticClass.PicturePath + picName, false);
                }
                catch(Exception e)
                {
                    MessageBox.Show("复制图片时出现异常，无法保存产品信息");
                    return false;

                }
            }
            return true;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (this.checkConfigValid())
            {

                if (!this.copyProductPic())
                {
                    return;
                }

                bool bR = MemoryTable.updateConfig(this.txtProductType.Text, this.picName, productWidth, productHeight, minCount, this.txtProductName.Text, red, green, blue);
                if (bR == true)
                {
                    this.btnAdd.Enabled = false;
                    MessageBox.Show("更改产品信息成功");

                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.checkConfigValid())
            {
                bool bExist = MemoryTable.ConfigExists(this.txtProductType.Text);
                if (bExist == false)
                {
                    MessageBox.Show("不存在该类型的产品");
                    return;
                }
                bool bR = MemoryTable.deleteConfig(this.txtProductType.Text);
                if (bR == true)
                {
                    this.refreshTypeList();
                }
            }
        }

        private void lbProductTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.lbProductTypes.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }
            string type = (string)this.lbProductTypes.Items[selectedIndex];
            DataRow dr = MemoryTable.getSpecifiedProductConfigInfo(type);
            if (dr!=null)
            {
                this.txtProductType.Text = type;
                //DataRow dr = dt.Rows[0];
                string productName = (string)dr["productName"];
                string width = dr["width"].ToString();
                string height = dr["height"].ToString();
                string mincount = dr["minCount"].ToString();
                this.txtProductName.Text = productName;
                this.txtWidth.Text = width;
                this.txtHeight.Text = height;
                this.txtMinCount.Text = mincount;
                try
                {
                    productWidth = int.Parse(width);
                    productHeight = int.Parse(height);
                    minCount = int.Parse(mincount);

                    red = int.Parse(dr["red"].ToString());
                    green = int.Parse(dr["green"].ToString());
                    blue = int.Parse(dr["blue"].ToString());
                    Color color = Color.FromArgb(red, green, blue);
                    this.btnShowColor.BackColor = color;

                    this.picName = dr["picname"].ToString();
                    string path = PublicConfig.staticClass.PicturePath + picName;
                    Image newImage = null;
                    try
                    {
                        newImage = Image.FromFile(path);
                    }
                    catch
                    {

                    }
                    this.pictureBox1.Image = newImage;
                }
                catch
                {

                }
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
