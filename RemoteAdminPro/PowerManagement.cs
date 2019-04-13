//--------------------------------------------------------------------
// FILENAME: PowerManagement.cs
//
// Copyright © 2011 Motorola Solutions, Inc. All rights reserved.
//
// DESCRIPTION:
//
// NOTES:
//
// 
//--------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Threading;

namespace RemoteAdminPro
{
    /// <summary>
    /// PowerManagement provides several methods to monitor and control 
    /// system and device(example: backlight, audio etc) power states. PowerManagement  
    /// does this by wrapping Microsoft's Power Management API. For more information on 
    /// Power Management please refer to MSDN.
    /// </summary>
    /// <remarks>
    /// PowerMangement should be used with extreme caution as it may result in 
    /// unexpected behavior. 
    /// </remarks>
    public class PowerManagement : IDisposable
    {
        #region ------------------Enumerations--------------------

        /// <summary>
        /// Defines the System power states
        /// </summary>
        public enum SystemPowerStates : uint
        {
            /// <summary>
            /// On state.
            /// </summary>
            On = 0x00010000,

            /// <summary>
            /// No power, full off.
            /// </summary>
            Off = 0x00020000,

            /// <summary>
            /// Critical off.
            /// </summary>
            Critical = 0x00040000,

            /// <summary>
            /// Boot state.
            /// </summary>
            Boot = 0x00080000,

            /// <summary>
            /// Idle state.
            /// </summary>
            Idle = 0x00100000,

            /// <summary>
            /// Suspend state.
            /// </summary>
            Suspend = 0x00200000,

            /// <summary>
            /// Reset state.
            /// </summary>
            Reset = 0x00800000
        }

        /// <summary>
        /// Defines the System power requirement flags
        /// </summary>
        public enum PowerReqFlags : uint
        {
            POWER_NAME = 0x00000001,
            POWER_FORCE = 0x00001000,
        }

        /// <summary>
        /// Defines the Device power states
        /// </summary>
        public enum DevicePowerStates
        {
            PwrDeviceUnspecified = -1,
            FullOn = 0,		// Full On: full power,  full functionality
            D0 = FullOn,
            LowOn,			// Low Power On: fully functional at low power/performance
            D1 = LowOn,
            StandBy,		// Standby: partially powered with automatic wake
            D2 = StandBy,
            Sleep,			// Sleep: partially powered with device initiated wake
            D3 = Sleep,
            Off,			// Off: unpowered
            D4 = Off,
            PwrDeviceMaximum
        }

        /// <summary>
        /// Defines the Power Status message type.
        /// </summary>
        [FlagsAttribute()]
        public enum MessageTypes : uint
        {
            /// <summary>
            /// System power state transition.
            /// </summary>
            Transition = 0x00000001,

            /// <summary>
            /// Resume from previous state.
            /// </summary>
            Resume = 0x00000002,

            /// <summary>
            /// Power supply switched to/from AC/DC.
            /// </summary>
            Change = 0x00000004,

            /// <summary>
            /// A member of the POWER_BROADCAST_POWER_INFO structure has changed.
            /// </summary>
            Status = 0x00000008
        }

        /// <summary>
        /// Defines the AC power status flags.
        /// </summary>
        public enum ACLineStatus : byte
        {
            /// <summary>
            /// AC power is offline.
            /// </summary>
            Offline = 0x00,

            /// <summary>
            /// AC power is online. 
            /// </summary>
            OnLine = 0x01,

            /// <summary>
            /// AC line status is unknown.
            /// </summary>
            Unknown = 0xff
        }

        /// <summary>
        /// Defines the Battery charge status flags.
        /// </summary>
        [FlagsAttribute()]
        public enum BatteryFlags : byte
        {
            /// <summary>
            /// High
            /// </summary>
            High = 0x01,

            /// <summary>
            /// Low
            /// </summary>
            Low = 0x02,

            /// <summary>
            /// Critical
            /// </summary>
            Critical = 0x04,

            /// <summary>
            /// Charging
            /// </summary>
            Charging = 0x08,

