using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using httpHelper;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;
using System.Windows.Forms;

namespace ResSync
{
    public class ResSyncer
    {
        //public static string server_ip = "http://118.202.124.35:9002";
        static string server_ip = "http://localhost:9002";
        string url_download_file_base = server_ip + "/index.php/ResSync/resourceSync/download_file/name/";
        string url_get_syc_list = server_ip + "/index.php/ResSync/resourceSync/get_syc_list";
        string local_file_path = "./商品图片/";
        public string lastTagTimeStamp = string.Empty;
        string group = string.Empty;
        List<string> file_name_list = new List<string>();
        static string configFile = "res_sync_config.xml";
        DataSet ds = null;
        public ResSyncer()
        {
            this.InitialConfigFile();
        }
        public void start_sync(string group)
        {
            this.group = group;
            this.start_sync();
        }
        public void start_sync()
        {
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_get_syc_list);
            helper.TryPostData(this.get_syc_list_url(), lastTagTimeStamp);
        }
        string get_syc_list_url()
        {
            if (this.group != string.Empty)
            {
                return url_get_syc_list + "/group/" + this.group;
            }
            else
            {
                return url_get_syc_list;
            }
        }
        void helper_RequestCompleted_get_syc_list(object o)
        {
            string strres = (string)o;
            Debug.WriteLine(
                string.Format("ResSyncer:helper_RequestCompleted_get_syc_list  ->  = {0}"
                , strres));
            object olist = fastJSON.JSON.Instance.ToObjectList(strres, typeof(List<res>), typeof(res));
            List<res> resList = (List<res>)olist;
            if (resList.Count > 0)
            {
                this.lastTagTimeStamp = resList[0].time_stamp;
                this.WriteNewTimeStamp();
                for (int i = 0; i < resList.Count; i++)
                {
                    res temp = resList[i];
                    this.file_name_list.Add(temp.resID);
                }
                myWebClient_DownloadFileCompleted(null, null);//开始下载文件
            }
        }
        void DownLoadFileInBackground(string fileName)
        {
            string url;
            if (this.group == string.Empty)
            {
                url = this.get_url_download_file(fileName);
            }
            else
            {
                url = this.get_url_download_file(fileName, this.group);
            }
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(myWebClient_DownloadFileCompleted);
            myWebClient.DownloadFileAsync(new Uri(url), local_file_path + fileName);
        }

        void myWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.file_name_list.Count > 0)
            {
                string fileName = this.file_name_list[0];
                this.file_name_list.RemoveAt(0);
                this.DownLoadFileInBackground(fileName);
            }
            else
            {
                MessageBox.Show("同步完成！", "提示");
            }
        }
        //拼凑下载文件的链接
        string get_url_download_file(string fileName, string group)
        {
            return get_url_download_file(fileName) + "/group/" + group;
        }
        string get_url_download_file(string fileName)
        {
            return url_download_file_base + fileName;
        }
        //保存同步时间戳
        void WriteNewTimeStamp()
        {
            DataTable dt = ds.Tables["config"];
            DataRow[] rows = dt.Select("key = 'time_stamp'");
            if (rows.Length > 0)
            {
                rows[0]["value"] = lastTagTimeStamp;
            }
            else
            {
                dt.Rows.Add(new object[] { "time_stamp", lastTagTimeStamp });
            }
            ds.WriteXml(configFile);
        }

        //初始化配置文件，初始化同步时间标签
        void InitialConfigFile()
        {
            try
            {
                ds = new DataSet("nsConfig");
                ds.Namespace = "";
                if (!File.Exists(configFile))
                {

                    ds.WriteXml(configFile);
                }
                else
                {
                    ds.ReadXml(configFile);
                    DataTable dt = ds.Tables["config"];
                    DataRow[] rows = dt.Select("key = 'time_stamp'");
                    if (rows.Length > 0)
                    {
                        lastTagTimeStamp = rows[0]["value"] as string;
                    }
                }
                if (ds.Tables.IndexOf("config") == -1)
                {
                    DataTable dt = new DataTable("config");
                    dt.Columns.Add("key", typeof(string));
                    dt.Columns.Add("value", typeof(object));
                    ds.Tables.Add(dt);
                    ds.WriteXml(configFile);
                }
            }
            catch
            {
            }
        }
    }
}
