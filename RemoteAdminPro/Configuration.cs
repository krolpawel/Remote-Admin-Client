using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace RemoteAdminPro
{
    [Serializable()]
    public class Configuration
    {
        public string brightness;
        public string gk;
        public int timerInterval;
        public string lPass;
        public string message;
        public int scanMode;


        public Configuration()
        {
            brightness = "";
            gk = "";
            timerInterval = 0;
            lPass = "1379";
            message = "";
            scanMode = 0;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string LPass
        {
            get { return lPass; }
            set { lPass = value; }
        }
        public string Brightness
        {
            get { return brightness; }
            set { brightness = value; }
        }
        public string Gk
        {
            get { return gk; }
            set { gk = value; }
        }
        public int TimerInterval
        {
            get { return timerInterval; }
            set { timerInterval = value; }
        }
        public int ScanMode
        {
            get { return scanMode; }
            set { scanMode = value; }
        }
    }
}
