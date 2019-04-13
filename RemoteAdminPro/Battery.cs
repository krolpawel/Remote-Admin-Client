using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace RemoteAdminPro
{
    class Battery
    {
        private bool m_bOpen = false;
        SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();
        SYSTEM_POWER_STATUS_EX2 status2 = new SYSTEM_POWER_STATUS_EX2();
        public byte GetACStatus()
        {
            //if (status.ACLineStatus == true) return true;
            //else return false;
            return status.ACLineStatus;
        }
        public int GetBS()
        {
            if (GetSystemPowerStatusEx(status, false) == 1)
            {
                //return String.Format("{0}%", status.BatteryLifePercent);
                return status.BatteryLifePercent;
            }
            return 0;
        }
        /*public int GetBS2()
        {
            if (GetSystemPowerStatusEx2(status2,
                (uint)Marshal.SizeOf(status2), false) ==
                (uint)Marshal.SizeOf(status2))
            {
                //return String.Format("{0}%", status2.BackupBatteryLifePercent);
                return status2.BackupBatteryLifePercent;
            }
            else
            {
                return 0;
            }
        }*/
        public int GetBS2()
        {
            if (GetSystemPowerStatusEx(status, false) == 1)
            {
                //return String.Format("{0}%", status.BatteryLifePercent);
                return status.BackupBatteryLifePercent;
            }
            return 0;
        }
        [DllImport("coredll")]
        private static extern uint GetSystemPowerStatusEx(SYSTEM_POWER_STATUS_EX lpSystemPowerStatus,
            bool fUpdate);

        [DllImport("coredll")]
        private static extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus,
            uint dwLen, bool fUpdate);

        // Percentage of full battery charge remaining. This member can be a value in the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN if the status is unknown.

    }
}

