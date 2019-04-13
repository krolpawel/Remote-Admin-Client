using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Symbol.Fusion;
using Symbol.Fusion.WLAN;
using Symbol.Display;
using Symbol.StandardForms;
using Microsoft.WindowsMobile.Configuration;
using System.Media;

/*
 * do sprawdzenia: update db ze skanera
 * 
 * 
 * 
 */
namespace RemoteAdminPro
{
    public partial class RemoteAdmin : Form
    {
        //pola
        string servAdr = "http://10.10.30.200/"; //adres serwera WWW
        string pass = "1379"; //hasło domyślne do ustawień aplikacji, wykorzystywane kiedy aplikacja po uruchomieniu nie połączy się z serwerem
        int scanMode = 1; //domyślny tryb skanera biletów
        string deviceName = "";
        
        string pathSndValid = "\\Program Files\\RemoteAdminPro\\sndValid.wav";
        string pathSndValidOut = "\\Program Files\\RemoteAdminPro\\sndValidOut.wav";
        string pathSndNotValid = "\\Program Files\\RemoteAdminPro\\sndNotValid.wav";
        string pathSndNotFound = "\\Program Files\\RemoteAdminPro\\sndNotFound.wav";
        System.Media.SoundPlayer spValid;
        System.Media.SoundPlayer spValidOut;
        System.Media.SoundPlayer spNotValid;
        System.Media.SoundPlayer spNotFound;

        Battery c; //przechowuje informacje o stanie baterii
        //int counter = 0; //licznik zegara
        private bool lastConnectionStatus=false; //status ostatniego odwołania do serwera
        WebRequest req, req2; //zapytania do serwera
        //int tryPassCounter=3; // dla blokady admina. po 3 źle wpisanych hasłach następuje blokada
        string Parameters, Parameters2; // przechowywanie parametrów do odwołań do serwera
        System.IO.Stream os,os2; //odpowiedzialne za żądania webowe do serwera
        byte[] bytes,bytes2; //przechowują przesyłane z serwerem dane
        bool busy = false;  //flaga zajętości. zamraża aplikacje gdy skaner łączy się z siecią. zapobiega wyjątkom
        bool firstTick = true; // flaga pierwszej komunikacji z serwerem. po niej aplikacja się minimalizuje. nie da się wcześniej wywołać funkcji minimalizacji
        Symbol.Display.Controller DisplayCtl = null; // uchwyt do wyświetlacza
        
        //dla ikonki w TRAYu
        NotifyIcon notifyIcon; //obiekt ikonki w TRAY'u
        ContextMenu cMenu=null; // menu kontekstowe do ikonki w TRAY'u
        
        /*dla alarmu
        string pathAlert = "\\Program Files\\Gatekeeper\\sndNotValid.wav";
        System.Media.SoundPlayer sp;
        int alertCounter = 6;*/

        //dla skanera kodów
        BarcodeAPI bAPI = new BarcodeAPI(); // uchwyt do skanera kodów
        private EventHandler myReadNotifyHandler = null; // ?
        private EventHandler myStatusNotifyHandler = null; // ?
        //private bool isReaderInitialized = false;
        EventHandler myFormActivatedEventHandler; // uchwyt do zdarzenia aktywacji formatki
        EventHandler myFormDeactivatedEventHandler; // uchwyt do zdarzenia dezaktywacji formatki
        
        //dla Fusion
        WLAN myWlan = null; //uchwyt do Fusiona
        Adapter myAdapter = null; // uchwyt do aktywnego adaptera sieciowego
        Adapter.SignalQualityHandler mySignalQualityHandler = null; //uchwyt do zmiany jakości sygnału sieci
        WLAN.StatusChangeHandler myStatusChangedHandler = null; //uchwyt do zmiany statusu sieci
        string actualESSID = "";    //ssid sieci do której aktualnie jest podłączone urządzenie / wykorzystane do menu kontekstowego
        string actualSignalStrength = ""; //aktualna siła sygnału urządzenia / wykorzystane do menu kontekstowego

        //konstruktory
        public RemoteAdmin()
        {
            InitializeComponent();

            DisplayCtl = new StandardDisplay((Device)SelectDevice.Select(Controller.Title,Device.AvailableDevices)); // ustawienie uchwytu do wyświetlacza
            c = new Battery();
            InitializeNetwork();
            InitializeTimer(); 
            tabControl1.TabPages[1].Hide(); // zabezpieczenie karty z ustawieniami
            tabControl1.SelectedIndex = 2;
            comboBox1.SelectedIndex = 1; //ustawienie listy wyboru trybu skanowania na pozycję domyślną
            
            //Barcode Initializer
            BarcodeScannerInitializer();

            //Fusion
            FusionInitializer();
            CheckWiFiConnection();
            CreateTrayIcon();

            //Sounds
            spValid = new SoundPlayer(pathSndValid);
            spValidOut = new SoundPlayer(pathSndValidOut);
            spNotValid = new SoundPlayer(pathSndNotValid);
            spNotFound = new SoundPlayer(pathSndNotFound);
        }
        
