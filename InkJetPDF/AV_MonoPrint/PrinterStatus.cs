using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ttp.Meteor.MeteorMonoPrint
{
    public enum PRINTER_STATUS
    {
        /// <summary>
        /// Meteor is not connected.  This will happen if the Meteor PrintEngine has not
        /// yet been started (e.g. Monitor.exe needs to be run), or if another application
        /// is currently connected to Meteor.
        /// </summary>
        DISCONNECTED,
        /// <summary>
        /// Meteor is connected and waiting for hardware
        /// </summary>
        WAITING_FOR_PCC,
        /// <summary>
        /// Meteor is initialising the PCC hardware
        /// </summary>
        INITIALISING,
        /// <summary>
        /// Meteor is inactive
        /// </summary>
        IDLE,
        /// <summary>
        /// The print data for the next print job is being loaded into the hardware buffers
        /// </summary>
        LOADING,
        /// <summary>
        /// The current print job is aborting
        /// </summary>
        ABORTING,
        /// <summary>
        /// The print data for the next print job is loaded and Meteor is waiting for an external
        /// product detect signal to start printing
        /// </summary>
        READY,
        /// <summary>
        /// Meteor is printing
        /// </summary>
        PRINTING,
        /// <summary>
        /// Meteor is in an error condition
        /// </summary>
        ERROR
    }

    /// <summary>
    /// Object to deal with connecting to Meteor and returning the current Meteor
    /// status.  For simplicity this object is designed to be called periodically 
    /// from a timer running in the main application form.
    /// 
    /// [ For more complex print applications it may be better to deal with the status 
    ///   polling in an asynchronous thread. ]
    ///   
    /// </summary>
    class PrinterStatus
    {
        /// <summary>
        /// Number of pccs required for the system as defined in the Meteor .cfg file
        /// </summary>
        public int PccsRequired { get; private set; }
        /// <summary>
        /// Is the connection to the Meteor Printer Interface currently open
        /// </summary>
        public bool Connected { get; private set; }
        /// <summary>
        /// Bitmask indicating the bits per pixel values that Meteor can use for
        /// the current head type.  BIT1 (0x2) for 1bpp, BIT2 (0x4) for 2bpp, etc.
        /// </summary>
        public Int32 SupportedBppBitmask { get; private set; }
        /// <summary>
        /// False if Meteor is in the process of turning head power on or off
        /// </summary>
        public bool HeadPowerIdle { get; private set; }
        /// <summary>
        /// Is head power enabled for at least one head in the system
        /// </summary>
        public bool HeadPowerEnabled { get; private set; }

        /// <summary>
        /// Tick count when we last attempted to open the Meteor Printer Interface
        /// </summary>
        private int tickLastConnectAttempt = Environment.TickCount - CONNECT_INTERVAL_MS;
        /// <summary>
        /// Interval between attemps to open the Printer Interface
        /// </summary>
        private const int CONNECT_INTERVAL_MS = 2000;

        /// Protect against brief transitions to MPS_IDLE
        private ePRINTERSTATE LastMeteorState = ePRINTERSTATE.MPS_DISCONNECTED;
        private const int IDLE_CONF = 4;
        private int idleConfCount = 0;

        /// <summary>
        /// Disconnect from the Meteor Printer Interface if currently connected.
        /// Should be called before the application exits.
        /// </summary>
        public void Disconnect() {
            if (Connected) {
                PrinterInterfaceCLS.PiAbort();
                WaitNotBusy();
                PrinterInterfaceCLS.PiClosePrinter();
                Connected = false;
            }
        }
    
        /// <summary>
        /// Get the status from Meteor.  Also attempts to open a connection to Meteor every
        /// 2 seconds if the application is not currently connected.
        /// </summary>
        /// <returns></returns>
        public PRINTER_STATUS GetStatus() {
            TAppStatus AppStatus;
            eRET rVal;

            if (!Connected) {
                if (Environment.TickCount < tickLastConnectAttempt + CONNECT_INTERVAL_MS) {
                    return PRINTER_STATUS.DISCONNECTED;
                }
                tickLastConnectAttempt = Environment.TickCount;
                
                rVal = PrinterInterfaceCLS.PiOpenPrinter();
                // RVAL_OK means the Meteor Printer Interface was successfully opened.
                //
                // RVAL_EXISTS means that the Printer Interface is already open in this
                // process.
                //
                // All other return values mean we have failed to open the printer 
                // interface - the most likely reasons are either the Meteor Print Engine
                // is not running, or the Meteor Printer Interface is already open in another
                // print application.
                //
                if (rVal != eRET.RVAL_OK && rVal != eRET.RVAL_EXISTS) {
                    return PRINTER_STATUS.DISCONNECTED;
                }
                Connected = true;
                // Send an abort command in case the application which previously used
                // Meteor left a print job running.  This is not done if Meteor is
                // currently initialising the hardware, as in this case it should 
                // not be necessary, and can take a significant amount of time to complete.
                rVal = PrinterInterfaceCLS.PiGetPrnStatus(out AppStatus);
                if (rVal == eRET.RVAL_OK && AppStatus.PrinterState != ePRINTERSTATE.MPS_INITIALIZING) {
                    PrinterInterfaceCLS.PiAbort();
                }
                // Wait until the abort has completed.  While an abort is in progress
                // the printer interface is busy, and status requests will fail.
                WaitNotBusy();
            }

            // Request the Meteor printer status.  Provided the Print Engine is
            // still running, this method should never fail in a single threaded
            // printer application.  In a more complex application which uses
            // multiple threads to can communicate with the Print Engine, it
            // is possible (though rare) for the status request to time out if 
            // the Printer Interface is busy servicing another request.
            //
            rVal = PrinterInterfaceCLS.PiGetPrnStatus(out AppStatus);
            if (rVal != eRET.RVAL_OK) {
                if (Connected) {
                    PrinterInterfaceCLS.PiClosePrinter();
                    Connected = false;
                }
                return PRINTER_STATUS.DISCONNECTED;
            }

            // Store the bitmask indicating which bits-per-pixel are valid for the current head type
            SupportedBppBitmask = AppStatus.SupportedBppBitmask;
            
            // The Meteor Print Engine state goes into IDLE after all connecteded PCCs have finished 
            // initialising.
            //
            // Here we trap the case of the Meteor configuration file listing more PCCs than are
            // currently attached.
            //
            PccsRequired = AppStatus.PccsRequired;
            if (PccsRequired != AppStatus.PccsAttached) {
                return PRINTER_STATUS.WAITING_FOR_PCC;
            }
            // Protect against the application acting on a brief status transition to IDLE
            ePRINTERSTATE PrinterState = AppStatus.PrinterState;
            if (AppStatus.PrinterState == ePRINTERSTATE.MPS_IDLE) {
                if (idleConfCount < IDLE_CONF) {
                    PrinterState = LastMeteorState;
                    idleConfCount++;
                }
            } else {
                idleConfCount = 0;
            }
            // Track last known stable printer state
            LastMeteorState = PrinterState;
            // Find out whether Meteor is currently turning head power on or off
            CheckHeadPowerIdle();
            // Is at least one head powered on
            HeadPowerEnabled = (AppStatus.HeadPowerState == eHEADPOWERSTATE.HPS_HEAD);
            // Convert from the Meteor printer status into the application status
            switch (PrinterState) {
                // There is no hardware connected.
                case ePRINTERSTATE.MPS_DISCONNECTED:
                    return PRINTER_STATUS.WAITING_FOR_PCC;
                // The PCC hardware has connected to the Print Engine but has not yet been
                // initialised
                case ePRINTERSTATE.MPS_CONNECTED:
                    return PRINTER_STATUS.INITIALISING;
                // The Print Engine is initialised and waiting for a print job to start
                case ePRINTERSTATE.MPS_IDLE:
                    return PRINTER_STATUS.IDLE;
                // The next print job is ready to start
                case ePRINTERSTATE.MPS_READY:
                    return PRINTER_STATUS.READY;
                // Meteor is actively printing
                case ePRINTERSTATE.MPS_PRINTING:
                    return PRINTER_STATUS.PRINTING;
                // The Print Engine is downloading FPGA code to the PCCs    
                case ePRINTERSTATE.MPS_INITIALIZING:
                    return PRINTER_STATUS.INITIALISING;
                // The PCC FPGA has completed programming and is starting up
                case ePRINTERSTATE.MPS_STARTUP:
                    return PRINTER_STATUS.INITIALISING;
                // Fault condition - more information can be found by looking at the
                // values in the status registers.  For the purposes of this application
                // we just report the generic error; the register values can be viewed
                // in Monitor
                case ePRINTERSTATE.MPS_FAULT:
                    return PRINTER_STATUS.ERROR;
                // Image data is being loaded
                case ePRINTERSTATE.MPS_LOADING:
                    return PRINTER_STATUS.LOADING;
                // Should never get here
                default:
                    return PRINTER_STATUS.ERROR;
            }
        }

        /// <summary>
        /// Polls the Printer Interface until it finishes the previous control command
        /// Use timeoutms to specify how long to wait in ms, default is 10000ms.
        /// For example, the Printer Interface will remain busy after a
        /// PiAbort until the abort has completed in Meteor.
        /// Note that the interface does not become busy as a result of a call to
        /// PiSendCommand.
        /// </summary>
        /// <param name="timeoutms">Timeout in milliseconds.  Defaults to ten seconds.</param>
        public bool WaitNotBusy(int timeoutms = 10000) {
            while (PrinterInterfaceCLS.PiIsBusy()) {
                Thread.Sleep(100);
                if ((timeoutms -= 100) <= 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Query the head and auxiliary temperatures for the given pcc / head
        /// </summary>
        /// <param name="pccnum">pcc index (1-max)</param>
        /// <param name="hnum">head index (1-max)</param>
        /// <param name="temperature1">set with the head temperature</param>
        /// <param name="temperature2">set with the aux temperature</param>
        /// <returns>success / failure</returns>
        public bool GetHeadTemperature(int pccnum, int hnum, out int temperature1, out int temperature2) {
            TAppHeadStatus HeadStatus;
            if (PrinterInterfaceCLS.PiGetHeadStatus(pccnum, hnum, out HeadStatus) == eRET.RVAL_OK) {
                // Convert from 0.1 degrees to degrees and round
                temperature1 = (int)(((double)HeadStatus.Temperature1 / 10.0) + 0.5);
                temperature2 = (int)(((double)HeadStatus.Temperature2 / 10.0) + 0.5);
                return true;
            } else {
                temperature1 = 0;
                temperature2 = 0;
                return false;
            }
        }

        /// <summary>
        /// Check whether any of the PCCs in the system are in the process of turning head
        /// power on or off.  Should only be called when all required PCCs are connected.
        /// Sets the value of HeadPowerIdle.
        /// </summary>
        private void CheckHeadPowerIdle() {
            for (int pccNum = 1; pccNum <= PccsRequired; pccNum++) {
                TAppPccStatus pccStatus;
                eRET rVal = PrinterInterfaceCLS.PiGetPccStatus(pccNum, out pccStatus);
                if (rVal != eRET.RVAL_OK) {
                    HeadPowerIdle = false;
                    return;
                }
                if ((pccStatus.bmStatusBits2 & Bmps.BMPS2_HEAD_POWER_IN_PROGRESS) != 0) {
                    HeadPowerIdle = false;
                    return;
                }
            }
            HeadPowerIdle = true;
        }
    }
}
