using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RemoteAdminPro
{
    public partial class ManualTic : Form
    {
        RemoteAdmin ra;
        string recievedCode;
        public ManualTic(RemoteAdmin _ra, string code)
        {
            InitializeComponent();
            ra = _ra;
            recievedCode = code;
            textBox1.Text = recievedCode;
            Focus();
            textBox1.Select(0, textBox1.Text.Length);
        }

        private void ManualTic_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }
        }

        private void TouchKeyboard(object sender, EventArgs e)
        {
            Button s = sender as Button;
            textBox1.Text += s.Text;
            Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            Focus();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Focus(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ra.label6.Text = textBox1.Text;
            ra.CheckTicket(textBox1.Text);
            this.Close();
        }
    }
}