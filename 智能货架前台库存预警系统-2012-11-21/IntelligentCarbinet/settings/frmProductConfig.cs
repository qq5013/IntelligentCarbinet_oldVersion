using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using httpHelper;
using PublicConfig;
using System.Diagnostics;
using ProductCategoryManage;
using ResSync;

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

                bool bR = MemoryTable.AddNewConfig(
                    this.txtProductType.Text,
                    this.picName,
                    productWidth,
                    productHeight,
                    minCount,
                    this.txtProductName.Text,
                    red, green, blue,
                    this.txtDescription.Text
                    );
                if (bR == true)
                {
                    this.btnAdd.Enabled = false;
                    this.refreshTypeList();
                }
            }

        }
        int default_image_width = 80;
        int default_image_height = 110;
        Color default_product_color = Color.Blue;
        int default_min_count = 0;
        bool checkConfigValid()
        {
            if (this.txtProductName.Text == null || this.txtProductName.Text.Length <= 0)
            {
                MessageBox.Show("必须填写一个产品名称");
                return false;
            }
            int width = (int)this.numWidth.Value;
            if (width <= 0)
            {
                MessageBox.Show("必须设定产品宽度");
                return false;
            }
            else
            {
                productWidth = width;
            }
            int height = (int)this.numHeight.Value;
            if (height <= 0)
            {
                MessageBox.Show("必须设定产品高度");
                return false;
            }
            else
            {
                productHeight = height;
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
                catch (Exception e)
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

                bool bR = MemoryTable.updateConfig(
                    this.txtProductType.Text,
                    this.picName,
                    productWidth,
                    productHeight,
                    minCount,
                    this.txtProductName.Text,
                    red, green, blue,
                    this.txtDescription.Text
                    );
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
            if (dr != null)
            {
                this.txtProductType.Text = type;
                //DataRow dr = dt.Rows[0];
                string productName = (string)dr["productName"];
                string width = dr["width"].ToString();
                string height = dr["height"].ToString();
                string mincount = dr["minCount"].ToString();
                this.txtProductName.Text = productName;
                this.numHeight.Value = int.Parse(height);
                this.numWidth.Value = int.Parse(width);
                this.txtMinCount.Text = mincount;
                this.txtDescription.Text = (string)dr["description"];
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
                        Stream s = File.Open(path, FileMode.Open);
                        pictureBox1.Image = Image.FromStream(s);
                        s.Close();
                        //newImage = Image.FromFile(path);
                        //newImage = (Image)newImage.Clone();
                    }
                    catch
                    {

                    }
                    //this.pictureBox1.Image = newImage;
                    this.pictureBox1.Width = (int)this.numWidth.Value;
                    this.pictureBox1.Height = (int)this.numHeight.Value;
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

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Width = (int)this.numWidth.Value;
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Height = (int)this.numHeight.Value;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            HttpWebConnect helper = new HttpWebConnect();
            string url = staticClass.get_product_category_sync_list_url();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_get_category_list);
            helper.TryPostData(url, string.Empty);
            //        bool bR = MemoryTable.AddNewConfig(
            //this.txtProductType.Text,
            //this.picName,
            //productWidth,
            //productHeight,
            //minCount,
            //this.txtProductName.Text,
            //red, green, blue,
            //this.txtDescription.Text
            //);
        }
        void helper_RequestCompleted_get_category_list(object o)
        {
            string strres = (string)o;
            Debug.WriteLine(
                string.Format("helper_RequestCompleted_get_category_list  ->  = {0}"
                , strres));
            object olist = fastJSON.JSON.Instance.ToObjectList(strres, typeof(List<ProductCategory>), typeof(ProductCategory));
            List<ProductCategory> resList = (List<ProductCategory>)olist;
            if (resList.Count > 0)
            {
                for (int i = 0; i < resList.Count; i++)
                {
                    ProductCategory pc = resList[i];
                    MemoryTable.AddNewConfig(
                                     pc.category_id,
                                     pc.category_image,
                                     default_image_width,
                                     default_image_height,
                                     default_min_count,
                                     pc.category_name,
                                    (int)default_product_color.R, (int)default_product_color.G, default_product_color.B,
                                     "尚未设置"
                     );
                }
            }            
            
            //deleControlInvoke dele = delegate(object ol)
            //{

            //};
            //this.Invoke(dele, olist);
            ResSyncer rs = new ResSyncer();
            rs.start_sync("ProductCategory");
        }
    }
}