        //initializery
        private void InitializeNetwork()
        {
            StringBuilder sb = null;
            StringWriter sw = null;
            XmlTextWriter xmlWriter = null;
            try
            {
              sb = new StringBuilder();
              sw = new StringWriter(sb);
              xmlWriter = new XmlTextWriter(sw);
              xmlWriter.WriteStartDocument();
              xmlWriter.WriteStartElement("wap-provisioningdoc");
              xmlWriter.WriteStartElement("characteristic");
              xmlWriter.WriteStartAttribute("type");
              xmlWriter.WriteString("CM_Networks");
              xmlWriter.WriteEndAttribute();
              xmlWriter.WriteStartElement("characteristic");
              xmlWriter.WriteStartAttribute("type");
              xmlWriter.WriteString("My Work Network");
              xmlWriter.WriteEndAttribute(); //type
              xmlWriter.WriteStartElement("parm");
              xmlWriter.WriteStartAttribute("name");
              xmlWriter.WriteString("DestId");
              xmlWriter.WriteEndAttribute(); //name
              xmlWriter.WriteStartAttribute("value");
              xmlWriter.WriteString("{8b06c75c-d628-4b58-8fcd-43af276755fc}");
              xmlWriter.WriteEndAttribute();
              xmlWriter.WriteEndElement(); //parm
              xmlWriter.WriteEndElement(); //characteristic
              xmlWriter.WriteEndElement(); //characteristic
              xmlWriter.WriteEndElement(); //wap-provisioningdoc
              xmlWriter.WriteEndDocument();
              var xmlDoc = new XmlDocument();
              xmlDoc.LoadXml(sb.ToString());
              ConfigurationManager.ProcessConfiguration(xmlDoc, false);
            }
            catch (Exception)
            {
               //Do stuff...
               throw;
            }
            finally
            {
               if (xmlWriter != null)
               {
                   xmlWriter.Flush();
                   xmlWriter.Close();
               }
            }
        }
        private void InitializeTimer()
        {
            timer1.Interval = 2000;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1_Tick);
            cbReporting.Checked = true;
        }
        private void BarcodeScannerInitializer()
        {
            bAPI.InitReader();
            //bAPI.Reader.Parameters.Feedback.Success.WaveFile = @"\\\Program Files\RemoteAdminPro\scan.wav";
            bAPI.Reader.Parameters.Feedback.Success.BeepTime = 0;
            bAPI.Reader.Decoders.I2OF5.Enabled = true;
            bAPI.Reader.Decoders.I2OF5.MaximumLength = 24; //maksymalna długość skanowanych kodów
            bAPI.Reader.Decoders.I2OF5.MinimumLength = 10; //minimalna długość skanowanych kodów
            bAPI.StartRead(false);
            //this.myStatusNotifyHandler = new EventHandler(myReader_StatusNotify);
            bAPI.AttachStatusNotify(myStatusNotifyHandler);
            myReadNotifyHandler = new EventHandler(myReader_ReadNotify);
            bAPI.AttachReadNotify(myReadNotifyHandler);
            myFormActivatedEventHandler = new EventHandler(MainForm_Activated);
            myFormDeactivatedEventHandler = new EventHandler(MainForm_Deactivate);
            this.Activated += myFormActivatedEventHandler;
            this.Deactivate += myFormDeactivatedEventHandler;
        }
        private void FusionInitializer()
        {
            try
            {
                myWlan = new WLAN(FusionAccessType.COMMAND_MODE);
            }
            catch
            {
                MessageBox.Show("Nie można otworzyć Fusion. Program nie może funkcjonować!");
                CloseApp();
            }
            try
            {
                myAdapter = myWlan.Adapters[0];
            }
            catch
            {
                MessageBox.Show("Błąd adaptera sieciowego. Program nie może funkcjonować!");
                CloseApp(); //zamknięcie programu
            }
            mySignalQualityHandler = new Adapter.SignalQualityHandler(myAdapter_SignalQualityChanged);
            myStatusChangedHandler = new WLAN.StatusChangeHandler(myWlan_StatusChanged);
            myAdapter.SignalQualityChanged += mySignalQualityHandler;
            myWlan.StatusChanged += myStatusChangedHandler;
        }

        //listenery i eventy
        public void tabControl1_SelectedIndexChanged(object sender, EventArgs e) //ukrycie karty ustawień gdy użytkownik ręcznie zmieni kartę
        {
            tabControl1.TabPages[1].Hide();
            tabControl1.TabPages[1].Text = "Inactive";
            if (tabControl1.SelectedIndex == 0)
                tbPass.Focus();
        }
        private void myReader_ReadNotify(object Sender, EventArgs e) //rozpoznawanie statusu odpowiedzi po zeskanowaniu
        {
            // Checks if the Invoke method is required because the ReadNotify delegate is called by a different thread
            if (this.InvokeRequired)
            {
                // Executes the ReadNotify delegate on the main thread
                this.Invoke(myReadNotifyHandler, new object[] { Sender, e });
            }
            else
            {
                // Get ReaderData
                Symbol.Barcode.ReaderData TheReaderData = this.bAPI.Reader.GetNextReaderData();
                switch (TheReaderData.Result)
                {
                    case Symbol.Results.SUCCESS:
                        if (tabControl1.SelectedIndex == 2)
                        {
                            label6.Text = TheReaderData.Text;
                            CheckTicket(TheReaderData.Text);
                        }
                        else if (tabControl1.SelectedIndex == 0 || tabControl1.SelectedIndex == 1)
                            CheckBarcodeConfig(TheReaderData.Text);
                        bAPI.StartRead(false);
                        break;
                    case Symbol.Results.E_SCN_READTIMEOUT:
                        this.bAPI.StartRead(false);
                        break;
                    case Symbol.Results.CANCELED:
                        break;
                    case Symbol.Results.E_SCN_DEVICEFAILURE:
                        this.bAPI.StopRead();
                        this.bAPI.StartRead(false);
                        break;
                    default:
                        string sMsg = "Read Failed\n"
                            + "Result = "
                            + (TheReaderData.Result).ToString();

                        if (TheReaderData.Result == Symbol.Results.E_SCN_READINCOMPATIBLE)
                        {
                            MessageBox.Show("Fail!");
                            this.Close();
                            return;
                        }
                        break;
                }
            }
        }
        private void MainForm_Activated(object sender, EventArgs e) //kiedy formatka jest aktywowana czyli jest na pierwszym planie
        {
            this.bAPI.isBackground = false;
            this.bAPI.StartRead(false);
            this.tbPass.Focus();
        }
        private void MainForm_Deactivate(object sender, EventArgs e) // kiedy formatka znika z pierwszego planu
        {
            bAPI.StopRead();
            this.bAPI.isBackground = true;
            tabControl1.SelectedIndex = 3;

        }
        private void cbReporting_CheckStateChanged(object sender, EventArgs e) //zarządzanie raportowaniem na podstawie checkboxa
        {
            timer1.Enabled = cbReporting.Checked == true ? true : false;
        }
        private void tbPass_TextChanged(object sender, EventArgs e) // pokazywanie przycisku logowania lokalnego gdy w polu hasła >3 znaki
        {
            if (tbPass.Text.Length > 3)
                btCheckPass.Visible = true;
            else
                btCheckPass.Visible = false;
        }
        private void tbBacklight_ValueChanged(object sender, EventArgs e) // ustawienie jasności przy masnipulacji trackbarem
        {
            DisplayCtl.BacklightIntensityLevel = tbBacklight.Value;
        }
        void myAdapter_SignalQualityChanged(object sender, StatusChangeArgs e) //pobranie informacji o aktualnej sieci w momencie zmiany siły sygnału
        {
            actualESSID = e.Adapter.ESSID.ToString();
            actualSignalStrength = e.SignalStrength.ToString() + " dBm, " + e.SignalQuality.ToString();
        }
        void myWlan_StatusChanged(object sender, StatusChangeEventArgs e) //zmiana statusu sieci
        {
            if (e.StatusChange == Symbol.Fusion.WLAN.StatusChangeEventArgs.StatusChanges.AdapterPowerOFF)
            {
                actualESSID = "WiFi OFF";
            }
            else if (e.StatusChange == Symbol.Fusion.WLAN.StatusChangeEventArgs.StatusChanges.AccesspointChanged)
            {
                //populate accesspoint MAC address (BSSID) in the list
                Symbol.Fusion.WLAN.StatusChangeEventArgs.APChangedEventData APData = (Symbol.Fusion.WLAN.StatusChangeEventArgs.APChangedEventData)(e.StatusChangeData);

                if (((APData.BSSID == "") || (APData.BSSID == null)))
                {
                    actualESSID = "Disconnected";
                }
                else
                {
                    actualESSID = APData.BSSID;
                }
            }
            else
            {
                actualESSID = "ERR";
            }
        }
        private void tbPass_KeyDown(object sender, KeyEventArgs e) //obsługa zatwierdzania enterem pola z hasłem
        {
            if (e.KeyCode == Keys.Enter && btCheckPass.Visible == true)
                btCheckPass_Click(sender, e);
        }
        private void notifyIcon_Click(object sender, EventArgs e) //kliknięcie ikonki w trayu
        {
            Point poi = new Point(30, 0); //poziom, pion
            cMenu.Show(label7, poi); //dlaczego musi być do kontrolki??? musi być do kontrolki na pierwszej karcie (albo na aktywnej)
        }
        private void SettingsFromTray_Click(object sender, EventArgs e)//kliknięcie pozycji w menu kontekstowym ikonki w trayu
        {
            if (busy == false)
            {
                this.Show();
                bAPI.StartRead(false);
                this.tbPass.Focus();
            }
        }
        private void btCheckPass_Click(object sender, EventArgs e) //logowanie do panelu
        {
            if (tbPass.Text == pass)
            {
                //tryPassCounter = 3;
                tabControl1.SelectedIndex = 1;
                tabControl1.TabPages[1].Text = "Settings";
                tabControl1.TabPages[1].Show();
            }
            else
            {
                //tryPassCounter--;
                //MessageBox.Show("ŹLE! Pozostały " + tryPassCounter + " próby! ");
            }
            tbPass.Text = "";
            /*if (tryPassCounter == 0)
            {
                alertTimer.Enabled = true;
                tbPass.Enabled = false;
                btCheckPass.Enabled = false;
                label7.Text="BLOKADA! Administrator poinformowany!";
                this.BackColor = System.Drawing.Color.Red;
                label7.ForeColor = System.Drawing.Color.White;
            }*/
        }
        //zmiana trybu skanowania
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Minimalizacja przy pierwszym kliku zegara. nie da sie wcześniej, bo konstruktor RA dopiero PO wykonaniu swoich instrukcji pokazuje okno.
            if (firstTick == true)
            {
                //Minimize(null,null);
                firstTick = false;
                deviceName = GetDeviceName();
            }
            
