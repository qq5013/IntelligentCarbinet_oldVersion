using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PublicConfig;
using System.Drawing;

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
