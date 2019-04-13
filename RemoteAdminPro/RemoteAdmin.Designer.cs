namespace RemoteAdminPro
{
    partial class RemoteAdmin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteAdmin));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.timer1 = new System.Windows.Forms.Timer();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.btCheckPass = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pbBackupBattery = new System.Windows.Forms.ProgressBar();
            this.btConfirm = new System.Windows.Forms.Button();
            this.lblGKServer = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.lblWWWServer = new System.Windows.Forms.Label();
            this.btClose = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbBacklight = new System.Windows.Forms.TrackBar();
            this.label12 = new System.Windows.Forms.Label();
            this.tbResponse = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbReporting = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btManual = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.pbBattery = new System.Windows.Forms.ProgressBar();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(2, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(235, 20);
            this.label7.Text = "Hasło";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(73, 43);
            this.tbPass.Name = "tbPass";
            this.tbPass.PasswordChar = '*';
            this.tbPass.Size = new System.Drawing.Size(100, 21);
            this.tbPass.TabIndex = 17;
            this.tbPass.TextChanged += new System.EventHandler(this.tbPass_TextChanged);
            this.tbPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPass_KeyDown);
            // 
            // btCheckPass
            // 
            this.btCheckPass.Location = new System.Drawing.Point(85, 70);
            this.btCheckPass.Name = "btCheckPass";
            this.btCheckPass.Size = new System.Drawing.Size(72, 20);
            this.btCheckPass.TabIndex = 18;
            this.btCheckPass.Text = "->Panel";
            this.btCheckPass.Visible = false;
            this.btCheckPass.Click += new System.EventHandler(this.btCheckPass_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.None;
            this.tabControl1.Location = new System.Drawing.Point(0, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 229);
            this.tabControl1.TabIndex = 26;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.btCheckPass);
            this.tabPage1.Controls.Add(this.tbPass);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 206);
            this.tabPage1.Text = "Main";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.label4.Location = new System.Drawing.Point(2, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(237, 10);
            this.label4.Text = "v 2.11.5.0";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 5F, System.Drawing.FontStyle.Regular);
            this.label3.Location = new System.Drawing.Point(4, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 10);
            this.label3.Text = "Paweł Król (kontakt@pawelkrol.pl) © Copyright 2015";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(50, 112);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(148, 71);
            this.pictureBox1.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage2.Controls.Add(this.tabControl2);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 203);
            this.tabPage2.Text = "Inactive";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(232, 206);
            this.tabControl2.TabIndex = 27;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage4.Controls.Add(this.pbBackupBattery);
            this.tabPage4.Controls.Add(this.btConfirm);
            this.tabPage4.Controls.Add(this.lblGKServer);
            this.tabPage4.Controls.Add(this.btCancel);
            this.tabPage4.Controls.Add(this.lblWWWServer);
            this.tabPage4.Controls.Add(this.btClose);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.tbBacklight);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.tbResponse);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.cbReporting);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(232, 183);
            this.tabPage4.Text = "General";
            // 
            // pbBackupBattery
            // 
            this.pbBackupBattery.Location = new System.Drawing.Point(77, 12);
            this.pbBackupBattery.Name = "pbBackupBattery";
            this.pbBackupBattery.Size = new System.Drawing.Size(156, 15);
            // 
            // btConfirm
            // 
            this.btConfirm.BackColor = System.Drawing.Color.Lime;
            this.btConfirm.Location = new System.Drawing.Point(159, 160);
            this.btConfirm.Name = "btConfirm";
            this.btConfirm.Size = new System.Drawing.Size(72, 20);
            this.btConfirm.TabIndex = 21;
            this.btConfirm.Text = "Zatwierdź";
            this.btConfirm.Click += new System.EventHandler(this.btConfirm_Click);
            // 
            // lblGKServer
            // 
            this.lblGKServer.Location = new System.Drawing.Point(0, 142);
            this.lblGKServer.Name = "lblGKServer";
            this.lblGKServer.Size = new System.Drawing.Size(240, 15);
            this.lblGKServer.Text = "GK Server:";
            // 
            // btCancel
            // 
            this.btCancel.BackColor = System.Drawing.Color.Yellow;
            this.btCancel.Location = new System.Drawing.Point(81, 160);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(72, 20);
            this.btCancel.TabIndex = 20;
            this.btCancel.Text = "Anuluj";
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lblWWWServer
            // 
            this.lblWWWServer.Location = new System.Drawing.Point(0, 127);
            this.lblWWWServer.Name = "lblWWWServer";
            this.lblWWWServer.Size = new System.Drawing.Size(241, 15);
            this.lblWWWServer.Text = "App Server:";
            // 
            // btClose
            // 
            this.btClose.BackColor = System.Drawing.Color.Red;
            this.btClose.ForeColor = System.Drawing.Color.White;
            this.btClose.Location = new System.Drawing.Point(3, 160);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(72, 20);
            this.btClose.TabIndex = 19;
            this.btClose.Text = "Zamknij";
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label8.Location = new System.Drawing.Point(-4, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 18);
            this.label8.Text = "Bckp batt lvl";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label8.ParentChanged += new System.EventHandler(this.label8_ParentChanged);
            // 
            // tbBacklight
            // 
            this.tbBacklight.BackColor = System.Drawing.Color.Moccasin;
            this.tbBacklight.Location = new System.Drawing.Point(71, 33);
            this.tbBacklight.Maximum = 63;
            this.tbBacklight.Name = "tbBacklight";
            this.tbBacklight.Size = new System.Drawing.Size(170, 41);
            this.tbBacklight.TabIndex = 9;
            this.tbBacklight.TabStop = false;
            this.tbBacklight.ValueChanged += new System.EventHandler(this.tbBacklight_ValueChanged);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(7, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 20);
            this.label12.Text = "Backlight";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbResponse
            // 
            this.tbResponse.Enabled = false;
            this.tbResponse.Location = new System.Drawing.Point(109, 80);
            this.tbResponse.Name = "tbResponse";
            this.tbResponse.Size = new System.Drawing.Size(41, 21);
            this.tbResponse.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(7, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 17);
            this.label11.Text = "Response Time:";
            // 
            // cbReporting
            // 
            this.cbReporting.Checked = true;
            this.cbReporting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReporting.Enabled = false;
            this.cbReporting.Location = new System.Drawing.Point(-1, 104);
            this.cbReporting.Name = "cbReporting";
            this.cbReporting.Size = new System.Drawing.Size(100, 20);
            this.cbReporting.TabIndex = 15;
            this.cbReporting.Text = "Reporting";
            this.cbReporting.CheckStateChanged += new System.EventHandler(this.cbReporting_CheckStateChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(156, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(18, 20);
            this.label10.Text = "s";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage5.Controls.Add(this.button5);
            this.tabPage5.Controls.Add(this.button4);
            this.tabPage5.Controls.Add(this.button3);
            this.tabPage5.Controls.Add(this.comboBox1);
            this.tabPage5.Controls.Add(this.label16);
            this.tabPage5.Location = new System.Drawing.Point(0, 0);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(232, 180);
            this.tabPage5.Text = "Scanner";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Yellow;
            this.button5.Location = new System.Drawing.Point(81, 160);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(72, 20);
            this.button5.TabIndex = 24;
            this.button5.Text = "Anuluj";
            this.button5.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Red;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(3, 161);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 20);
            this.button4.TabIndex = 23;
            this.button4.Text = "Zamknij";
            this.button4.Click += new System.EventHandler(this.btClose_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Lime;
            this.button3.Location = new System.Drawing.Point(159, 160);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 20);
            this.button3.TabIndex = 22;
            this.button3.Text = "Zatwierdź";
            this.button3.Click += new System.EventHandler(this.btConfirm_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular);
            this.comboBox1.Items.Add("Stop");
            this.comboBox1.Items.Add("Entry");
            this.comboBox1.Items.Add("Exit");
            this.comboBox1.Items.Add("Bidirectional");
            this.comboBox1.Location = new System.Drawing.Point(57, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(175, 30);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.label16.Location = new System.Drawing.Point(8, 4);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 29);
            this.label16.Text = "Mode";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Moccasin;
            this.tabPage3.Controls.Add(this.label17);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Controls.Add(this.btManual);
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(232, 203);
            this.tabPage3.Text = "Scanner";
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(57, 183);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(114, 20);
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Moccasin;
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(0, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 139);
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.label15.Location = new System.Drawing.Point(65, 71);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(175, 20);
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.label14.Location = new System.Drawing.Point(3, 91);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(237, 20);
            this.label14.Text = "Device:";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.label13.Location = new System.Drawing.Point(3, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(233, 19);
            this.label13.Text = "Scanned:";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(237, 33);
            this.label5.Text = "Status";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btManual
            // 
            this.btManual.Location = new System.Drawing.Point(177, 183);
            this.btManual.Name = "btManual";
            this.btManual.Size = new System.Drawing.Size(56, 20);
            this.btManual.TabIndex = 3;
            this.btManual.Text = "Manual";
            this.btManual.Click += new System.EventHandler(this.btManual_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "Undo";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(7, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(226, 31);
            this.label6.Text = "[Ticket Number]";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pbBattery
            // 
            this.pbBattery.Location = new System.Drawing.Point(81, 18);
            this.pbBattery.Name = "pbBattery";
            this.pbBattery.Size = new System.Drawing.Size(156, 15);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label9.Location = new System.Drawing.Point(0, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 15);
            this.label9.Text = "Battery Level";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Fuchsia;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 15);
            this.label1.Text = "Conn: Unknown";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(139, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.Text = "%";
            // 
            // RemoteAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Moccasin;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pbBattery);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "RemoteAdmin";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Button btCheckPass;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ProgressBar pbBackupBattery;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar pbBattery;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbReporting;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbResponse;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar tbBacklight;
        private System.Windows.Forms.Button btConfirm;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblWWWServer;
        private System.Windows.Forms.Label lblGKServer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btManual;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label label6;
    }
}

