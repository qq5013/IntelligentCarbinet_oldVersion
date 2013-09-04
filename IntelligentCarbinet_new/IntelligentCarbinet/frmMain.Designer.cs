namespace IntelligentCarbinet
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.产品监控ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.货架产品监控ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.物资监控防盗模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缺货监控ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.产品设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.产品参数设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统参数设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.产品监控ToolStripMenuItem,
            this.产品设置ToolStripMenuItem,
            this.帮助HToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(901, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 产品监控ToolStripMenuItem
            // 
            this.产品监控ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.货架产品监控ToolStripMenuItem,
            this.物资监控防盗模式ToolStripMenuItem,
            this.缺货监控ToolStripMenuItem});
            this.产品监控ToolStripMenuItem.Name = "产品监控ToolStripMenuItem";
            this.产品监控ToolStripMenuItem.Size = new System.Drawing.Size(88, 21);
            this.产品监控ToolStripMenuItem.Text = "物资监控(&M)";
            // 
            // 货架产品监控ToolStripMenuItem
            // 
            this.货架产品监控ToolStripMenuItem.Name = "货架产品监控ToolStripMenuItem";
            this.货架产品监控ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.货架产品监控ToolStripMenuItem.Text = "货架一监控";
            this.货架产品监控ToolStripMenuItem.Click += new System.EventHandler(this.货架产品监控ToolStripMenuItem_Click);
            // 
            // 物资监控防盗模式ToolStripMenuItem
            // 
            this.物资监控防盗模式ToolStripMenuItem.Name = "物资监控防盗模式ToolStripMenuItem";
            this.物资监控防盗模式ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.物资监控防盗模式ToolStripMenuItem.Text = "货架二监控";
            this.物资监控防盗模式ToolStripMenuItem.Click += new System.EventHandler(this.物资监控防盗模式ToolStripMenuItem_Click);
            // 
            // 缺货监控ToolStripMenuItem
            // 
            this.缺货监控ToolStripMenuItem.Name = "缺货监控ToolStripMenuItem";
            this.缺货监控ToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.缺货监控ToolStripMenuItem.Text = "缺货监控";
            this.缺货监控ToolStripMenuItem.Visible = false;
            this.缺货监控ToolStripMenuItem.Click += new System.EventHandler(this.缺货监控ToolStripMenuItem_Click);
            // 
            // 产品设置ToolStripMenuItem
            // 
            this.产品设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.产品参数设置ToolStripMenuItem,
            this.系统参数设置ToolStripMenuItem});
            this.产品设置ToolStripMenuItem.Name = "产品设置ToolStripMenuItem";
            this.产品设置ToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.产品设置ToolStripMenuItem.Text = "设置(&T)";
            // 
            // 产品参数设置ToolStripMenuItem
            // 
            this.产品参数设置ToolStripMenuItem.Name = "产品参数设置ToolStripMenuItem";
            this.产品参数设置ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.产品参数设置ToolStripMenuItem.Text = "物资参数设置";
            this.产品参数设置ToolStripMenuItem.Click += new System.EventHandler(this.产品参数设置ToolStripMenuItem_Click);
            // 
            // 系统参数设置ToolStripMenuItem
            // 
            this.系统参数设置ToolStripMenuItem.Name = "系统参数设置ToolStripMenuItem";
            this.系统参数设置ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.系统参数设置ToolStripMenuItem.Text = "系统参数设置";
            this.系统参数设置ToolStripMenuItem.Click += new System.EventHandler(this.系统参数设置ToolStripMenuItem_Click);
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem});
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.帮助HToolStripMenuItem.Text = "帮助(&H)";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.关于ToolStripMenuItem.Text = "关于(&A)";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(901, 578);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(280, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(901, 603);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "智能物资管理系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 产品监控ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 产品设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 产品参数设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 货架产品监控ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 缺货监控ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统参数设置ToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem 物资监控防盗模式ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}