            /// <summary>
            /// Reserved1
            /// </summary>
            Reserved1 = 0x10,

            /// <summary>
            /// Reserved2
            /// </summary>
            Reserved2 = 0x20,

            /// <summary>
            /// Reserved3
            /// </summary>
            Reserved3 = 0x40,

            /// <summary>
            /// No system battery
            /// </summary>
            NoBattery = 0x80,

            /// <summary>
            /// Unknown status
            /// </summary>
            Unknown = High | Low | Critical | Charging | Reserved1 | Reserved2 | Reserved3 | NoBattery
        }

        /// <summary>
        /// Responses from <see cref="WaitForMultipleObjects"/> function.
        /// </summary>
        private enum Wait : uint
        {
            /// <summary>
            /// The state of the first object is signaled.
            /// </summary>
            Object0 = 0x00000000,
            /// <summary>
            /// The state of the second object is signaled.
            /// </summary>
            Object1 = 0x00000001,
            /// <summary>
            /// Wait abandoned.
            /// </summary>
            Abandoned = 0x00000080,
            /// <summary>
            /// Wait failed.
            /// </summary>
            Failed = 0xffffffff,
        }

        /// <summary>
        /// Provides list of event actions.
        /// </summary>
        private enum EventAction : uint
        {
            EVENT_PULSE = 1,
            EVENT_RESET = 2,
            EVENT_SET = 3
        }

        #endregion -----------------Enumerations-------------------

        #region --------------------Members-----------------------

        /// <summary>
        /// Indicates that an application would like to receive all types of 
        /// power notifications.
        /// </summary>
        private const uint POWER_NOTIFY_ALL = 0xFFFFFFFF;

        /// <summary>
        /// Indicates an infinite wait period
        /// </summary>
        private const int INFINITE = -1;

        /// <summary>
        /// Allocate message buffers on demand and free the message buffers after they are read.
        /// </summary>
        private const int MSGQUEUE_NOPRECOMMIT = 1;

        /// <summary>
        /// Event handle used to abort the worker thread
        /// </summary>
        private IntPtr powerThreadAbortEvent;

        /// <summary>
        /// Flag requesting worker thread closure
        /// </summary>
        private bool abortPowerThread = false;

        /// <summary>
        /// Flag to indicate that the worker thread is running
        /// </summary>
        private bool powerThreadRunning = false;

        /// <summary>
        /// Thread interface queue
        /// </summary>
        private Queue powerQueue;

        /// <summary>
        /// Handle to the message queue
        /// </summary>
        private IntPtr hMsgQ = IntPtr.Zero;

        /// <summary>
        /// Handle returned from RequestPowerNotifications
        /// </summary>
        private IntPtr hReq = IntPtr.Zero;

        /// <summary>
        /// Boolean used to indicate if the object has been disposed
        /// </summary>
        private bool bDisposed = false;

        /// <summary>
        /// Occurs when there is some PowerNotify information available.
        /// </summary>
        public event EventHandler PowerNotify;

        #endregion --------------------Members--------------------

        #region -------------------Structures---------------------

        /// <summary>
        /// Contains information about a message queue.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MessageQueueOptions
        {
            /// <summary>
            /// Size of the structure in bytes.
            /// </summary>
            public uint Size;

            /// <summary>
            /// Describes the behavior of the message queue. Set to MSGQUEUE_NOPRECOMMIT to 
            /// allocate message buffers on demand and to free the message buffers after 
            /// they are read, or set to MSGQUEUE_ALLOW_BROKEN to enable a read or write 
            /// operation to complete even if there is no corresponding writer or reader present.
            /// </summary>
            public uint Flags;

            /// <summary>
            /// Number of messages in the queue.
            /// </summary>
            public uint MaxMessages;

            /// <summary>
            /// Number of bytes for each message, do not set to zero.
            /// </summary>
            public uint MaxMessage;

            /// <summary>
            /// Set to TRUE to request read access to the queue. Set to FALSE to request write 
            /// access to the queue.
            /// </summary>
            public uint ReadAccess;
        };

