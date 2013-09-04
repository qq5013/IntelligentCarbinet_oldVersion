using System;
using System.Collections.Generic;
using System.Text;
using IntelligentCarbinet;
using System.Windows.Forms;
using System.Drawing;

namespace PublicConfig
{
    public static class staticClass
    {
        public static int tagsBufferTime = 15000;//读取标签的缓冲时间
        public static int storageCheckInterval = 10;//安全库存检查时间间隔
        public static int udpPort = 5000;
        public static string restIP = string.Empty;
        public static string restPort = string.Empty;

        public static string RestAddress = "http://192.168.1.120:9002/index.php/LED/CommandInfo/addCommandInfo";
        public static string getRestAddress()
        {
            return string.Format("http://{0}:{1}/index.php/LED/CommandInfo/addCommandInfo", restIP, restPort);
        }
        public static MonitorMode mode = MonitorMode.讲解模式;
        public static DateTime timeBase = DateTime.Now;
        public static string PicturePath = @"商品图片\";
        public static string configFilePath = "app.config";
        //public static DBType currentDbType = DBType.sqlite;
        public static string currentDBConnectString = string.Empty;

        public static int baseWidth = 80;
        public static int baseHeight = 20;

        public static int realWidth = 0;
        public static int realHeight = 0;


        public static int getRealHeight(int height)
        {
            return (height * realHeight / baseHeight);
        }
        public static int getRealWidth(int width)
        {
            return (width * realWidth / baseWidth);
        }
        public static void setScreenPara(int width, int heigth)
        {
            realHeight = heigth;
            realWidth = width;
        }
        public static Point getRealPoint(Point p)
        {
            Point _p = new Point(p.X * realWidth / baseWidth, p.Y * realHeight / baseHeight);
            return _p;
        }
        public static Size getRealSize(Size s)
        {
            Size _s = new Size(s.Width * realWidth / baseWidth, s.Height * realHeight / baseHeight);
            return _s;
        }
        /*
         Data Source=127.0.0.1\SQLExpress;Initial Catalog=IMS;User ID=sa;pwd=078515
         */
    }
}
