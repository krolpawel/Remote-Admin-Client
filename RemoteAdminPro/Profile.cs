using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteAdminPro
{
    [Serializable()]
    public class Profile
    {
        //pola
        string name;
        string ssid;
        string securityType;
        string authenticationType;
        string encryptionType;
        string passPhrase;
        string allowMixedMode;
        string aesAllowMixedMode;
        string addressingMode;
        string ipAddress;
        string subnetMask;
        string gateway;
        string gateway2;
        string dns;
        string dns2;
        string powerMode;

        //metody dostępowe
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Ssid
        {
            get { return ssid; }
            set { ssid = value; }
        }        
        public string SecurityType
        {
            get { return securityType; }
            set { securityType = value; }
        }   
        public string AuthenticationType
        {
            get { return authenticationType; }
            set { authenticationType = value; }
        }       
        public string EncryptionType
        {
            get { return encryptionType; }
            set { encryptionType = value; }
        }
        public string PassPhrase
        {
            get { return passPhrase; }
            set { passPhrase = value; }
        }
        public string AllowMixedMode
        {
            get { return allowMixedMode; }
            set { allowMixedMode = value; }
        }      
        public string AESAllowMixedMode
        {
            get { return aesAllowMixedMode; }
            set { aesAllowMixedMode = value; }
        }        
        public string AddressingMode
        {
            get { return addressingMode; }
            set { addressingMode = value; }
        }       
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
        public string SubnetMask
        {
            get { return subnetMask; }
            set { subnetMask = value; }
        }
        public string Gateway
        {
            get { return gateway; }
            set { gateway = value; }
        }
        public string Gateway2
        {
            get { return gateway2; }
            set { gateway2 = value; }
        }
        public string Dns
        {
            get { return dns; }
            set { dns = value; }
        }
        public string Dns2
        {
            get { return dns2; }
            set { dns2 = value; }
        }
        public string PowerMode
        {
            get { return powerMode; }
            set { powerMode = value; }
        }

        //konstruktory
        public Profile()
        {
            name = "";
            ssid = "";
            securityType = "";
            authenticationType = "";
            encryptionType = "";
            passPhrase = "";
            allowMixedMode = "";
            aesAllowMixedMode = "";
            addressingMode = "";
            ipAddress = "";
            subnetMask = "";
            gateway = "";
            gateway2 = "";
            dns = "";
            dns2 = "";
            powerMode = "";
        }
        public Profile(string _name, string _ssid, string _secType, string _authType, string _encType, string _pass, string _allowMixedMode,
            string _aesAllowMixedMode, string _addressingMode, string _ip, string _subnetMask, string _gateway, string _gateway2,
            string _dns, string _dns2, string _powerMode)
        {
            name = _name;
            ssid = _ssid;
            securityType = _secType;
            authenticationType = _authType;
            encryptionType = _encType;
            passPhrase = _pass;
            allowMixedMode = _allowMixedMode;
            aesAllowMixedMode = _aesAllowMixedMode;
            addressingMode = _addressingMode;
            ipAddress = _ip;
            subnetMask = _subnetMask;
            gateway = _gateway;
            gateway2 = _gateway2;
            dns = _dns;
            dns2 = _dns2;
            powerMode = _powerMode;
        }
    }
}
