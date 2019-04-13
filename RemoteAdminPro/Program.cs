using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RemoteAdminPro
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            RemoteAdmin window;
            Application.Run(window = new RemoteAdmin());
        }
    }
}