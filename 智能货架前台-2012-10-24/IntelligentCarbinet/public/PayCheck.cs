using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using httpHelper;
using System.Security.Cryptography;
using System.Diagnostics;

namespace KeyChec
{
    public class PayCheck
    {
        static string key_check_url = "http://www.iot-top.com:10000/index.php/PayCheck/CheckKey";
        static Timer _timer = new Timer();
        static string strSource = "yingkou2012";
        static string strCrypto;
        public static void start_loop_check(string source)
        {
            strSource = source;
            _timer.Interval = 1000 * 60 * 5;
            _timer.Tick += new EventHandler(_timer_Tick);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] targetData = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strSource));
            string byte2String = "";

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += System.Convert.ToString(targetData[i], 16).PadLeft(2, '0');
            }
            strCrypto = byte2String.PadLeft(32, '0');

            _timer.Enabled = true;
        }

        static void _timer_Tick(object sender, EventArgs e)
        {
            HttpWebConnect helper = new HttpWebConnect();
            helper.RequestCompleted += new deleGetRequestObject(helper_RequestCompleted_get_syc_list);
            helper.TryPostData(key_check_url, strCrypto);
        }
        static void helper_RequestCompleted_get_syc_list(object o)
        {
            string strres = (string)o;
            Debug.WriteLine("checked");
            if (strres == "failed")
            {
                MessageBox.Show("软件版本已过期，请联系提供商！", "提示");
                Application.Exit();
            }

        }
    }
}
