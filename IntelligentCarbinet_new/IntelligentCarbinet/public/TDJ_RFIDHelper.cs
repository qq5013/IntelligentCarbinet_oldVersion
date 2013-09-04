using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using PublicConfig;


namespace InventoryMSystem
{
    public enum emuTagInfoFormat
    {
        标准, 简化, 无
    }
    /// <summary>
    /// 将标签信息解析后通过此类中转处理
    /// </summary>
    public class TagInfo
    {
        public int readCount = 0;
        public string antennaID = string.Empty;
        public string tagType = string.Empty;
        public string epc = string.Empty;
        public string getTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //public int milliSecond = DateTime.Now.Millisecond;

        public long milliSecond = 0;

        public string toString()
        {
            string str = string.Empty;
            str = string.Format("ant -> {0} | count -> {1} | epc -> {2}",
                                this.antennaID, this.readCount, this.epc);

            return str;
        }
        public static TagInfo Parse(string tagInfo)
        {
            Debug.WriteLine("TagInfo -> Parse  " + tagInfo);
            TagInfo ti = new TagInfo();
            try
            {

                emuTagInfoFormat format = emuTagInfoFormat.标准;
                if (tagInfo.Substring(0, 4) == "Disc")
                {
                    format = emuTagInfoFormat.标准;
                }
                else
                {
                    format = emuTagInfoFormat.简化;
                }
                /*
    Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C000000000 
             
                 */
                if (format == emuTagInfoFormat.标准)
                {
                    string[] arrays = tagInfo.Split(',');
                    if (arrays.Length < 6)//信息不全
                    {
                        return null;
                    }

                    string temp = arrays[1];
                    ti.getTime = temp.Substring(temp.IndexOf(':') + 1).Trim();

                    temp = arrays[2];
                    string strCount = arrays[2].Substring(temp.IndexOf(':') + 1);
                    try
                    {
                        ti.readCount = int.Parse(strCount);

                    }
                    catch (System.Exception ex)
                    {

                    }
                    temp = arrays[3];
                    ti.antennaID = temp.Substring(temp.IndexOf(':') + 1).Trim();

                    //这里不再做天线误读的判断，因为误读至少说明这个标签被读到了，这个信息还有用
                    //if (ti.antennaID != "01" && ti.antennaID != "02" && ti.antennaID != "04" && ti.antennaID != "08")
                    //{
                    //    return null;
                    //}

                    temp = arrays[4];
                    ti.tagType = temp.Substring(temp.IndexOf(':') + 1).Trim();
                    temp = arrays[5];
                    ti.epc = temp.Substring(temp.IndexOf(':') + 1).Trim();
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = dt - PublicConfig.staticClass.timeBase;
                    //ti.getTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    //ti.milliSecond = dt.Millisecond;
                    ti.milliSecond = (long)ts.TotalMilliseconds;

                    //  Debug.WriteLine(ti.toString());
                }


#if TRACE
                Debug.WriteLine("TagInfo <- Parse  ");
#endif

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("TagInfo <- Parse error   " + ex.Message);

            }
            return ti;

        }

    }
    //板状读写器操作类
    //实时获取标签信息
    public class TDJ_RFIDHelper
    {
        //两个天线可能有误读，在此时间范围内更正
        int milliSecondDelay = 500;//缓冲时间，毫秒，

        //标签取走的缓冲时间，如果间隔该段时间没有该标签信息更新，则认为标签已经丢失
        int lostDelay = staticClass.tagsBufferTime;//单位毫秒

        int minReadCount = 0;//最少读取到的标签次数，少于此数目，则认为为误读
        //该表将存储读到的标签信息
        private DataTable tagsInfo = new DataTable("tagsInfo");
        private DataTable tagsBuffer = new DataTable("bufferTable");
        System.Timers.Timer refreshTimer = new System.Timers.Timer();
        public bool bBusy = false;

        ManualResetEvent resetState = new ManualResetEvent(true);

