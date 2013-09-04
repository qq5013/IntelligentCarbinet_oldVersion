using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace CarbinetNM
{
    // 柜子的层
    public class CarbinetFloor
    {
        #region Members
        //相对于柜子的位置
        public int relativeLeft = 0;
        public int relativeTop = 0;

        public int maxDocNumber = -1;
        private int height = 104;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        private int width = 300;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        // 相对于柜子的位置
        private int left = 40;//放置档案的最左边位置
        private int top = 33;//层的顶置
        public int Left
        {
            get { return left; }
            set { left = value; }
        }


        public int Top
        {
            get { return top; }
            set { top = value; }
        }
        //保存添加的档案
        public List<DocumentFile> documentList = new List<DocumentFile>();
        public int floorNumber;
        public static int DOCUMENT_GAP = 3;//档案间的间隔
        System.Windows.Forms.Control.ControlCollection controls;
        // List<DocumentChair> chairsList = new List<DocumentChair>();

        public Carbinet carbinet = null;

        #endregion

        public CarbinetFloor(Carbinet carbinet, int floorNumber,
                                System.Windows.Forms.Control.ControlCollection controls)
        {
            this.floorNumber = floorNumber;
            this.controls = controls;
        }
        /// <summary>
        ///   获取指定索引前档案的宽度总和
        /// </summary>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public int getDocumentsTotalWidth(int endIndex)
        {
            int totalWidth = 0;
            if (endIndex == -1)
            {
                endIndex = this.documentList.Count;
            }
            for (int i = 0; i < endIndex; i++)
            {
                totalWidth += this.documentList[i].Width;
            }
            //if (this.documentList.Count > 0)
            //{
            //    foreach (DocumentFile df in this.documentList)
            //    {
            //        totalWidth += df.Width;
            //    }
            //}
            return totalWidth;
        }

        // 添加物资时，将物资重新排序
        public void AddDoc(DocumentFile Doc)
        {
#if TRACE
            Debug.WriteLine("footprint -> CarbinetFloor.AddDoc");
#endif
            //检查是否档案数量是否已经超出设置的最大数量或者超出了该层的宽度
            int totalDocumentWidth = this.getDocumentsTotalWidth(-1) + (this.documentList.Count - 1) * CarbinetFloor.DOCUMENT_GAP;
            if (totalDocumentWidth + Doc.Width > this.width)
            {
                return;
            }
            if (this.maxDocNumber != -1 && this.documentList.Count >= this.maxDocNumber)
            {
                return;
            }

            //检查是否已经存在了
            bool bExist = false;
            foreach (DocumentFile df in this.documentList)
            {
                if (Doc.name == df.name)
                {
                    bExist = true;
                    break;
                }
            }
            if (bExist)//存在的将不再重复添加
            {
                return;
            }

            //物资添加到每层的列表中
            this.documentList.Add(Doc);
            if (Doc.Height > this.height)
            {
                Doc.Height = this.height - 5;
            }
            Doc.Top = this.top + this.height - Doc.Height;
            //Doc.Top = this.height - Doc.Height;
            //重新排序物资
            this.reIndexDocs();

            this.controls.Add(Doc.doc);
        }

        //整理所有物资的位置
        public void reIndexDocs()
        {
            for (int i = 0; i < this.documentList.Count; i++)
            {
                DocumentFile df = this.documentList[i];
                Debug.WriteLine(
                    string.Format("before sort  -> i = {0} name = {1}"
                    , i.ToString(), df.name));
            }
            this.documentList.Sort();

            for (int i = 0; i < this.documentList.Count; i++)
            {
                DocumentFile df = this.documentList[i];
                if (i > 0)
                {
                    //如果档案所占空间超出了该层的右边界，则不显示
                    int crtLeft = this.documentList[i - 1].Left + this.documentList[i - 1].Width
                                + CarbinetFloor.DOCUMENT_GAP;
                    df.Left = crtLeft;
                    if (crtLeft + df.Width > this.width)
                    {
                        df.doc.Visible = false;
                    }
                    else
                    {
                        df.doc.Visible = true;
                    }
                }
                else
                {
                    df.Left = this.Left;
                }
                df.Top = this.top + this.height - df.Height;

                Debug.WriteLine(
                    string.Format("CarbinetFloor.reIndexDocs  -> i = {0} Left = {1}  Top = {2}"
                    , i.ToString(), df.Left, df.Top));
            }
            for (int i = 0; i < this.documentList.Count; i++)
            {
                DocumentFile df = this.documentList[i];
                Debug.WriteLine(
                    string.Format("after sort  -> i = {0} name = {1} left = {2}"
                    , i.ToString(), df.name, df.Left));
            }
            return;
            //for (int i = 0; i < this.documentList.Count; i++)
            //{
            //    DocumentFile df = this.documentList[i];
            //    df.index = 0;
            //    for (int j = 0; j < this.documentList.Count; j++)
            //    {
            //        if (j == i)
            //        {
            //            continue;
            //        }
            //        if ((df > this.documentList[j]) > 0)
            //        {
            //            df.index++;
            //        }
            //    }
            //}
        }


        public void RemoveDoc(DocumentFile doc, int i)
        {
#if TRACE
            Debug.WriteLine("footprint -> CarbinetFloor.RemoveDoc");
#endif
            this.controls.Remove(doc.doc);
            this.documentList.Remove(doc);
            //this.chairsList.Remove(this.chairsList[i]);
#if TRACE
            Debug.WriteLine("RemoveDoc <-");
#endif
        }

        //public void setDocBGColor(string name, Color color)
        //{
        //    foreach (DocumentFile df in this.documentList)
        //    {
        //        if (df.name==name)
        //        {
        //            df.setBackgroundColor(color);
        //            break;
        //        }
        //    }
        //}
    }
}