            //wstępne uzupełnianie pól w widoku
            //counter++;
            //string name = System.Net.Dns.GetHostName(); // pobieranie nazwy hosta. ta nazwa jest wykorzystywana jako nazwa urządzenia
            string name = deviceName;
            int bright = DisplayCtl.BacklightIntensityLevel; // jasność ekranu
            if (tabControl1.SelectedIndex != 1) //jeśli nie jesteśmy obecnie na karcie ustawień updateujemy kontrolki na karcie ustawień
            {
                pbBattery.Value = c.GetBS(); //procent baterii
                label2.Text = pbBattery.Value.ToString()+"%";
                pbBackupBattery.Value = c.GetBS2(); //procent baterii backupowej
                tbBacklight.Value = bright; //trackbar jasności
                tbResponse.Text = (timer1.Interval / 1000).ToString(); //czas odpowiedzi skanera
            }
            string gkAdr = GetGkAdr();
            if (gkAdr == "0.0.0.0")
                return;
            lblGKServer.Text = "GK Server: "+gkAdr;
            lblWWWServer.Text = "App Server: "+servAdr.Substring(0,servAdr.Length-1).Substring(7);
            //przygotowanie żądania do kolektora danych
            Parameters = "device=" + name + "&battLvl=" + c.GetBS() + "&acStatus=" + c.GetACStatus()
                + "&bright=" + bright + "&gkSrvAdr=" + gkAdr + "&IP=" + GetIPAddress() + "&timer=" + (timer1.Interval / 1000)
                +"&localPass="+pass+"&scanMode="+scanMode.ToString();
            req = WebRequest.Create(servAdr+"RA/collector.php");
            req.Timeout = 1000;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.Proxy = new System.Net.WebProxy(servAdr+"RA/collector.php", true); //bez proxy
            bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            //wysłanie requesta
            try
            {
                os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //wysłanie
                os.Close();

                //odbieranie odpowiedzi
                WebResponse resp = req.GetResponse();
                if (resp != null)
                {
                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    string str = sr.ReadToEnd().Trim();
                    //jeśli dostaję odpowiedź XML to wykonuję update
                    if (str.Contains("<?xml"))
                    {
                        UpdateConfig(str);
                    }
                }
                lastConnectionStatus = true;
                label1.Text = "Conn: OK";
            }
            catch
            {
                lastConnectionStatus = false;
                label1.Text = "Conn: FAILED";
            }
            finally
            {
                req.Abort();
            }
            //ustawianie wartości w menu kontekstowym dla ikonki w trayu
            if (cMenu != null)
            {
                if (lastConnectionStatus == true)
                    cMenu.MenuItems[0].Text = "RA Connection: OK";
                else
                    cMenu.MenuItems[0].Text = "RA Connection: FALSE!";
                cMenu.MenuItems[1].Text = "Battery: " + c.GetBS()+"%";
                cMenu.MenuItems[2].Text = "WiFi: " + actualESSID;
                cMenu.MenuItems[3].Text = actualSignalStrength;
            }          
        }
        
        //przyciski
        private void btCancel_Click(object sender, EventArgs e) //przycisk ANULUJ
        {
            DialogResult dr = MessageBox.Show("Chcesz anulować zmiany?", "Potwierdź anulację", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                tabControl1.SelectedIndex = 0;
            }
        }
        private void btClose_Click(object sender, EventArgs e)//przycisk ZAMKNIJ
        {
            DialogResult dr = MessageBox.Show("Chcesz wyjść z programu?", "Potwierdź wyjście", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                CloseApp();
            }
        }
        private void btConfirm_Click(object sender, EventArgs e)//przycisk ZATWIERDŹ
        {
            if (lastConnectionStatus == true)
            {
                if (Validator())
                {
                    string address = servAdr + "RA/devListener.php";
                    Parameters2 = "device=" + System.Net.Dns.GetHostName() + "&bright=" + tbBacklight.Value + "&timer=" 
                        + tbResponse.Text + "&admin=tpAdmin&scanMode=" + comboBox1.SelectedIndex;
                    timer1.Interval = Convert.ToInt32(tbResponse.Text);
                    req2 = WebRequest.Create(address);
                    req2.ContentType = "application/x-www-form-urlencoded";
                    req2.Method = "POST";
                    req2.Proxy = new System.Net.WebProxy(address, true); //bez proxy
                    bytes2 = System.Text.Encoding.ASCII.GetBytes(Parameters2);
                    req2.ContentLength = bytes2.Length;
                    //wysłanie requesta
                    try
                    {
                        os2 = req2.GetRequestStream();
                        os2.Write(bytes2, 0, bytes2.Length);
                        os2.Close();

                        //odbieranie odpowiedzi
                        WebResponse resp2 = req2.GetResponse();
                        if (resp2 != null)
                        {
                            StreamReader sr2 = new StreamReader(resp2.GetResponseStream());
                            string str2 = sr2.ReadToEnd().Trim();
                            if (str2 == "OK-adm")
                            {
                                label17.Text = ScanModePresenter(comboBox1.SelectedIndex);
                                tabControl1.SelectedIndex = 2;
                                MessageBox.Show("Zmieniono!");
                            }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                    req2.Abort();
                }
                else
                    MessageBox.Show("Pola wypełnione niepoprawnie");
            }
            else
                MessageBox.Show("Nie można wprowadzić zmian kiedy skaner nie ma połączenia z serwerem");
        }
        
        //ogólne
        private void CloseApp()//Zamykanie aplikacji 
        {
            this.myStatusNotifyHandler = null;
            myReadNotifyHandler = null;
            this.Activated -= myFormActivatedEventHandler;
            this.Deactivate -= myFormDeactivatedEventHandler;
            bAPI.StopRead(); //zatrzymanie readera
            bAPI.TermReader(); //uśmiercenie readera
            bAPI = null;
            this.Close();
        }
        private bool IpValidator(string str)
        {
            Regex regex = new Regex("((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?).){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
            Match match = regex.Match(str);
            if (match.Success)
                return true;
            return false;
        }

        //Skaner kodów
        private void CheckBarcodeConfig(string code) //przesłanie kodu w celu updateu konfiguracji
        {
            //przygotowanie requesta do wysłania 
            string name = System.Net.Dns.GetHostName(); //w celu identyfikacji pytającego na serwerze
            string param = "action=configBarcodeRequest&code=" + code+"&name="+name;
            WebRequest request = WebRequest.Create(servAdr + "RA/devListener.php");
            request.Timeout = 1000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Proxy = new System.Net.WebProxy(servAdr+"RA/devListener.php", true); //bez proxy
            byte[] bytesCnf = System.Text.Encoding.ASCII.GetBytes(param);
            request.ContentLength = bytesCnf.Length;
            //wysłanie requesta
            try
            {
                System.IO.Stream osCnf = request.GetRequestStream();
                osCnf.Write(bytesCnf, 0, bytesCnf.Length);
                osCnf.Close();

                //odbieranie odpowiedzi
                WebResponse response = request.GetResponse();
                if (response != null)
                {
                    StreamReader srCnf = new StreamReader(response.GetResponseStream());
                    string str2 = srCnf.ReadToEnd().Trim();
                    //label5.Text = str2;
                    //jeśli dostanę odpowiedź XML to wykonuję update
                    if (str2.Contains("<?xml"))
                    {
                        if (str2.Contains("Configuration"))
                        {
                            label1.Text = "Applied config from barcode";
                            UpdateConfig(str2);
                        }
                        //jeśli otrzymam profile sieciowe do wprowadzenia
                        else if (str2.Contains("ArrayOfProfile"))
                        {
                            List<Profile> profList = null;
                            profList = LoadProfileFromXml(str2);
                            if (profList != null)
                            {
                                for (int i = 0; i < profList.Count; i++)
                                    if (MakeProfileFromClass(profList[i]))
                                        MessageBox.Show("Profil wprowadzony poprawnie");

                            }
                        }
                        //jeśli otrzymam nazwę profilu sieciowego do aktywacji
                        else if (str2.Contains("ActiveProfile"))
                        {
                            bool found = false;
                            string profileName = str2.Substring(str2.IndexOf("<ProfileName>") + 13);
                            profileName = profileName.Substring(0, profileName.IndexOf("</ProfileName>"));
                            if(ActivateProfile(profileName))
                                MessageBox.Show("Profil " + profileName + " został aktywowany");
                            else
                                MessageBox.Show("Nie znaleziono profilu " + profileName + " !");
                        }
                        else
                            MessageBox.Show("Nieznana odpowiedź XML");
                    }
                    else
                        MessageBox.Show("Prawdopodobnie nie znaleziono kodu w bazie");
                }
            }
            catch
            {
                lastConnectionStatus = false;
            }
            request.Abort();
        }
        public void CheckTicket(string code)
        {
            if(scanMode==0) //tryb STOP
                return;
            ShowCheckTicketEffect("Wait");
            label13.Text = "";
            label14.Text = "";
            label15.Text = "";
            //przygotowanie requesta do wysłania 
            string name = System.Net.Dns.GetHostName(); //w celu identyfikacji pytającego na serwerze
            string param = "action=checkTicket&code=" + code + "&name=" + name;
            WebRequest request = WebRequest.Create(servAdr + "RA/devListener.php");
            request.Timeout = 1000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Proxy = new System.Net.WebProxy(servAdr + "RA/devListener.php", true); //bez proxy
            byte[] bytesCnf = System.Text.Encoding.ASCII.GetBytes(param);
            request.ContentLength = bytesCnf.Length;
            //wysłanie requesta
            try
            {
                System.IO.Stream osTicket = request.GetRequestStream();
                osTicket.Write(bytesCnf, 0, bytesCnf.Length);
                osTicket.Close();

                //odbieranie odpowiedzi
                WebResponse response = request.GetResponse();
                if (response != null)
                {
                    StreamReader srTicket = new StreamReader(response.GetResponseStream());
                    string str2 = srTicket.ReadToEnd().Trim();
                    label5.Text = str2;
                    //jeśli dostanę odpowiedź XML to wykonuję update
                    if (str2.Contains("<?xml"))
                    {
                        if (str2.Contains("CheckTicket"))
                        {
                            if (str2.Contains("ValidIn"))
                                ShowCheckTicketEffect("ValidIn");
                            else if (str2.Contains("NotValid"))
                            {
                                ShowCheckTicketEffect("NotValid");
                                label13.Text = "Time: " + str2.Substring(str2.IndexOf("<Time>") + 6, str2.IndexOf("</Time>") - str2.IndexOf("<Time>")-6);
                                label15.Text = str2.Substring(str2.IndexOf("<Time2>") + 7, str2.IndexOf("</Time2>") - str2.IndexOf("<Time2>") - 7);
                                label14.Text = "Device: " + str2.Substring(str2.IndexOf("<Device>") + 8, str2.IndexOf("</Device>") - str2.IndexOf("<Device>") - 8);
                                
                            }
                            else if (str2.Contains("ValidOut"))
                                ShowCheckTicketEffect("ValidOut");
                            else if (str2.Contains("NotFound"))
                                ShowCheckTicketEffect("NotFound");
                            else if (str2.Contains("error"))
                                ShowCheckTicketEffect("DBerror");
                            else if (str2.Contains("notInDB"))
                                ShowCheckTicketEffect("notInDB");
                            else if (str2.Contains("modeStop"))
                                ShowCheckTicketEffect("modeStop");
                            else
                                MessageBox.Show("Nieznana odpowiedź XML (bilet)");
                        }
                    }
                    else
                        MessageBox.Show("Prawdopodobnie nie znaleziono kodu biletu w bazie");
                }
            }
            catch
            {
                lastConnectionStatus = false;
            }
            request.Abort();
        }
        public void ShowCheckTicketEffect(string effect)
        {
            if (effect == "Wait")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.Yellow;
                panel1.BackColor = System.Drawing.Color.Yellow;
                label5.Text = "Wait...";
            }
            else if (effect == "ValidIn")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.LightGreen;
                panel1.BackColor = System.Drawing.Color.LightGreen;
                label5.Text = "VALID";
                spValid.Play();
            }
            else if (effect == "ValidOut")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.LightBlue;
                panel1.BackColor = System.Drawing.Color.LightBlue;
                label5.Text = "VALID OUT";
                spValidOut.Play();
            }
            else if (effect == "NotValid")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.LightCoral;
                panel1.BackColor = System.Drawing.Color.LightCoral;
                label5.Text = "NOT VALID!";
                spNotValid.Play();
            }
            else if (effect == "NotFound")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.LightCoral;
                panel1.BackColor = System.Drawing.Color.LightCoral;
                label5.Text = "NOT FOUND";
                spNotFound.Play();
            }
            else if (effect == "DBerror")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.Orange;
                panel1.BackColor = System.Drawing.Color.Orange;
                label5.Text = "DATABASE ERROR";
            }
            else if (effect == "notInDB")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.Orange;
                panel1.BackColor = System.Drawing.Color.Orange;
                label5.Text = "DEVICE ISN'T IN DB";
            }
            else if (effect == "modeStop")
            {
                tabControl1.TabPages[2].BackColor = System.Drawing.Color.Orange;
                panel1.BackColor = System.Drawing.Color.Orange;
                label5.Text = "MODE STOP";
            }
        }
        public string ScanModePresenter(int mode)
        {
            switch (mode)
            {
                case 0: return "STOP"; break;
                case 1: return "ENTRY"; break;
                case 2: return "EXIT"; break;
                case 3: return "BIDIRECTIONAL"; break;
                default: return "ERR"; break;
            }
        }
        //dla barcode scannera - chyba niepotrzebne. na razie zostaje na później (20150810)
        /*private void myReader_StatusNotify(object Sender, EventArgs e)
        {
            // Checks if the Invoke method is required because the StatusNotify delegate is called by a different thread
            if (this.InvokeRequired)
            {
                // Executes the StatusNotify delegate on the main thread
                this.Invoke(myStatusNotifyHandler, new object[] { Sender, e });
            }
            else
            {
                // Get ReaderData
                Symbol.Barcode.BarcodeStatus TheStatusData = this.bAPI.Reader.GetNextStatus();

                switch (TheStatusData.State)
                {
                    case Symbol.Barcode.States.WAITING:
                        break;
                }
            }
        }*/

        //konfiguracja ogólna
        private void UpdateConfig(string str)
        {
            timer1.Enabled = false;
            Configuration cnf = null;
            //tworzenie klasy z konfiguracją na podstawie otrzymanego XMLa
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                using (TextReader reader = new StringReader(str))
                {
                    //label5.Text = str;
                    cnf = (Configuration)serializer.Deserialize(reader);
                }
            }
            catch
            {
                MessageBox.Show("Błędne wywołanie.\nNie można stworzyć klasy na podstawie otrzymanego XMLa");
            }
            if (tabControl1.SelectedIndex != 1) //wprowadzanie zmian jeśli znajdujemy się na karcie głównej
            {
                //Fix scan mode
                if (cnf.scanMode != scanMode)
                {
                    scanMode = cnf.scanMode;
                    comboBox1.SelectedIndex = scanMode;
                    label17.Text = ScanModePresenter(scanMode);
                }
                //Fix brightness
                if(cnf.brightness!=" " && cnf.brightness!="" && cnf.brightness!="-1") //odfiltrowanie wartości oznaczających brak zmiany
                    DisplayCtl.BacklightIntensityLevel = Convert.ToInt32(cnf.brightness);

                //Fix Gatekeeper IP Address
                if (cnf.gk != " " && cnf.gk != "") //odfiltrowanie wartości oznaczających brak zmiany
                {
                    string gkSkn = GetGkAdr();
                    if (gkSkn != cnf.gk) //sprawdzenie czy adres różni się od obecnie wprowadzonego, aby nie wykonywać niepotrzebnych operacji
                    {
                        SetGkAdr(cnf.gk);
                    }
                }

                //Fix timer interval
                if(cnf.timerInterval>0) 
                    timer1.Interval = cnf.timerInterval * 1000;

                //Fix local admin password
                if(cnf.lPass!=" " && cnf.lPass !="")
                    pass = cnf.lPass;

                //Optional show message
                if (cnf.message != "" && cnf.message != " ")
                {
                    DialogResult dr = MessageBox.Show(cnf.message, "Informacja od administratora", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    if (dr == DialogResult.OK)
                    {
                        //wysłanie potwierdzenia odebrania wiadomości
                        string param = "device=" + System.Net.Dns.GetHostName() + "&message=received";
                        WebRequest reqX = WebRequest.Create(servAdr+"RA/devListener.php");
                        reqX.Timeout = 1000;
                        reqX.ContentType = "application/x-www-form-urlencoded";
                        reqX.Method = "POST";
                        reqX.Proxy = new System.Net.WebProxy(servAdr+"RA/devListener.php", true); //bez proxy
                        byte[] bytesX = System.Text.Encoding.ASCII.GetBytes(param);
                        reqX.ContentLength = bytesX.Length;
                        try
                        {
                            Stream osX = reqX.GetRequestStream();
                            osX.Write(bytesX, 0, bytesX.Length);
                            osX.Close();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                        reqX.Abort();
                        cnf.message = "";
                        Minimize(null,null);
                    }
                }
            }
            cnf = null;
            timer1.Enabled = true;
        }
        public static string GetIPAddress()
        {
            //zazwyczaj skaner będize miał nadane 2 numery IP. lokalny oraz nadany przez sieć/zdefiniowany przez profil
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string str = "";
            foreach (IPAddress IP in host.AddressList)
                if (!IP.ToString().Contains("169.254")) //stały fragment lokalnego adresu IP
                    str = IP.ToString();
            return str;
        }
        private int SetGkAdr(string newAdr) // podmiana pliku konfiguracyjnego gatekeepera
        {
            string path = "\\Program Files\\Gatekeeper\\GatekeeperPDA.config";
            string allXML = "";
            
            //odczyt całego configu
            if (File.Exists(path))
            {
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    allXML = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    string oldIP = GetGkAdr();
                    if(File.Exists(path))
                        File.Delete(path);
                    allXML = allXML.Replace(oldIP, newAdr);
                    Thread.Sleep(200);
                    //zapis nowego configu
                    FileStream fs2 = new FileStream(path, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs2, Encoding.UTF8);
                    sw.Write(allXML);
                    sw.Close();
                    fs2.Close();
                    RestartGK();
                }
                catch
                {
                    MessageBox.Show("UWAGA!\n\n Zmiana adresu nie powiodła się.\n Zweryfikuj plik konfiguracyjny GK.\n\nIstnieje niebezpieczeństwo, że plik nie istnieje!", "Operacja nieudana!", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                }
            }
            return 0;
        }
        private void RestartGK() //restart aplikacji Gatekeeper (jeśli jest uruchomiona) w celu wprowadzenia nowego configu
        {
            this.Show();
            Thread.Sleep(2000);
            if (ProcessCE.IsRunning(@"\Program Files\Gatekeeper\Gatekeeper.exe"))
            {
                ProcessInfo[] list = ProcessCE.GetProcesses();
                foreach (ProcessInfo item in list)
                {
                    if (item.FullPath == @"\Program Files\Gatekeeper\Gatekeeper.exe")
                        item.Kill();
                }
                Process.Start(@"\Program Files\Gatekeeper\Gatekeeper.exe", "");
            }
        }
        private bool GkConfAvailable()
        {
            return true;
            FileInfo file = new FileInfo(@"\\Program Files\\Gatekeeper\\GatekeeperPDA.config");
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return true;
        }
        private string GetGkAdr() //pobieranie adresu IP serwera GK z pliku konfiguracyjnego
        {
            if (GkConfAvailable())
            {
                XmlTextReader reader = null;
                string IP = "";
                try
                {
                    string path = System.IO.Path.GetDirectoryName(@"\\\Program Files\Gatekeeper\");
                    reader = new XmlTextReader("\\Program Files\\Gatekeeper\\GatekeeperPDA.config");
                    while (reader.Read())
                    {
                        if (reader.Name == "config")
                        {
                            IP = reader["IPAddress"];
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Nie można uzyskać dostępu do pliku konfiguracyjnego GK");
                    return "0.0.0.0";
                }
                reader.Close();
                return IP;
            }
            return "0.0.0.0";
        }
        private bool Validator() //sprawdza wartości przy zatwierdzaniu ustawień wykonanych lokalnie
        {
            try
            {
                int resp = Convert.ToInt32(tbResponse.Text);
                if (resp > 0 && resp < 3600)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        private string GetDeviceName()
        {
            XmlTextReader reader = null;
            string name = "";
            try
            {
                string path = System.IO.Path.GetDirectoryName(@"\\\Program Files\Gatekeeper\");
                reader = new XmlTextReader("\\Program Files\\Gatekeeper\\GatekeeperPDA.config");
                while (reader.Read())
                {
                    if (reader.Name == "config")
                    {
                        name = reader["Name"];
                        if(name!="")
                            return name;
                    }
                }
                
            }
            catch
            {
                return System.Net.Dns.GetHostName();
            }
            reader.Close();
            return System.Net.Dns.GetHostName();
        }


        //Profile sieciowe
        private bool CheckWiFiConnection()
        {
            //włączenie modułu WiFi jesli nie jest włączony
            if (myWlan.Adapters[0].PowerState == Symbol.Fusion.WLAN.Adapter.PowerStates.OFF)
                myWlan.Adapters[0].PowerState = Symbol.Fusion.WLAN.Adapter.PowerStates.ON;
            Profiles profiles = myWlan.Profiles;
            //bool connectionStatus = false;
            string basicProfile = ""; //przechowuje nazwę profilu podstawowego jeśli zostanie znaleziony
            string connectToBasic = ""; //przypisanie nazwy podstawowego profilu sieciowego jeśli urządzenie jest do niego podłączone
            //sprawdzenie czy skaner jest podłączony do którejś sieci
            try
            {
                for (int i = 0; i < profiles.Length; i++)
                {
                    if (profiles[i].Enabled == true && myWlan.Adapters[0].ActiveProfile.Name == profiles[i].Name)
                    {
                        if (myWlan.Adapters[0].ConnectionStatus == Symbol.Fusion.FusionResults.SUCCESS)
                        {
                            //connectionStatus = true;
                            if (profiles[i].Name == "Ticketpro" || profiles[i].Name == "TicketproU")
                                connectToBasic = profiles[i].Name;
                        }
                    }
                    if (profiles[i].Name == "Ticketpro" || profiles[i].Name == "TicketproU")
                        basicProfile = profiles[i].Name;
                }
            }
            catch
            {
                MessageBox.Show("Nie mozna uzyskac dostępu do adaptera sieciowego! Aplikacja nie może funkcjonować");
                basicProfile = "ERR";
                CloseApp();
            }
            //gdy nie ma profilu podstawowego
            if (basicProfile == "")
            {
                DialogResult dr = MessageBox.Show("Nie wykryto profilu podstawowego (Ticketpro lub TicketproU). Czy chcesz załadować z pliku konfiguracyjnego?", "Brak profilu podstawowego", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                bool profileLoaded = false;
                if (dr == DialogResult.Yes) //ładowanie profilu z pliku konfiguracyjnego
                    profileLoaded = LoadProfileFromConfig();
                DialogResult dr2 = DialogResult.Abort;
                if (dr == DialogResult.No)
                    dr2 = MessageBox.Show("Czy chcesz załadować domyślny?", "Brak profilu podstawowego", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                else if (profileLoaded == false)
                    dr2 = MessageBox.Show("Nie udało się załadować profilu z pliku!\nCzy chcesz załadować domyślny?", "Brak profilu podstawowego", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr2 == DialogResult.Yes) //ładowanie profilu domyślnego ustawionego w tym programie
                    profileLoaded = LoadDefaultProfile();
                else if (dr2 == DialogResult.No)
                    return false;
                //jeśli załadowano profil
                if (profileLoaded)
                {
                    DialogResult dr3 = MessageBox.Show("Załadowano. Połączyć?", "Zaladowano profil", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (dr3 == DialogResult.Yes)//Aktywacja załadowanego profilu
                    {
                        bool baseProfileActivated = ActivateProfile("Ticketpro");
                        if (baseProfileActivated == false)
                            baseProfileActivated = ActivateProfile("TicketproU");
                        if (baseProfileActivated == true)
                        {
                            MessageBox.Show("Profil podstawowy został aktywowany");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się aktywować profilu podstawowego!");
                            return false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nic z tego! Nie ma więcej opcji. Zaprogramuj ręcznie.");
                    Minimize(null, null);
                }
                return false;
            }
            return false;
        }
        private List<Profile> LoadProfileFromXml(string str) //ładowanie listy profili na podstawie XMLa otrzymanego z serwera
        {
            List<Profile> profileList = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Profile>));
                using (TextReader reader = new StringReader(str))
                {
                    profileList = (List<Profile>)serializer.Deserialize(reader);
                }
                return profileList;
            }
            catch {
                MessageBox.Show("Błędna lub niekompletna odpowiedź serwera.\nNie można załadować profili.");
                return null;
            }
        }
        private bool LoadProfileFromConfig()//ładowanie domyślnego profilu sieciowego z pliku konfiguracyjnego
        {
            List<Profile> profileList = null;
            try
            {
                //config musi znajdować się w katalogu głównym
                string path = "RAconfig.xml";

                XmlSerializer serializer = new XmlSerializer(typeof(List<Profile>));
                
                StreamReader reader = new StreamReader(path);
                profileList = (List<Profile>)serializer.Deserialize(reader);
                reader.Close();
                for (int i = 0; i < profileList.Count; i++)
                {
                    MakeProfileFromClass(profileList[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się załadować profilu z klasy.\n"+ex.ToString());
                return false;
            }
            return true;
        }
        private bool MakeProfileFromClass(Profile p) //tworzenie profilu na podstawie klasy "Profile"
        {
            string errorMessage = "";
            if (p.Name != "" && p.Ssid != "" && p.AddressingMode != "" && p.AESAllowMixedMode != "" && p.AllowMixedMode != "" && p.AuthenticationType != ""
                && p.EncryptionType != "" && p.PowerMode != "" && p.SecurityType != "" && p.SubnetMask != "")
            {
                //sprawdzenie czy profil już istnieje
                for (int i = 0; i < myWlan.Profiles.Length; i++)
                {
                    if (myWlan.Profiles[i].Name == p.Name)
                        errorMessage = "Profil o nazwie " + p.Name + " już istnieje. Nowy profil nie zostanie wprowadzony.";
                }

                InfrastructureProfileData profile = new InfrastructureProfileData(p.Name,p.Ssid);
                switch (p.SecurityType)
                {
                    case "LEGACY":
                        profile.SecurityType = WLANSecurityType.SECURITY_LEGACY;
                        break;
                    case "WAPI":
                        profile.SecurityType = WLANSecurityType.SECURITY_WAPI;
                        break;
                    case "WPA_PERSONAL":
                        profile.SecurityType = WLANSecurityType.SECURITY_WPA_PERSONAL;
                        break;
                    case "WPA_ENTERPRISE":
                        profile.SecurityType = WLANSecurityType.SECURITY_WPA_ENTERPRISE;
                        break;
                    case "WPA2_PERSONAL":
                        profile.SecurityType = WLANSecurityType.SECURITY_WPA2_PERSONAL;
                        break;
                    case "WPA2_ENTERPRISE":
                        profile.SecurityType = WLANSecurityType.SECURITY_WPA2_ENTERPRISE;
                        break;
                    default:
                        errorMessage = "Nie odczytano poprawnie typu zabezpieczeń sieci";
                        break;
                }
                switch (p.AuthenticationType)
                {
                    case "NONE":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.NONE;
                        break;
                    case "EAP-TLS":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TLS;
                        break;
                    case "EAP-FAST_MS_CHAP_V2":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_FAST_MSCHAPV2;
                        break;
                    case "EAP-FAST_TLS":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_FAST_TLS;
                        break;
                    case "EAP-FAST_GTC":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_FAST_GTC;
                        break;
                    case "PEAP_TLS":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.PEAP_TLS;
                        break;
                    case "PEAP_GTC":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.PEAP_GTC;
                        break;
                    case "PEAP_MS_CHAP_V2":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.PEAP_MSCHAPV2;
                        break;
                    case "LEAP":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.LEAP;
                        break;
                    case "EAP_TTLS_CHAP":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TTLS_CHAP;
                        break;
                    case "EAP_TTLS_MD5":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TTLS_MD5;
                        break;
                    case "EAP_TTLS_MS_CHAP":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TTLS_MSCHAP;
                        break;
                    case "EAP_TTLS_MS_CHAP_V2":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TTLS_MSCHAPV2;
                        break;
                    case "EAP_TTLS_PAP":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.EAP_TTLS_PAP;
                        break;
                    case "WAPI_CERTIFICATE":
                        profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.WAPI_CERTIFICATE;
                        break;
                    default:
                        errorMessage = "Nie odczytano poprawnie typu uwierzytelniania";
                        break;
                }
                switch (p.EncryptionType)
                {
                    case "AES":
                        profile.Encryption.EncryptionType = Encryption.EncryptionTypes.AES;
                        break;
                    case "TKIP":
                        profile.Encryption.EncryptionType = Encryption.EncryptionTypes.TKIP;
                        break;
                }
                profile.Encryption.PassPhrase = p.PassPhrase;
                if (p.AllowMixedMode == "ON" || p.AllowMixedMode == "1")
                    profile.Encryption.AllowMixedMode = Encryption.ALLOW_MIXED_MODE.ALLOW_MIXED_MODE_ON;
                else if (p.AllowMixedMode == "OFF" || p.AllowMixedMode == "0")
                    profile.Encryption.AllowMixedMode = Encryption.ALLOW_MIXED_MODE.ALLOW_MIXED_MODE_OFF;
                else
                    errorMessage="Niewłaściwa wartość 'AllowMixedMode'";
                if (p.AESAllowMixedMode == "ON" || p.AESAllowMixedMode == "1") 
                    profile.Encryption.AESAllowMixedMode = Encryption.AES_ALLOW_MIXED_MODE.AES_ALLOW_MIXED_MODE_ON;
                else if (p.AESAllowMixedMode == "OFF" || p.AESAllowMixedMode == "0")
                    profile.Encryption.AESAllowMixedMode = Encryption.AES_ALLOW_MIXED_MODE.AES_ALLOW_MIXED_MODE_OFF;
                else
                    errorMessage="Niewłaściwa wartość 'AESAllowMixedMode'";
                if (p.AddressingMode == "STATIC")
                    profile.IPSettings.AddressingMode = IPSettings.AddressingModes.STATIC;
                else if (p.AddressingMode == "DHCP")
                    profile.IPSettings.AddressingMode = IPSettings.AddressingModes.DHCP;
                else
                    errorMessage = "Niewłaściwy typ adresowania";

                IPconfig ipCnf = new IPconfig(this);
                ipCnf.ShowDialog();
                profile.IPSettings.IPAddress = ipCnf.textBox1.Text;
                if (IpValidator(p.SubnetMask))
                    profile.IPSettings.SubnetMask = p.SubnetMask;
                else
                    errorMessage = "Niewłaściwa maska podsieci";
                if (IpValidator(p.Gateway))
                    profile.IPSettings.GateWay = p.Gateway;
                else
                    errorMessage = "Niewłaściwa brama 1";
                if (p.Gateway2 != "")
                {
                    if (IpValidator(p.Gateway2))
                        profile.IPSettings.GateWay2 = p.Gateway2;
                    else
                        errorMessage = "Nieprawidłowa brama 2";
                }
                if (IpValidator(p.Dns))
                    profile.IPSettings.DNS1 = p.Dns;
                else
                    errorMessage = "Niewłaściwy adres DNS 1";
                if (p.Dns2 != "")
                {
                    if (IpValidator(p.Dns2))
                        profile.IPSettings.DNS2 = p.Dns2;
                    else
                        errorMessage = "Nieprawidłowy DNS 2";
                }
                switch (p.PowerMode)
                {
                    case "CAM":
                        profile.PowerMode = Symbol.Fusion.WLAN.PowerMode.CAM;
                        break;
                    case "FAST":
                        profile.PowerMode = Symbol.Fusion.WLAN.PowerMode.FAST_POWER_SAVE;
                        break;
                    case "MAX":
                        profile.PowerMode = Symbol.Fusion.WLAN.PowerMode.MAX_POWER_SAVE;
                        break;
                    default:
                        errorMessage = "Nie udało się odczytać trybu energii";
                        break;
                }
                if (errorMessage == "")
                {
                    myWlan.Profiles.CreateInfrastructureProfile(profile);
                    return true;
                }
                else
                {
                    MessageBox.Show("Nie udało się dodać profilu.\n" + errorMessage);
                    return false;
                }
            }
            else
            {
                errorMessage="W konfigu nie ma wszystkich wymaganych informacji do stworzenia profilu";
                return false;
            }
        }
        private bool LoadDefaultProfile()
        {
            IPconfig ipCnf = new IPconfig(this);
            ipCnf.ShowDialog();
            string ip = ipCnf.textBox1.Text;
            try
            {
                InfrastructureProfileData profile = new InfrastructureProfileData("Ticketpro", "Ticketpro");
                profile.SecurityType = WLANSecurityType.SECURITY_WPA2_PERSONAL;
                profile.Authentication.AuthenticationType = Authentication.AuthenticationTypes.NONE;
                profile.Encryption.EncryptionType = Encryption.EncryptionTypes.AES;
                profile.Encryption.PassPhrase = "skanerzy36";
                profile.Encryption.AllowMixedMode = Encryption.ALLOW_MIXED_MODE.ALLOW_MIXED_MODE_ON;
                profile.Encryption.AESAllowMixedMode = Encryption.AES_ALLOW_MIXED_MODE.AES_ALLOW_MIXED_MODE_ON;
                profile.IPSettings.AddressingMode = IPSettings.AddressingModes.STATIC;
                profile.IPSettings.IPAddress = ip;
                profile.IPSettings.SubnetMask = "255.255.255.0";
                profile.IPSettings.GateWay = "10.10.30.100";
                profile.IPSettings.DNS1 = "10.10.30.100";
                profile.PowerMode = Symbol.Fusion.WLAN.PowerMode.CAM;
                myWlan.Profiles.CreateInfrastructureProfile(profile);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool ActivateProfile(string name)
        {
            busy = true;
            bool found = false;
            //sprawdzenie czy profil już istnieje
            for (int i = 0; i < myWlan.Profiles.Length; i++)
            {
                if (myWlan.Profiles[i].Name == name)
                    found = true;
            }
            //jeśli znaleziono profil
            if (found)
            {
                for (int i = 0; i < myWlan.Profiles.Length; i++)
                {
                    if (myWlan.Profiles[i].Name == name)
                    {
                        try
                        {
                            myWlan.Profiles[i].Enabled = true;
                            myWlan.Profiles[i].Connect(true);
                        }
                        catch
                        {
                            MessageBox.Show("Nie udało się połączyć");
                        }
                    }
                    else
                        myWlan.Profiles[i].Enabled = false;
                }
                busy = false;
                return true;
            }
            busy = false;
            return false;
        }
        
        //Tray Icon
        private void CreateTrayIcon() //tworzenie ikonki w trayu i menu kontekstowego
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            this.Closing += new CancelEventHandler(RemoveIcon);
            IntPtr hIcon = LoadIcon(GetModuleHandle(null), "#32512");
            notifyIcon.Add(hIcon);
            cMenu = new ContextMenu();
            MenuItem m0 = new MenuItem();
            m0.Text = "RA Connection: ";
            m0.Enabled = false;

            MenuItem m1 = new MenuItem();
            m1.Text = "Battery: ";
            m1.Enabled = false;

            MenuItem m3 = new MenuItem();
            m3.Text = "WiFi: ";
            m3.Enabled = false;

            MenuItem m4 = new MenuItem();
            m4.Text = "Signal: ";
            m4.Enabled = false;
            
            MenuItem m2 = new MenuItem();
            m2.Text = "Main View";
            m2.Click+=new EventHandler(SettingsFromTray_Click);

            cMenu.MenuItems.Add(m0);
            cMenu.MenuItems.Add(m1);
            cMenu.MenuItems.Add(m3);
            cMenu.MenuItems.Add(m4);
            cMenu.MenuItems.Add(m2);
        }
        private void RemoveIcon(object sender, EventArgs e)
        {
            notifyIcon.Remove();
        }
        [DllImport("coredll.dll")]
        internal static extern IntPtr LoadIcon(IntPtr hInst, string IconName);
        [DllImport("coredll.dll")]
        internal static extern IntPtr GetModuleHandle(String lpModuleName);

        //minimalizacja
        [DllImport("coredll.dll")]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_MINIMIZED = 6;
        public void Minimize(object sender, EventArgs e) //minimalizacja formatki
        {
            bAPI.StopRead();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.WindowState = FormWindowState.Normal;
            this.ControlBox = true;
            this.MinimizeBox = true;
            this.MaximizeBox = true;

            ShowWindow(this.Handle, SW_MINIMIZED);
        }

        private void label8_ParentChanged(object sender, EventArgs e)
        {

        }

        private void btManual_Click(object sender, EventArgs e)
        {
            ManualTic m = new ManualTic(this, label6.Text);
            m.ShowDialog();
        }

        /* private void alertTimer_Tick(object sender, EventArgs e)
        {
            /*sp = new System.Media.SoundPlayer(pathAlert);
            if (alertCounter > 0)
            {
                sp.Play();
                alertCounter--;
            }
            else
            {
                alertCounter = 6;
                alertTimer.Enabled = false;
            }
        }*/
    }
}