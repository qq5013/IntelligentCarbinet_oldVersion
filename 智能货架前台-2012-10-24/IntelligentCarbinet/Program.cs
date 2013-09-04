using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PublicConfig;
using System.Drawing;
using KeyChec;

namespace IntelligentCarbinet
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            resetConfig();
            PayCheck.start_loop_check("yingkou2012");

            Application.Run(new frmMain());
            //Application.Run(new Form1());
            //Application.Run(new test());
//            TDJ_RFIDHelper helper = new TDJ_RFIDHelper();
//            string str = @"Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C001010101   
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C001010102  
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C001010103
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:43, Count:00019, Ant:02, Type:04, Tag:300833B2DDD906C001010101   
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:44, Count:00019, Ant:01, Type:04, Tag:300833B2DDD906C001010101   
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:45, Count:00019, Ant:06, Type:04, Tag:300833B2DDD906C001010101   
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:46, Count:00019, Ant:06, Type:04, Tag:300833B2DDD906C001010101   
//   
//Disc:2000/02/28 20:01:51, Last:2000/02/28 20:07:42, Count:00019, Ant:02, Type:04, ";
//            helper.ParseDataToTag(str);
        }
        public static void resetConfig()
        {
            //初始化一些设置
            object otagsBufferTime = nsConfigDB.ConfigDB.getConfig("tagsBufferTime");
            if (null != otagsBufferTime)
            {
                staticClass.tagsBufferTime = int.Parse((string)otagsBufferTime);
            }
            object oInterval = nsConfigDB.ConfigDB.getConfig("checkStorageInterval");
            if (oInterval != null)
            {
                staticClass.storageCheckInterval = int.Parse((string)oInterval);
            }
            object oPort = null;
            oPort = nsConfigDB.ConfigDB.getConfig("portListening");
            if (oPort != null)
            {
                staticClass.udpPort = int.Parse((string)oPort);
            }
            object orestIP = nsConfigDB.ConfigDB.getConfig("restIP");
            if (orestIP != null)
            {
                staticClass.restIP = (string)orestIP;
            }
            object orestPort = nsConfigDB.ConfigDB.getConfig("restPort");
            if (orestPort != null)
            {
                staticClass.restPort = (string)orestPort;
            }

            MemoryTable.memoryTableInitilize();
        }

    }
}