        public TDJ_RFIDHelper()
        {
            //this.tagsInfo.CaseSensitive = true;

            tagsInfo.Columns.Add("epc", typeof(string));
            tagsInfo.Columns.Add("antennaID", typeof(string));
            tagsInfo.Columns.Add("tagType", typeof(string));
            tagsInfo.Columns.Add("readCount", typeof(string));
            tagsInfo.Columns.Add("milliSecond", typeof(long));
            tagsInfo.Columns.Add("getTime", typeof(string));
            tagsInfo.Columns.Add("state", typeof(string));


            tagsBuffer.Columns.Add("epc", typeof(string));
            tagsBuffer.Columns.Add("antennaID", typeof(string));
            tagsBuffer.Columns.Add("readCount", typeof(int));
            tagsBuffer.Columns.Add("milliSecond", typeof(long));
            tagsBuffer.Columns.Add("getTime", typeof(string));

        }
        public List<TagInfo> getTagList()
        {
            this.resetState.WaitOne();
            this.resetState.Reset();
            List<TagInfo> list = new List<TagInfo>();
            DataRowCollection rows = null;
            rows = this.tagsInfo.Rows;

            foreach (DataRow dr in rows)
            {
                TagInfo ti = new TagInfo();
                ti.epc = dr["epc"].ToString();
                ti.antennaID = dr["antennaID"].ToString();
                Debug.WriteLine("getTagList ->  epc = " + ti.epc + "   antenna = " + ti.antennaID);
                list.Add(ti);
            }
            this.resetState.Set();

            return list;
        }
        //还没用
        public void deleteExpiredTags(TagInfo ti)
        {
            DataRow[] rows = null;
            long span = (long)(ti.milliSecond - lostDelay);
            if (span > 0)
            {
                rows = tagsInfo.Select("milliSecond < " + span.ToString() + "");
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        //Debug.WriteLine(string.Format("deleteExpiredTags :  span -> {0}   millis -> {1} epc -> {2}", span.ToString(), rows[i]["milliSecond"], rows[i]["epc"]));

                        //rows[i]["state"] = "deleted";
                        this.tagsInfo.Rows.Remove(rows[i]);
                    }

                    this.outputTagTable();
                }
            }
        }
        public void tagDeleted(int timespan)
        {
            //this.resetState.WaitOne();
            //this.resetState.Reset();
            DataRow[] rows = null;
            TimeSpan ts = DateTime.Now - PublicConfig.staticClass.timeBase;
            long span = (long)(ts.TotalMilliseconds - timespan);
            if (timespan > 0)
            {
                rows = tagsInfo.Select("milliSecond < " + span.ToString() + "");
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        Debug.WriteLine(string.Format("tagDeleted :  span -> {0}   millis -> {1} epc -> {2}", span.ToString(), rows[i]["milliSecond"], rows[i]["epc"]));


                        //删除缓存里关于该标签的记录
                        DataRow[] rowsToRemove = this.tagsBuffer.Select("epc = '" + rows[i]["epc"] + "'");
                        if (rowsToRemove.Length > 0)
                        {
                            for (int j = 0; j < rowsToRemove.Length; j++)
                            {
                                this.tagsBuffer.Rows.Remove(rowsToRemove[j]);
                            }
                        }

                        //rows[i]["state"] = "deleted";
                        this.tagsInfo.Rows.Remove(rows[i]);


                    }

                    this.outputTagTable();
                }
            }
            //this.resetState.Set();

        }

        //接收串口或者其它方式接收到的标签信息，
        public void ParseDataToTag(string data)
        {
            this.tagDeleted(lostDelay);
            //this.bBusy = true;
            if (data == null || data.Length <= 0)
            {
                return;
            }
            Debug.WriteLine(
                string.Format("TDJ_RFIDHelper.ParseDataToTag  ->  = {0}"
                , data));
            this.stringBuilder.Append(data);
            int tagLength = 110;//每条数据的标准长度为110
            string temp1 = this.stringBuilder.ToString();
            //Debug.WriteLine(temp1);
            int start = temp1.IndexOf("Disc:");
            if (start < 0)
            {
                return;
            }
            int tempStart = start;
            int lastDiscIndex = start;
            while (true)//找到最后一个Disc，并且其后有满格式的数据，即长度为110
            {
                int DiscIndex = temp1.IndexOf("Disc:", lastDiscIndex + 1);
                if (DiscIndex == -1)
                {
                    break;
                }
                else
                {
                    if (temp1.Length < DiscIndex + tagLength)
                    {
                        break;
                    }
                }
                lastDiscIndex = DiscIndex;
            }
            //int tail = lastDiscIndex + 110;
            int tail = lastDiscIndex - 1;

            string temp = this.stringBuilder.ToString(start, tail - start + 1);
            //string temp = this.stringBuilder.ToString(start, tail + 2 - start + 1);

            this.stringBuilder.Remove(0, tail + 1);//正确数据之前的数据已经没用了

            for (int i = 0; i < temp.Length; i++)
            {
                string tagInfo = string.Empty;
                int startIndex = temp.IndexOf("Disc", i);
                string restStr = "no rest";
                if (startIndex >= 0)
                {
                    restStr = temp.Substring(startIndex);
                }
                Debug.WriteLine(
                    string.Format("TDJ_RFIDHelper.ParseDataToTag  -> startIndex = {0} lastDiscIndex = {1}  rest temp = {2}"
                    , startIndex, lastDiscIndex, restStr));
                if (startIndex == -1)
                {
                    this.bBusy = false;
                    return;
                }
                if (temp.Length - startIndex >= tagLength)
                {

                    tagInfo = temp.Substring(startIndex, tagLength);
                }
                else
                {
                    this.bBusy = false;
                    return;
                }
                Debug.WriteLine(
                    string.Format("TDJ_RFIDHelper.ParseDataToTag  -> parsed = {0}"
                    , tagInfo));
                TagInfo ti = TagInfo.Parse(tagInfo);
                if (null != ti)
                {

                    this.AddNewTag2Table(ti);
                    //this.bBusy = false;
                    //return;
                }

                //this.tagDeleted(lostDelay);

                i = startIndex + tagLength;
            }
            this.bBusy = false;
        }
        public void ReceiveNewTag()
        {
            string temp1 = this.stringBuilder.ToString();
            int start = temp1.IndexOf("Disc:");
            int end = temp1.IndexOf("\r\n");
            if (start >= 0 && end > 110 && start < end)
            {
                string temp = this.stringBuilder.ToString(start, end + 2);
                this.stringBuilder.Remove(0, end + 2);

                for (int i = 0; i < temp.Length; i++)
                {
                    string tagInfo = string.Empty;
                    int startIndex = temp.IndexOf("Disc", i);
                    if (startIndex == -1)
                    {
                        return;
                    }
                    if (temp.Length - startIndex > 110)
                    {
                        tagInfo = temp.Substring(startIndex, 110);
                    }
                    else
                    {
                        return;
                    }
                    TagInfo ti = TagInfo.Parse(tagInfo);
                    if (null == ti)
                    {
                        return;
                    }
                    this.AddNewTag2Table(ti);
                    i = startIndex + 110;
                }
            }
        }
        public void ReceiveNewTag(string tagInfo)
        {
            TagInfo ti = TagInfo.Parse(tagInfo);
            if (null == ti)
            {
                return;
            }
            this.AddNewTag2Table(ti);
        }
        public void outputTagTable()
        {
            Debug.WriteLine("*******************************************************************");
            DataRowCollection rows = this.tagsInfo.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                Debug.WriteLine(string.Format("epc -> {0}  antennaID -> {1}  readCount = {2}", row["epc"].ToString(), row["antennaID"].ToString(), row["readCount"].ToString()));
            }

        }


        // 标签的噪声过滤有两种情况：
        // 1 有没有标签的过滤
        //        这种情况之下有两种形式：添加新标签和删除旧标签
        //        每当有新标签，需要记录标签采集到的时间，如果超过过滤时间，仍能收到该标签，说明
        //        该标签非误读所致
        //        如果在间隔过滤时间之后一次没有收到标签采集到的数据，说明该标签已经不存在。这需要更新
        //        每次读取到标签的时间点
        //   每次读到当前列表中没有的标签，将该标签添加到缓存列表中。如果列表中已经存在该标签，则检查
        //   读取时间是否已经超过过滤时间，如果已经超过，则将其添加到当前列表中，并从缓存列表中移除
        //   
        // 2 天线之间误读的过滤
        //        这里主要通过在过滤时间内标签读取到的总次数确定
        //        每次读到标签，先要放到缓冲表中，如果缓冲表中已经存在该标签并且天线不一致，更新读到
        //        的次数；如果超出过滤时间，将读取次数多的添加到当前列表中

        // 总之，处理流程如下：
        // 当有标签进入时，检查当前列表中有无超出过滤时间的标签，有则移除
        // 将标签id，当前时间点，读取次数，天线id记录到缓冲表中
        // 检查有无超出过滤时间的标签；如果有，则继续检查是否通过同一天线读到，如果是则直接添加到
        // 当前列表中；如果不是通过同一天线读到，则要比较将读取到次数多的标签添加到当前列表中

        /// <summary>
        /// 将新解析完的标签尝试添加到列表中
        /// 首先要检查列表中是否已经有新标签的epc，如果已经有标签epc，查看天线编号是否一致，如果天线编号一致，则替换原有的
        /// 标签信息，如果天线编号不一致，则查看是否在缓冲时间段内，如果是则表明这可能是误读，要用读取次数多的标签信息代替
        /// 读取次数少的标签信息；如果不在缓冲时间段内，则认为标签已经改变了位置
        /// 因此，导致表内信息改变的情况有以下几种：
        /// 1 epc不存在，加到表中
        /// 2 epc存在，且天线编号一致，新的代替旧的
        /// 3 epc存在，缓冲时间段内，天线编号不一致，用读取次数多的代替少的
        /// 4 epc存在，非缓冲时间段内，天线编号不一致，新的代替旧的
        /// </summary>
        /// <param name="ti"></param>
        public void AddNewTag2Table(TagInfo ti)
        {
            //////////////////////////////////////////////////////////////////////////
            // new versioin
            DataRow[] rows = null;

            if (ti.antennaID == "01" || ti.antennaID == "02" || ti.antennaID == "04" || ti.antennaID == "08")
            {

                #region 非误读标签的处理操作


                // 将标签id，当前时间点，读取次数，天线id记录到缓冲表中
                // 读写器可能重复发送，这里把多余的数据过滤掉
                rows = tagsBuffer.Select(string.Format("epc = '{0}' and getTime = '{1}'", ti.epc, ti.getTime));
                if (rows.Length <= 0)
                {
                    this.tagsBuffer.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.readCount, ti.milliSecond });
                }
                //检查标签是否超出过滤时间的方法就是是否有 小于 ti.milliSecond - 过滤时间 的记录
                DataRow[] rowsToRemove = null;
                DataRow[] rowsWithEpc = null;

                rowsWithEpc = tagsBuffer.Select(string.Format("epc = '{0}'"
                                                        , ti.epc));// 把所有当前数据都作为评判依据
                if (rowsWithEpc.Length > 0)//否则没超过过滤时间，不需要操作
                {
                    //先检查和该标签对应的天线的id的种数，如果只有一种，那说明没有误读
                    List<string> antennaList = new List<string>();
                    for (int i = 0; i < rowsWithEpc.Length; i++)
                    {
                        string s = rowsWithEpc[i]["antennaID"].ToString();
                        if (!antennaList.Contains(s))
                        {
                            antennaList.Add(s);
                        }
                    }

                    if (antennaList.Count > 0)
                    {
                        string currentAntenna = antennaList[0];
                        int currentTotalCount = 0;
                        int rowsCount = 0;
                        // 如果数目大于1，那就直接比较从读到当面标签的时间向前过滤时间这段时间内的读取次数
                        // 以读取次数多的为准
                        int readCountGap = -1;//多个天线读取到标签的次数的差值，差距到一定程度再发生变化
                        if (antennaList.Count > 1)
                        {
                            Debug.WriteLine("有多个天线对应一个标签");
                            foreach (string s in antennaList)
                            {
                                rows = tagsBuffer.Select(string.Format("epc = '{0}' and milliSecond < {1} and (antennaID in ({2}))"
                                    , ti.epc, ti.milliSecond, "'" + s + "'"));
                                int totalReadCount = 0;
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    totalReadCount += (int)rows[j]["readCount"];
                                }
                                if (totalReadCount > currentTotalCount)
                                {
                                    currentTotalCount = totalReadCount;
                                    currentAntenna = s;
                                    rowsCount = rows.Length;
                                }
                                Debug.WriteLine(string.Format("epc = {0}  antena = {1} count = {2}   readcount = {3}"
                                    , ti.epc, s, rows.Length, totalReadCount));
                            }
                            Debug.WriteLine(string.Format("max readcount   antena = {0}   readcount = {1}"
                                , currentAntenna, currentTotalCount));
                        }
                        else
                        {
                            Debug.WriteLine(" 没有多个天线对应一个标签");
                            rows = tagsBuffer.Select(string.Format("epc = '{0}' and milliSecond < {1} and (antennaID in ({2}))"
                                , ti.epc, ti.milliSecond, "'" + antennaList[0] + "'"));
                            int totalReadCount = 0;
                            for (int j = 0; j < rows.Length; j++)
                            {
                                totalReadCount += (int)rows[j]["readCount"];
                            }
                            rowsCount = rows.Length;
                            currentTotalCount = totalReadCount;
                        }

                        //将该标签添加到当前标签列表中
                        // 统计时间必须超过过滤时间，并且读取到的次数超过一定值，这样才靠谱
                        double minMillionSecond = double.Parse(rowsWithEpc[0]["milliSecond"].ToString());//第一行是最小的时间值
                        Debug.WriteLine(
                            string.Format("TDJ_RFIDHelper.AddNewTag2Table  -> 最开始记录时间 = {0}    当前运行时间 = {1}"
                            , minMillionSecond, ti.milliSecond - this.lostDelay));
                        if ((minMillionSecond <= ti.milliSecond - this.lostDelay) && rowsCount > this.lostDelay / 1000 / 3)//10秒内只是有5次成功读到
                        //if ((minMillionSecond <= ti.milliSecond - this.lostDelay) && currentTotalCount >= 5)
                        {
                            DataRow[] rowsCurrent = null;
                            rowsCurrent = tagsInfo.Select("epc = '" + ti.epc + "'");
                            if (rowsCurrent.Length > 0)
                            {
                                //更新数据
                                rowsCurrent[0]["milliSecond"] = ti.milliSecond;
                                rowsCurrent[0]["getTime"] = ti.getTime;
                                rowsCurrent[0]["antennaID"] = currentAntenna;
                            }
                            else
                            {
                                tagsInfo.Rows.Add(new object[] { 
                            ti.epc, currentAntenna, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                            }
                        }
                    }

                    #region 过期数据处理

                    //过期的数据删除掉，如果数据太少，就保留作分析用
                    rowsToRemove = tagsBuffer.Select(string.Format("epc = '{0}' and milliSecond <= {1}"
                                                        , ti.epc, ti.milliSecond - this.lostDelay));
                    int rowsToReserve = this.lostDelay / 1000;
                    if ((rowsWithEpc.Length > rowsToReserve) && rowsToRemove.Length > 0)
                    {
                        int countToRemove = rowsWithEpc.Length - rowsToReserve;
                        if (rowsToRemove.Length < countToRemove)
                        {
                            countToRemove = rowsToRemove.Length;
                        }
                        //为缓存保留至少n条数据
                        for (int k = 0; k < countToRemove; k++)
                        {
                            tagsBuffer.Rows.Remove(rowsToRemove[k]);
                        }
                    }

                    #endregion

                }

                #endregion

            }
            else
            {
                #region 如果标签是误读的处理操作
                //标签虽然误读，但是最起码说明这个标签还是存在的
                //DataRow[] rowsCurrent = null;
                //rowsCurrent = tagsInfo.Select("epc = '" + ti.epc + "'");
                //if (rowsCurrent.Length > 0)
                //{
                //    //更新数据
                //    rowsCurrent[0]["milliSecond"] = ti.milliSecond;
                //    rowsCurrent[0]["getTime"] = ti.getTime;
                //}

                #endregion
            }

            return;
            //////////////////////////////////////////////////////////////////////////

            //
            //DataRow[] rows = null;

            if (ti.readCount < this.minReadCount)
            {
                return;
            }
            this.resetState.WaitOne();
            this.resetState.Reset();
            rows = tagsInfo.Select("epc = '" + ti.epc + "'");
            if (rows.Length <= 0)//epc不存在，加到表中
            {
                this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
            }
            else
            {
                if (ti.antennaID == rows[0]["antennaID"].ToString())//天线编号一致
                {
                    this.tagsInfo.Rows.Remove(rows[0]);
                    this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                }
                else//天线编号不一致
                {
                    {
                        //没超出一秒，有可能超出缓冲时间，需要比较毫秒
                        long oldM = 0;
                        try
                        {
                            oldM = long.Parse(rows[0]["milliSecond"].ToString());
                        }
                        catch (System.Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        if ((ti.milliSecond - oldM) > this.milliSecondDelay)//超出缓冲时间
                        {
                            this.tagsInfo.Rows.Remove(rows[0]);
                            this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                        }
                        else
                        {
                            //读取次数多的代替少的
                            int oldC = 0;
                            try
                            {
                                oldC = int.Parse(rows[0]["readCount"].ToString());
                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine(ex.Message);

                            }
                            if (ti.readCount > oldC)
                            {
                                this.tagsInfo.Rows.Remove(rows[0]);
                                this.tagsInfo.Rows.Add(new object[] { ti.epc, ti.antennaID, ti.tagType, ti.readCount, ti.milliSecond, ti.getTime, "new" });
                            }
                        }
                    }
                }
            }
            this.resetState.Set();
#if TRACE
            Debug.WriteLine("AddNewTag2Table  <- ");
#endif

            this.outputTagTable();
        }

        List<byte> maxbuf = new List<byte>();
        StringBuilder stringBuilder = new StringBuilder();

    }
}
