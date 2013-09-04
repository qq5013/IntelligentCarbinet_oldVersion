using System.Windows.Forms;
using System;
using System.Drawing;

namespace CarbinetNM
{
    public class DocumentFile : IComparable<DocumentFile>
    {
        #region
        public event EventHandler Click;
        public string name;
        string text;
        public CarbinetFloor floor = null;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                this.doc.Text = value;
            }
        }
        public int columnNumber;
        public int floorNumber;//物资在所在层的位置索引
        public Button doc;
        public string indexBase;//比较大小的根据，通常是在同一行中的排序，比如列号
        public int carbinetIndex;//柜子编号
        private int width = 16;

        public int Width
        {
            get { return this.doc.Width; }
            set
            {
                this.doc.Width = value;
                //if (this.floor != null)
                //{
                //    this.floor.reIndexDocs();
                //}
            }
        }
        private int height = 92;

        public int Height
        {
            get { return this.doc.Height; }
            set { this.doc.Height = value; }
        }

        public int Left
        {
            get { return this.doc.Left; }
            set { this.doc.Left = value; }
        }

        public int Top
        {
            get { return this.doc.Top; }
            set { this.doc.Top = value; }
        }
        //public DocumentChair myChair;


        #endregion

        public DocumentFile(string name, int floor)
        {
            this.doc = new Button();
            this.doc.Width = this.width;
            this.doc.Height = this.height;
            this.doc.BackColor = Color.White;
            //this.doc.BackgroundImage = global::Carbinet.Properties.Resources.grey;
            this.doc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            //this.doc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            //this.doc.BackColor = System.Drawing.Color.Silver;
            //this.doc.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.doc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.doc.FlatAppearance.BorderSize = 0;
            this.doc.FlatAppearance.BorderColor = Color.Red;

            this.doc.Name = name;
            this.doc.UseVisualStyleBackColor = false;
            this.doc.TabStop = false;
            this.doc.Text = this.text;
            this.name = name;
            this.floorNumber = floor;

            this.doc.Click += new EventHandler(doc_Click);
        }

        void doc_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
        public void setDocPosition(int left, int top)
        {
            this.doc.Left = left;
            this.doc.Top = top;
        }
        public void setBackgroundColor(Color color)
        {
            this.doc.BackColor = color;
        }
        public void setBackgroundImage(Image image)
        {
            this.doc.BackgroundImage = image;
        }
        public void setBorderWidth(int width)
        {
            this.doc.FlatAppearance.BorderSize = width;
        }
        public void setText(string text)
        {
            this.doc.Text = text;
        }
        public int CompareTo(DocumentFile other)
        {
            // The temperature comparison depends on the comparison of the
            // the underlying Double values. Because the CompareTo method is
            // strongly typed, it is not necessary to test for the correct
            // object type.
            //return (string.CompareOrdinal(other.indexBase, this.indexBase));
            return (string.CompareOrdinal(this.indexBase, other.indexBase));
        }

        public static int operator >(DocumentFile df1, DocumentFile df2)
        {
            return string.CompareOrdinal(df1.indexBase, df2.indexBase);
            //return 0;
        }
        public static int operator <(DocumentFile df1, DocumentFile df2)
        {
            return string.CompareOrdinal(df1.indexBase, df2.indexBase);
            //return 0;
        }

        //public void setChair(DocumentChair dc)
        //{
        //    //this.myChair = dc;
        //}
    }
}
