using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentCarbinet
{
    public class CommandInfo
    {
        public string commandName = string.Empty;
        public string info = string.Empty;
        public string startTime = string.Empty;
        public string ledIP = string.Empty;
        public string state = string.Empty;

        public CommandInfo()
        {

        }
        public CommandInfo(string _commandName, string _info, string _time, string _ip)
        {
            this.commandName = _commandName;
            this.info = _info;
            this.startTime = _time;
            this.ledIP = _ip;
        }
    }

    public class LedInfo
    {
        public string info = string.Empty;
        public string startTime = string.Empty;
        public string ledIP = string.Empty;
        public string state = string.Empty;

        public LedInfo()
        {

        }
        public LedInfo(string _info, string _time, string _ip)
        {
            this.info = _info;
            this.startTime = _time;
            this.ledIP = _ip;
        }
    }
}
