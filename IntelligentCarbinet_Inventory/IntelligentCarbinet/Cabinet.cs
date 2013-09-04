using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Drawing;


namespace CarbinetNM
{


    //柜子类
    public class Carbinet
    {
        #region member

        int floorCount = 4;
        int left = 100;
        public int Left
        {
            get { return left; }
            set { left = value; }
        }
        int top = 30;

        public int Top
        {
            get { return top; }
            set { top = value; }
        }
        int width = 100;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        int height = 300;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        int border = 0;

        public int Border
        {
            get { return border; }
            set { border = value; }
        }
        int leftPading = 0;

        public int LeftPading
        {
            get { return leftPading; }
            set { leftPading = value; }
        }
        int topPading = 0;

        public int TopPading
        {
            get { return topPading; }
            set { topPading = value; }
        }

        //保存form的控件集合
        System.Windows.Forms.Control.ControlCollection controlArray = null;
        //柜子的层
        List<CarbinetFloor> floors = new List<CarbinetFloor>();
        #endregion
        public Carbinet(System.Windows.Forms.Control.ControlCollection controlA)
        {
            if (null == controlA)
            {
                throw new Exception("传入参数为空！");
            }
            this.controlArray = controlA;
        }
        public void AddFloor(CarbinetFloor cf)
        {
            cf.Left = this.left + cf.relativeLeft;
            cf.Top = this.top + cf.relativeTop;
            this.floors.Add(cf);
        }
        /// <summary>
        /// 配置档案柜的层，每层的高度和放置档案的最大数量
        /// </summary>
        /// <param name="floorIndex"></param>
        /// <param name="height"></param>
        /// <param name="maxDoc"></param>
        public void ConfigFloor(int floorIndex, int height, int maxDoc)
        {
            CarbinetFloor floor = null;
            if (floorIndex <= 0)
            {
                Debug.WriteLine("floor index error,start from 1");
                return;
            }
            if (floorIndex == 1)
            {
                if (this.floors.Count >= 1)
                {
                    floor = this.floors[0];
                }
                else
                {
                    floor = new CarbinetFloor(this, 1, this.controlArray);
                    this.floors.Add(floor);
                }
            }
            else
            {
                if (floorIndex - 1 > this.floors.Count)// 越层初始化，即初始化2层的时候1层尚未初始化
                {
                    Debug.WriteLine(string.Format("initial floor error ->  floor index = {0}", floorIndex));
                    return;
                }
                if (floorIndex <= this.floors.Count)
                {
                    floor = this.floors[floorIndex - 1];
                }
                else
                {
                    floor = new CarbinetFloor(this, floorIndex, this.controlArray);
                    this.floors.Add(floor);
                }
            }
            if (floor != null)
            {
                floor.Height = height;
                floor.maxDocNumber = maxDoc;
            }
            // 每层的空间配置
            if (floor.floorNumber == 1)
            {
                floor.Top = 24;
                floor.Left = this.leftPading;
            }

            if (floor.floorNumber == 2)
            {
                floor.Top = 137;
                floor.Left = this.leftPading;
            }
            if (floor.floorNumber == 3)
            {
                floor.Top = 220;
                floor.Left = this.leftPading;
            }
        }

        public void ConfigFloor(int floorCount)
        {
            this.floorCount = floorCount;

            CarbinetFloor cf = new CarbinetFloor(this, 1, this.controlArray);
            //int top = cf.floor_height * this.floors.Count;
            cf.Top = 0;
            cf.Height = 130;
            cf.Left = this.left;
            cf.maxDocNumber = 9;
            this.floors.Add(cf);
            cf = new CarbinetFloor(this, 2, this.controlArray);
            cf.Top = 130;
            cf.Height = 110;
            cf.Left = this.left;
            cf.maxDocNumber = 9;
            this.floors.Add(cf);
            //for (int i = 1; i <= floorCount; i++)
            //{
            //    CarbinetFloor cf = new CarbinetFloor(i, this.controlArray);
            //    int top = cf.floor_height * this.floors.Count;
            //    cf.top = top;
            //    this.floors.Add(cf);
            //}
        }
        // 向柜子的指定层添加档案文件
        public void AddDocFile(string name, int floor)
        {
            DocumentFile df = new DocumentFile(name, floor);
            this.AddDocFile(df);
        }
        //todo 废弃
        public void AddDocFile(string name, int floor, int width, int height)
        {
            DocumentFile df = new DocumentFile(name, floor);
            df.Width = width;
            df.Height = height;
            this.AddDocFile(df);
        }
        #region 操作具体物资
        public void AddDocFile(DocumentFile Doc)
        {
            foreach (CarbinetFloor cf in this.floors)
            {
                if (cf.floorNumber == Doc.floorNumber)
                {
                    Doc.floor = cf;
                    cf.AddDoc(Doc);
                    break;
                }
            }
        }
        public void RemoveDocFile(string docName)
        {
            Debug.WriteLine("RemoveDocFile->");
            bool bFinded = false;
            foreach (CarbinetFloor cf in this.floors)
            {
                for (int i = 0; i < cf.documentList.Count; i++)
                {
                    if (cf.documentList[i].name == docName)
                    {
                        cf.RemoveDoc(cf.documentList[i], i);
                        bFinded = true;
                        break;
                    }
                    if (bFinded)
                    {
                        break;
                    }
                }
            }

            Debug.WriteLine("RemoveDocFile <-");
        }
        public DocumentFile getDoc(string docName)
        {
            DocumentFile dfr = null;
            bool bFind = false;

            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        //df.setBackgroundColor(color);
                        dfr = df;
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
            return dfr;
        }
        public void setDocProperty(string docName, string text, Color bgcolor, Image image)
        {
            bool bFind = false;
            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        if (text != string.Empty)
                        {
                            df.setText(text);
                        }
                        if (bgcolor != Color.Empty)
                        {
                            df.setBackgroundColor(bgcolor);
                        }
                        if (image != null)
                        {
                            df.setBackgroundImage(image);
                        }
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
        }
        public void setDocText(string docName, string text)
        {
            bool bFind = false;
            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        //df.setBackgroundColor(color);
                        df.setText(text);
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
        }
        public void setDocBGColor(string docName, Color color)
        {
            bool bFind = false;
            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        df.setBackgroundColor(color);
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
        }
        public void setDocBGImage(string docName, Image image)
        {
            bool bFind = false;
            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        df.setBackgroundImage(image);
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
        }
        public void setDocSize(string docName, int width, int height)
        {
            foreach (CarbinetFloor cf in this.floors)
            {

                foreach (DocumentFile df in cf.documentList)
                {
                    if (df.name == docName)
                    {
                        if (width != -1)
                        {
                            df.Width = width;
                        }
                        if (height != -1)
                        {
                            df.Height = height;
                        }
                        cf.reIndexDocs();
                        break;
                    }
                }

            }
        }

        #endregion
    }


}
