using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Log
{
    
    public class txtLog
    {
        static StreamWriter swWrite = null;//写文件
        static string logpath = null;//日志路径

        /// <summary>
        /// 判断是否有当天日志，如果没有则创建
        /// </summary>
        /// <param name="sCurDate">日期</param>
        /// <param name="pathweb">路径</param>
        static void InitLog(string sCurDate, string pathweb)
        {
            try
            {
                if (sCurDate != "" || sCurDate != null)
                {
                    logpath = pathweb + "\\" + sCurDate + ".txt";
                }
                if (!File.Exists(logpath))
                {
                    swWrite = File.CreateText(logpath);
                }
                else
                {
                    swWrite = File.AppendText(logpath);
                }
            }
            catch (System.Exception ex)
            {
            	
            }

        }

        /// <summary>
        /// 日志主方法
        /// </summary>
        /// <param name="sMess">错误</param>
        /// <param name="ex">异常</param>
        /// <param name="path1">日志路径</param>
        public static void LogError(string sMess, Exception ex)
        {
#if LOG_OUTPUT
            DateTime dt = DateTime.Now;
            string sCurDate = dt.ToString("yyyy-MM-dd");
            if (swWrite == null)
            {
                InitLog(sCurDate, ".");
            }
            swWrite.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            swWrite.WriteLine("输出信息：" + sMess);
            if (ex != null)
            {
                swWrite.WriteLine("异常信息：\r\n" + ex.ToString());
                swWrite.WriteLine("异常堆栈：\r\n" + ex.StackTrace);
            }
            swWrite.WriteLine("\r\n");
            swWrite.Flush();
            swWrite.Dispose();
            swWrite = null;
#endif
        }
    }
}
