using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;

namespace RemoteAdminPro
{
    public partial class IPconfig : Form
    {
        RemoteAdmin RA = null;
        public IPconfig(RemoteAdmin _ra)
        {
            RA = _ra;
            InitializeComponent();
            textBox1.Text = "10.10.30.";
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IpValidator(textBox1.Text))
                this.Close();
            else
                MessageBox.Show("niepoprawny format adresu IP!");
        }
        private bool IpValidator(string str)
        {
            Regex regex = new Regex(@"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])");
            Match match = regex.Match(str);
            try
            {
                IPAddress ip = IPAddress.Parse(str);
            }
            catch
            {
                return false;
            }
            if (match.Success)
                return true;
            return false;
        }
        private void TouchKeyboard(Button sender, EventArgs e)
        {
            
        }

        private void TouchKeyboard(object sender, EventArgs e)
        {
            Button s = sender as Button;
            textBox1.Text += s.Text;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
        }
    }
}