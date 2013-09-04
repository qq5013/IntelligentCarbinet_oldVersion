using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace IntelligentCarbinet
{
    public interface ITagListener
    {
        void NewTagMessage();
    }
    public class EpcProductHelper
    {
        #region 成员
        public ITagListener listener = null;
        List<EpcProduct> ProductList = new List<EpcProduct>();
        System.Windows.Forms.Timer _timer = null;
        TDJ_RFIDHelper helper = new TDJ_RFIDHelper();
        UDPServer udp_server = new UDPServer();
        int udp_port = 5000;
        ManualResetEvent resetState = new ManualResetEvent(true);

        //标识是否产品是否发生了变化。一般有以下几种变化：
        // 1 标签消失，即产品下架
        // 2 新标签出现，及产品上架
        // 3 标签层发生变化，及产品位置发生了变化
        bool bHasNewMessage = false;
        #endregion
        public EpcProductHelper()
        {
        }
        public EpcProductHelper(int udpPort)
        {
            this.udp_port = udpPort;
        }
        public void start()
        {
            this.start(udp_port);
        }
        public void start(ITagListener Listener)
        {
            this.listener = Listener;
            this.start(udp_port);
        }
        public void start(int udpPort)
        {
            if (_timer == null)
            {
                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 2000;
                _timer.Tick += new EventHandler(_timer_Tick);
            }
            this.udp_port = udpPort;
            this._timer.Enabled = true;
            this.udp_server.startUDPListening(this.udp_port);
        }
        public void stop()
        {
            if (this._timer != null)
            {
                this._timer.Enabled = false;
            }
            this.udp_server.stop_listening();
            this.ProductList.Clear();
        }
        public List<EpcProduct> GetCurrentProductList()
        {
            resetState.WaitOne();
            resetState.Reset();
            List<EpcProduct> listR = this.ProductList.FindAll(ProductOnFloor);
            resetState.Set();
            return listR;
        }
        #region 内部函数
        private void _timer_Tick(object sender, EventArgs e)
        {
            udp_server.Manualstate.WaitOne();
            udp_server.Manualstate.Reset();
            string temp1 = udp_server.sbuilder.ToString();

            udp_server.sbuilder.Remove(0, temp1.Length);

            udp_server.Manualstate.Set();

            //根据最新数据解析到的读取到的标签
            List<TagInfo> tagList = helper.ParseDataToTag(temp1);
            IntervalCheckProducts(tagList);
        }
        private void IntervalCheckProducts(List<TagInfo> tiList)
        {
            foreach (TagInfo ti in tiList)
            {
                strEpc = ti.epc;
                strAntenaID = ti.antennaID;
                //如果是误读的标签，则不加入到检测中
                if (ti.antennaID == "01" || ti.antennaID == "02" || ti.antennaID == "04" || ti.antennaID == "08")
                {
                    //如果该epc尚未添加，则只需添加到列表中即可
                    if (this.ProductList.FindIndex(FindEpcProduct) < 0)
                    {
                        EpcProduct new_ep = new EpcProduct(ti);
                        this.ProductList.Add(new_ep);
                    }
                }
            }

            foreach (EpcProduct ep in this.ProductList)
            {
                strEpc = ep.tagInfo.epc;
                strAntenaID = ep.tagInfo.antennaID;

                if (tiList.FindIndex(IsThisEpcAndAntena) >= 0)
                {
                    //如果该epc已经存在，更改其读取记录为读取到
                    int OnFloorstate = ep.SetRecordReaded();
                    // 当本产品本身满足上架条件并且整个货架上没有相同Epc的产品在架时，才能上架
                    // 这是为了防止一个标签同时被两个天线读到导致同一产品在不同层上架的情况发生
                    if (OnFloorstate == 1 && this.ProductList.FindIndex(FindEpcProductOnFloorWithoutAntena) < 0)
                    {
                        ep.ShiftToBeOnFloor();
                        this.bHasNewMessage = true;
                    }
                }
                else
                {
                    ep.SetRecordUnreaded();
                }

                //关于误读得到的数据的处理
                //如果有标签在货架上，则认为这是该层读到的，否则放弃
                if (tiList.FindIndex(IsJustThisEpcAndWrongAntena) >= 0 && ep.IsOnFloor)
                {
                    ep.SetRecordReaded();
                }

                int state = ep.VoteDecrease();//每次固定减一
                if (state < 0)//表明产品下架了
                {
                    ep.ShiftFromFloor();//产品下架后才有可能在其它层上架
                    this.bHasNewMessage = true;
                }
            }
            //Debug.WriteLine(string.Format("new message -> port {0}    message  {1}", this.udp_port.ToString(), bHasNewMessage.ToString()));
            if (this.listener != null && this.bHasNewMessage)
            {
                listener.NewTagMessage();
                this.bHasNewMessage = false;
            }
        }
        #region 辅助函数
        static string strAntenaID = string.Empty;
        static string strEpc = string.Empty;//作为一个中转而已

        private static bool IsThisEpcAndAntena(TagInfo ti)
        {
            return ti.epc == strEpc && ti.antennaID == strAntenaID;
        }
        private static bool IsJustThisEpcAndWrongAntena(TagInfo ti)
        {
            return ti.epc == strEpc && ti.antennaID != "01" && ti.antennaID != "02" && ti.antennaID != "04" && ti.antennaID != "08";
        }
        private static bool FindEpcProduct(EpcProduct ep)
        {
            return ep.tagInfo.epc == strEpc && ep.tagInfo.antennaID == strAntenaID;
        }
        private static bool FindEpcProductOnFloorWithoutAntena(EpcProduct ep)
        {
            return ep.tagInfo.epc == strEpc && ep.IsOnFloor;
        }
        static bool ProductOnFloor(EpcProduct ep)
        {
            return ep.IsOnFloor;
        }
        #endregion
        #endregion
    }
}
