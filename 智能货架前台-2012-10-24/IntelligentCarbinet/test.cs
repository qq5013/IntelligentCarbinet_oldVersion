using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace IntelligentCarbinet
{
    public partial class test : Form, ITagListener
    {
        EpcProductHelper helper = new EpcProductHelper();
        public test()
        {
            InitializeComponent();
            helper.listener = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            helper.start(5000);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<EpcProduct> list = helper.GetCurrentProductList();
            foreach (EpcProduct ep in list)
            {
                Debug.WriteLine(string.Format("epc => {0}   antena => {1}", ep.tagInfo.epc, ep.tagInfo.antennaID));
            }
        }

        #region ITagListener 成员

        public void NewTagMessage()
        {
            List<EpcProduct> list = helper.GetCurrentProductList();
            if (list.Count > 0)
            {
                foreach (EpcProduct ep in list)
                {
                    Debug.WriteLine(string.Format("epc => {0}   antena => {1}", ep.tagInfo.epc, ep.tagInfo.antennaID));
                }
            }
            else
            {
                Debug.WriteLine("no products now!!!");
            }
        }

        #endregion
    }
}