        /// <summary>
        /// Contains information about the power status of the system  
        /// as received from the Power Status message queue.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PowerInfo
        {
            /// <summary>
            /// Defines the event type.
            /// </summary>
            /// <see cref="MessageTypes"/>
            public MessageTypes Message;

            /// <summary>
            /// One of the system power flags.
            /// </summary>
            /// <see cref="SystemPowerStates"/>
            public SystemPowerStates Flags;

            /// <summary>
            /// The byte count of SystemPowerState that follows. 
            /// </summary>
            public uint Length;

            /// <summary>
            /// Levels available in battery flag fields
            /// </summary>
            public uint NumLevels;

            /// <summary>
            /// Number of seconds of battery life remaining, 
            /// or 0xFFFFFFFF if remaining seconds are unknown.
            /// </summary>
            public uint BatteryLifeTime;

            /// <summary>
            /// Number of seconds of battery life when at full charge, 
            /// or 0xFFFFFFFF if full battery lifetime is unknown.
            /// </summary>
            public uint BatteryFullLifeTime;

            /// <summary>
            /// Number of seconds of backup battery life remaining, 
            /// or BATTERY_LIFE_UNKNOWN if remaining seconds are unknown.
            /// </summary>
            public uint BackupBatteryLifeTime;

            /// <summary>
            /// Number of seconds of backup battery life when at full charge, 
            /// or BATTERY_LIFE_UNKNOWN if full battery lifetime is unknown.
            /// </summary>
            public uint BackupBatteryFullLifeTime;

            /// <summary>
            /// AC power status. 
            /// </summary>
            /// <see cref="ACLineStatus"/>
            public ACLineStatus ACLineStatus;

            /// <summary>
            /// Battery charge status. 
            /// </summary>
            /// <see cref="BatteryFlags"/>
            public BatteryFlags BatteryFlag;

            /// <summary>
            /// Percentage of full battery charge remaining. 
            /// This member can be a value in the range 0 (zero) to 100, or 255 
            /// if the status is unknown. All other values are reserved.
            /// </summary>
            public byte BatteryLifePercent;

            /// <summary>
            /// Backup battery charge status. 
            /// </summary>
            public byte BackupBatteryFlag;

            /// <summary>
            /// Percentage of full backup battery charge remaining. 
            /// This value must be in the range of 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN.
            /// </summary>
            public byte BackupBatteryLifePercent;
        };

        #endregion -------------------Structures------------------

        #region ---------------------Methods----------------------

        /// <summary>
        /// Ensures that resources are freed when the garbage collector reclaims the object.
        /// </summary>
        ~PowerManagement()
        {
            Dispose();
        }

