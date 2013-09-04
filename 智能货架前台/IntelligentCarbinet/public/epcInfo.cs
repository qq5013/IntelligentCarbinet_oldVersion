using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentCarbinet
{
    public class epcInfo
    {
        public epcInfo(string _epc, string _type)
        {
            this.epc = _epc;
            this.type = _type;
        }
        public string epc = string.Empty;
        public string type = string.Empty;
    }
}
