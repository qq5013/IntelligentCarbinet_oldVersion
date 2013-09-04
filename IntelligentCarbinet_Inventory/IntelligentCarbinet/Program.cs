using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PublicConfig;
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


            MemoryTable.memoryTableInitilize();
        }
    }
}
