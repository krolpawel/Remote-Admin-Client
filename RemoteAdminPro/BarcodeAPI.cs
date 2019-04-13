using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Symbol.Barcode;
/// <summary>
/// The namespace for CS_BarcodeSample1.
/// </summary>
namespace RemoteAdminPro
{

    /// <summary>
    /// The class which communicates with the EMDK for .NET scanner API 
    /// - Symbol.Barcode. 
    /// </summary>
    class BarcodeAPI
    {
        private Reader myReader = null;
        private ReaderData myReaderData = null;
        private System.EventHandler myReadNotifyHandler = null;
        private System.EventHandler myStatusNotifyHandler = null;
        Symbol.Generic.Device MyDevice = null;

        internal bool isBackground = false; //The flag used to track whether the application is in background or not (in foreground).

        /// <summary>
        /// Initialize the reader.
        /// </summary>
        public bool InitReader()
        {
            // If the reader is already initialized then fail the initialization.
            if (myReader != null)
            {
                return false;
            }
            else // Else initialize the reader.
            {
                try
                {
                    // Get the device selected by the user.
                    for (int i = 0; i < Symbol.Barcode.Device.AvailableDevices.Count(); i++)
                    {
                        if (Device.AvailableDevices[i].DeviceName == "SCN1:")
                        {
                            MyDevice = Device.AvailableDevices[i];
                        }
                    }

                    if (MyDevice == null)
                    {
                        return false;
                    }

                    // Create the reader, based on selected device.
                    myReader = new Reader(MyDevice);

                    // Create the reader data.
                    myReaderData = new ReaderData(
                        ReaderDataTypes.Text,
                        ReaderDataLengths.MaximumLabel);

                    // Enable the Reader.
                    myReader.Actions.Enable();

                    // In this sample, we are setting the aim type to trigger. 
                    switch (myReader.ReaderParameters.ReaderType)
                    {
                        case Symbol.Barcode.READER_TYPE.READER_TYPE_IMAGER:
                            myReader.ReaderParameters.ReaderSpecific.ImagerSpecific.AimType = Symbol.Barcode.AIM_TYPE.AIM_TYPE_TRIGGER;
                            break;
                        case Symbol.Barcode.READER_TYPE.READER_TYPE_LASER:
                            myReader.ReaderParameters.ReaderSpecific.LaserSpecific.AimType = Symbol.Barcode.AIM_TYPE.AIM_TYPE_TRIGGER;
                            break;
                        case Symbol.Barcode.READER_TYPE.READER_TYPE_CONTACT:
                            // AimType is not supported by the contact readers.
                            break;
                    }
                    myReader.Actions.SetParameters();


                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Stop reading and disable/close the reader.
        /// </summary>
        public void TermReader()
        {
            // If we have a reader
            if (myReader != null)
            {
                try
                {
                    // stop all the notifications.
                    StopRead();

                    //Detach all the notification handler if the user has not done it already.
                    DetachReadNotify();
                    DetachStatusNotify();

                    // Disable the reader.
                    myReader.Actions.Disable();

                    // Free it up.
                    myReader.Dispose();

                    // Make the reference null.
                    myReader = null;
                }

                catch{}
            }

            // After disposing the reader, dispose the reader data. 
            if (myReaderData != null)
            {
                try
                {
                    // Free it up.
                    myReaderData.Dispose();

                    // Make the reference null.
                    myReaderData = null;
                }

                catch{}
            }
        }

        /// <summary>
        /// Start a read on the reader.
        /// </summary>
        public void StartRead(bool toggleSoftTrigger)
        {
            // If we have both a reader and a reader data
            if ((myReader != null) &&
                (myReaderData != null) && (isBackground == false))

                try
                {
                    if (!myReaderData.IsPending)
                    {
                        // Submit a read.
                        myReader.Actions.Read(myReaderData);

                        if (toggleSoftTrigger && myReader.Info.SoftTrigger == false)
                        {
                            myReader.Info.SoftTrigger = true;
                        }
                    }
                }

                catch{}
        }

        /// <summary>
        /// Stop all reads on the reader.
        /// </summary>
        public void StopRead()
        {
            //If we have a reader
            if (myReader != null)
            {
                try
                {
                    // Flush (Cancel all pending reads).
                    if (myReader.Info.SoftTrigger == true)
                    {
                        myReader.Info.SoftTrigger = false;
                    }
                    myReader.Actions.Flush();
                }

                catch{}
            }
        }

        /// <summary>
        /// Provides the access to the Symbol.Barcode.Reader reference.
        /// The user can use this reference for his additional Reader - related operations.
        /// </summary>
        public Symbol.Barcode.Reader Reader
        {
            get
            {
                return myReader;
            }
        }

        /// <summary>
        /// Attach a ReadNotify handler.
        /// </summary>
        public void AttachReadNotify(System.EventHandler ReadNotifyHandler)
        {
            // If we have a reader
            if (myReader != null)
            {
                // Attach the read notification handler.
                myReader.ReadNotify += ReadNotifyHandler;
                myReadNotifyHandler = ReadNotifyHandler;
            }

        }

        /// <summary>
        /// Detach the ReadNotify handler.
        /// </summary>
        public void DetachReadNotify()
        {
            if ((myReader != null) && (myReadNotifyHandler != null))
            {
                // Detach the read notification handler.
                myReader.ReadNotify -= myReadNotifyHandler;
                myReadNotifyHandler = null;
            }
        }

        /// <summary>
        /// Attach a StatusNotify handler.
        /// </summary>
        public void AttachStatusNotify(System.EventHandler StatusNotifyHandler)
        {
            // If we have a reader
            if (myReader != null)
            {
                // Attach status notification handler.
                myReader.StatusNotify += StatusNotifyHandler;
                myStatusNotifyHandler = StatusNotifyHandler;
            }
        }

        /// <summary>
        /// Detach a StatusNotify handler.
        /// </summary>
        public void DetachStatusNotify()
        {
            // If we have a reader registered for receiving the status notifications
            if ((myReader != null) && (myStatusNotifyHandler != null))
            {
                // Detach the status notification handler.
                myReader.StatusNotify -= myStatusNotifyHandler;
                myStatusNotifyHandler = null;
            }
        }

    }

}
