using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace IntelligentCarbinet
{
    /// <summary>
    /// 读取到的标签的类，记录了标签的编号、是否在货架上等属性
    /// </summary>
    public class EpcProduct
    {
        int initialVote = 6;
        public string epc;
        public TagInfo tagInfo = null;
        public bool IsOnFloor = false;
        //标签所在货架层的索引
        public string FloorIndex = string.Empty;
        //是否读取到的记录，默认为-1，读取到设为1，未读取到设为0
        public List<int> ReadRecord = new List<int>();
        int iReadRecordLength = 5;//记录的读取到的总范围
        int iReadRecordMinLength = 3;//总范围中读到的最小次数
        public int Vote = 0;
        public EpcProduct(TagInfo ti)
        {
            this.tagInfo = ti;
            for (int i = 0; i < iReadRecordLength; i++)
            {
                this.ReadRecord.Add(-1);
            }
            this.ReadRecord[0] = 1;
        }
        public int VoteDecrease()
        {
            this.Vote--;
            //Debug.WriteLine("VoteDecrease -> vote = " + this.Vote.ToString());
            if (this.IsOnFloor && this.Vote < 0)
            {
                //Debug.WriteLine(string.Format("Off floor epc {0}  antena  {1} ", this.tagInfo.epc, this.tagInfo.antennaID));
                return -1;//当在货架上，且票数小于0时
            }
            return 0;//一般情况
        }
        public int SetRecordReaded()
        {
            //设置读取记录为读取到
            for (int i = iReadRecordLength - 1; i > 0; i--)
            {
                ReadRecord[i] = ReadRecord[i - 1];
            }
            ReadRecord[0] = 1;

            if (this.IsOnFloor)
            {
                int iAddedVote = 0;
                int sum = 0;
                for (int j = 0; j < iReadRecordLength; j++)
                {
                    sum += ReadRecord[j];
                }
                iAddedVote = sum;
                int voteTemp = this.Vote + iAddedVote;
                if (voteTemp < initialVote)
                {
                    this.Vote = voteTemp;
                }
                else
                {
                    this.Vote = initialVote;
                }
                //Debug.WriteLine(string.Format("SetRecordReaded -> vote = {0}  Added = {1}"
                //                    , this.Vote.ToString(), iAddedVote.ToString()));
            }
            else
            {
                //每次读到新数据，检查是否可以认为其确实在货架上
                int sum = 0;
                for (int j = 0; j < iReadRecordLength; j++)
                {
                    sum += ReadRecord[j];
                }
                if (sum >= iReadRecordMinLength)
                {
                    return 1;//符合上架的条件了
                }
            }
            return 0;
        }
        public void ShiftFromFloor()
        {
            this.IsOnFloor = false;
        }
        public void ShiftToBeOnFloor()
        {
            this.IsOnFloor = true;//放置在货架上了

            this.Vote = initialVote;
            //Debug.WriteLine(string.Format("On Floor ->   epc {0}  antena  {1} ", this.tagInfo.epc, this.tagInfo.antennaID));
        }
        public void SetRecordUnreaded()
        {
            //设置读取记录为未读取到
            for (int i = iReadRecordLength - 1; i > 0; i--)
            {
                ReadRecord[i] = ReadRecord[i - 1];
            }
            ReadRecord[0] = 0;

        }
        public EpcProduct()
        {

        }
    }
}
