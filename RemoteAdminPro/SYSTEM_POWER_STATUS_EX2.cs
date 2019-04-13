using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace RemoteAdminPro
{
    class SYSTEM_POWER_STATUS_EX2
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
        public byte Reserved2;
        public byte BackupBatteryFlag;
        public byte BackupBatteryLifePercent;
        public byte Reserved3;
        public int BackupBatteryLifeTime;
        public int BackupBatteryFullLifeTime;
        public int BatteryVoltage;
        public int BatteryCurrent;
        public int BatteryAverageCurrent;
        public int BatteryAverageInterval;
        public int BatterymAHourConsumed;
        public int BatteryTemperature;
        public int BackupBatteryVoltage;
        public byte BatteryChemistry;
    }
}