        /// <summary>
        /// Releases the resources used by the object.
        /// </summary>
        public void Dispose()
        {
            if (!bDisposed)
            {
                // Try disabling notifications and ending the thread
                DisableNotifications();
                bDisposed = true;

                // SupressFinalize to take this object off the finalization queue 
                // and prevent finalization code for this object from executing a second time.
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Sets the system power state to the requested value.
        /// </summary>
        /// <param name="systemState">The system power state to set the device to.</param>
        /// <returns>Win32 error code</returns>
        /// <remarks>Should be used with extreme care since it may result in an unexpected 
        /// application or system behavior.</remarks>
        public int SetSystemPowerState(SystemPowerStates systemState)
        {
            uint nError = 0;

            nError = CESetSystemPowerState(
                IntPtr.Zero,
                (uint)systemState,
                0);

            return (int)nError;
        }

        /// <summary>
        /// Returns the current system power state currently in effect.
        /// </summary>
        /// <param name="systemStateName">Receives the system power state name</param>
        /// <param name="systemState">Receives the system power state</param>
        /// <returns>Win32 error code</returns>
        public int GetSystemPowerState(StringBuilder systemStateName, out SystemPowerStates systemState)
        {
            uint nError = 0;

            nError = CEGetSystemPowerState(systemStateName, (uint)systemStateName.Capacity, out systemState);

            return (int)nError;
        }

        /// <summary>
        /// Requests that the Power Manager change the power state of a device.
        /// </summary>
        /// <param name="deviceName">Specifies the device name, for example, COM1:.</param>
        /// <param name="deviceState">Indicates the device power state</param>
        /// <returns>Win32 error code</returns>
        /// <remarks>Should be used with extreme care since it may result in an unexpected 
        /// application or system behavior.</remarks>
        public int DevicePowerNotify(string deviceName, DevicePowerStates deviceState)
        {
            uint nError = 0;

            nError = CEDevicePowerNotify(deviceName, (uint)deviceState, (uint)PowerReqFlags.POWER_NAME);

            return (int)nError;
        }

        /// <summary>
        /// Activates notification events. An application can now register to PowerNotify and be 
        /// notified when a power notification is received.
        /// </summary>
        public void EnableNotifications()
        {
            // Set the message queue options
            MessageQueueOptions Options = new MessageQueueOptions();

            // Size in bytes ( 5 * 4)
            Options.Size = (uint)Marshal.SizeOf(Options);
            // Allocate message buffers on demand and to free the message buffers after they are read
            Options.Flags = MSGQUEUE_NOPRECOMMIT;
            // Number of messages in the queue.
            Options.MaxMessages = 32;
            // Number of bytes for each message, do not set to zero.
            Options.MaxMessage = 512;
            // Set to true to request read access to the queue.
            Options.ReadAccess = 1;	// True

            // Create the queue and request power notifications on it
            hMsgQ = CECreateMsgQueue("PowerNotifications", ref Options);

            hReq = CERequestPowerNotifications(hMsgQ, POWER_NOTIFY_ALL);

            // If the above succeed
            if (hMsgQ != IntPtr.Zero && hReq != IntPtr.Zero)
            {
                powerQueue = new Queue();

                // Create an event so that we can kill the thread when we want				
                powerThreadAbortEvent = CreateEvent(IntPtr.Zero, 1, 0, null);

                // Create the power watcher thread
                new Thread(new ThreadStart(PowerNotifyThread)).Start();
            }
        }

        /// <summary>
        /// Disables power notification events.
        /// </summary>
        public void DisableNotifications()
        {
            // If we are already closed just exit
            if (!powerThreadRunning)
                return;

            // Attempt to end the PowerNotifyThread
            EventModify(powerThreadAbortEvent, EventAction.EVENT_SET);

            // Wait for the thread to stop
            int count = 0;
            while (powerThreadRunning)
            {
                Thread.Sleep(100);

                // If it did not stop it time record this and give up
                if (count++ > 50)
                    break;
            }

            // Close the abort event handle
            CloseHandle(powerThreadAbortEvent);
        }

        /// <summary>
        /// Obtain the next PowerInfo structure
        /// </summary>
        public PowerInfo GetNextPowerInfo()
        {
            // Get the next item from the queue in a thread safe manner
            lock (powerQueue.SyncRoot)
                return (PowerInfo)powerQueue.Dequeue();
        }

        /// <summary>
        /// Worker thread that creates and reads a message queue for power notifications
        /// </summary>
        private void PowerNotifyThread()
        {
            powerThreadRunning = true;

            // Keep going util we are asked to quit
            while (!abortPowerThread)
            {
                IntPtr[] Handles = new IntPtr[2];

                Handles[0] = hMsgQ;
                Handles[1] = powerThreadAbortEvent;

                // Wait on two handles because the message queue will never
                // return from a read unless messages are posted.
                Wait res = (Wait)CEWaitForMultipleObjects(
                                                        (uint)Handles.Length,
                                                        Handles,
                                                        false,
                                                        INFINITE);


                switch (res)
                {
                    // This must be an error - Exit loop and thread
                    case Wait.Abandoned:
                        abortPowerThread = true;
                        break;

                    // Timeout - Continue after a brief sleep
                    case Wait.Failed:
                        Thread.Sleep(500);
                        break;

                    // Read the message from the queue
                    case Wait.Object0:
                        {
                            // Create a new structure to read into
                            PowerInfo Power = new PowerInfo();

                            uint PowerSize = (uint)Marshal.SizeOf(Power);
                            uint BytesRead = 0;
                            uint Flags = 0;

                            // Read the message
                            if (CEReadMsgQueue(hMsgQ, ref Power, PowerSize,
                                                ref BytesRead, 0, ref Flags))
                            {
                                // Set value to zero if percentage is not known
                                if ((Power.BatteryLifePercent < 0) || (Power.BatteryLifePercent > 100))
                                    Power.BatteryLifePercent = 0;

                                if ((Power.BackupBatteryLifePercent < 0) || (Power.BackupBatteryLifePercent > 100))
                                    Power.BackupBatteryLifePercent = 0;

                                // Add the power structure to the queue so that the 
                                // UI thread can get it
                                lock (powerQueue.SyncRoot)
                                    powerQueue.Enqueue(Power);

                                // Fire an event to notify the UI
                                if (PowerNotify != null)
                                    PowerNotify(this, null);
                            }

                            break;
                        }

                    // Process the thread aboart event (fired from DisableNotifications method)
                    case Wait.Object1:
                        EventModify(powerThreadAbortEvent, EventAction.EVENT_RESET);
                        abortPowerThread = true;
                        break;
                }
            }

            // Stop receiving power notifications
            if (hReq != IntPtr.Zero)
                CEStopPowerNotifications(hReq);

            // Close the message queue
            if (hMsgQ != IntPtr.Zero)
                CECloseMsgQueue(hMsgQ);

            powerThreadRunning = false;
        }


        #endregion -----------------Methods---------------------

        #region ---------Native Power Management Imports----------

        [DllImport("coredll.dll", EntryPoint = "RequestPowerNotifications")]
        private static extern IntPtr CERequestPowerNotifications(IntPtr hMsgQ, uint Flags);

        [DllImport("coredll.dll", EntryPoint = "StopPowerNotifications")]
        private static extern bool CEStopPowerNotifications(IntPtr hReq);

        [DllImport("coredll.dll", EntryPoint = "SetDevicePower")]
        private static extern uint CESetDevicePower(string Device, uint dwDeviceFlags, uint DeviceState);

        [DllImport("coredll.dll", EntryPoint = "GetDevicePower")]
        private static extern uint CEGetDevicePower(string Device, uint dwDeviceFlags, uint DeviceState);

        [DllImport("coredll.dll", EntryPoint = "DevicePowerNotify")]
        private static extern uint CEDevicePowerNotify(string Device, uint DeviceState, uint Flags);

        [DllImport("coredll.dll", EntryPoint = "SetSystemPowerState")]
        private static extern uint CESetSystemPowerState(IntPtr sState, uint StateFlags, uint Options);

        [DllImport("coredll.dll", EntryPoint = "GetSystemPowerState")]
        private static extern uint CEGetSystemPowerState(StringBuilder Buffer, uint Length, out SystemPowerStates Flags);

        [DllImport("coredll.dll", EntryPoint = "CreateMsgQueue")]
        private static extern IntPtr CECreateMsgQueue(string Name, ref MessageQueueOptions Options);

        [DllImport("coredll.dll", EntryPoint = "CloseMsgQueue")]
        private static extern bool CECloseMsgQueue(IntPtr hMsgQ);

        [DllImport("coredll.dll", EntryPoint = "ReadMsgQueue")]
        private static extern bool CEReadMsgQueue(IntPtr hMsgQ, ref PowerInfo Power, uint BuffSize, ref uint BytesRead, uint Timeout, ref uint Flags);

        [DllImport("coredll.dll", EntryPoint = "WaitForMultipleObjects", SetLastError = true)]
        private static extern int CEWaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);

        [DllImport("Coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr passZero, Int32 manualReset, Int32 initState, string name);

        [DllImport("Coredll.dll", EntryPoint = "EventModify", SetLastError = true)]
        private static extern int EventModify(IntPtr hEvent, EventAction action);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        #endregion ---------Native Power Management Imports----------
    }
}
