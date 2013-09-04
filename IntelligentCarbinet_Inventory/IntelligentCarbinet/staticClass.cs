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
        public static int tagsBufferTime = 1000 * 60 * 15;//读取标签的缓冲时间
        public static int storageCheckInterval = 10;//安全库存检查时间间隔
        public static int udpPort = 5000;

        public static string RestAddress = "http://172.16.13.35:9002/index.php/";
        public static string addLedInfo = RestAddress + "LED/led/addLedInfo";
        public static string addCommandInfo = RestAddress + "LED/CommandInfo/addCommandInfo";

        public static DateTime timeBase = DateTime.Now;
        public static string PicturePath = @"商品图片\";
        public static string configFilePath = "app.config";
        //public static DBType currentDbType = DBType.sqlite;
        public static string currentDBConnectString = string.Empty;
        public static double baseWidth = 1280;
        public static double baseHeight = 770;
        public static double widthPara = 1;//屏幕尺寸系数
        public static double heightPara = 1;

        public static int getRealHeight(int height)
        {
            return (int)(height * heightPara);
        }
        public static int getRealWidth(int width)
        {
            return (int)(width * widthPara);
        }
        public static void setScreenPara()
        {
            Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                Screen sc = screens[i];
                if (sc.Primary == true)
                {
                    Rectangle rect = sc.WorkingArea;
                    widthPara = rect.Width / baseWidth;
                    heightPara = rect.Height / baseHeight;
                }

            }
        }

        /*
         Data Source=127.0.0.1\SQLExpress;Initial Catalog=IMS;User ID=sa;pwd=078515
         */
    }
}
