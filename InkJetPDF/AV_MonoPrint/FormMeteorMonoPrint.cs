using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting;
using System.Security.Permissions;
using W8AVMOM;
using System.Runtime.Remoting.Channels.Tcp;

namespace Ttp.Meteor.MeteorMonoPrint
{
    public partial class FormMeteorMonoPrint : Form

    {
        //   [SecurityPermission(SecurityAction.Demand)]
        /// <summary>
        /// Object handles Meteor connection and status
        /// </summary>
        private PrinterStatus status = new PrinterStatus();
        /// <summary>
        /// Last known printer status
        /// </summary>
        private PRINTER_STATUS latestPrinterStatus = PRINTER_STATUS.DISCONNECTED;
        /// <summary>
        /// Currently loaded print image
        /// </summary>
        private IMeteorImageData image;
        /// <summary>
        /// Prevent re-entrancy problems in the timers - can happen if 
        /// a message box is displayed.
        /// </summary>
        private bool inTimer;
        /// <summary>
        /// Prevent re-entrancy if PiSetHeadPower fails and the check box
        /// value is reset from within the check changed handler; also used
        /// if we change the head power check box state based on the Meteor
        /// status
        /// </summary>
        private bool inSetHeadPower;
        /// <summary>
        /// The last target head temperature value which was sent to Meteor
        /// </summary>
        private int lastSetHeadTemperature = -1;
        /// <summary>
        /// The last target auxiliary temperature value which was sent to Meteor
        /// </summary>
        private int lastSetAuxTemperature = -1;
        /// <summary>
        /// The last known value of the bits-per-pixel modes supported by Meteor
        /// This can change if the Meteor head type is changed
        /// </summary>
        private int lastSupportedBppBitmask = 0;
        /// <summary>
        /// ID of the latest meteor print job.  Incremented for each job.
        /// </summary>
        private int jobid = 1;
        /// <summary>
        /// Set after a job has been successfully started when there is Meteor
        /// hardware connected, and cleared when the Meteor status changes to
        /// ready or printing.
        /// The flag is not set if a job is started without hardware (e.g.
        /// to allow the SimPrint output to be checked)
        /// </summary>
        bool bJobStarting = false;
        /// <summary>
        /// Set after we abort a print job and cleared when the Meteor status 
        /// changes to idle.
        /// </summary>
        bool bJobAborting = false;

      
        /// <summary>
        /// Constructor
        /// </summary>
        public FormMeteorMonoPrint() {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Load the data for a new print image
        /// </summary>
        internal void LoadImage() {
            // Attempt to avoid memory allocation problems with large images by
            // cleaning up the currently loading image.  For very large images
            // the 64 bit build of the application should be used.
            if (image != null) {
                pictureBoxPrintData.Image = null;
                image.Dispose();
                image = null;
                pictureBoxPrintData.Refresh();
            }
            GC.Collect();
            // Load the new image
            Cursor.Current = Cursors.WaitCursor;
            string FileName = openFileDialogLoadImage.FileName;
            image = ImageDataFactory.Create(FileName,100000,100000);

            if (image != null) {
                pictureBoxPrintData.Image = image.GetPreviewBitmap();
                Cursor.Current = Cursors.Default;
                toolStripStatusFilename.Text = image.GetBaseName();
                toolStripStatusFilename.ToolTipText = FileName;
                toolStripStatusImageDimensions.Text = string.Format("{0} x {1} pixels", image.GetDocWidth(), image.GetDocHeight());
            } else {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to load image " + openFileDialogLoadImage.FileName,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                openFileDialogLoadImage.FileName = "";
                toolStripStatusFilename.Text = "No image loaded";
                toolStripStatusFilename.ToolTipText = "";
                toolStripStatusImageDimensions.Text = "";
            }
            EnableControls();
        }

        internal int  GetImageHeight()
        {
            return image.GetDocHeight();
        }
        internal int GetImageWidth()
        {
            return image.GetDocWidth();
        }

        internal void LoadImage(string FileName,double printwidth,double printheight)
        {
            // Attempt to avoid memory allocation problems with large images by
            // cleaning up the currently loading image.  For very large images
            // the 64 bit build of the application should be used.
            if (image != null)
            {
                pictureBoxPrintData.Image = null;
                image.Dispose();
                image = null;
                pictureBoxPrintData.Refresh();
            }
            GC.Collect();
            // Load the new image
            Cursor.Current = Cursors.WaitCursor;
            // string FileName = openFileDialogLoadImage.FileName;
            image = ImageDataFactory.Create(FileName,printwidth,printheight);
           
            if (image != null)
            {
                pictureBoxPrintData.Image = image.GetPreviewBitmap();
                Cursor.Current = Cursors.Default;
                toolStripStatusFilename.Text = image.GetBaseName();
                toolStripStatusFilename.ToolTipText = FileName;
                toolStripStatusImageDimensions.Text = string.Format("{0} x {1} pixels", image.GetDocWidth(), image.GetDocHeight());
            }
            else
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to load image " + openFileDialogLoadImage.FileName,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                openFileDialogLoadImage.FileName = "";
                toolStripStatusFilename.Text = "No image loaded";
                toolStripStatusFilename.ToolTipText = "";
                toolStripStatusImageDimensions.Text = "";
            }
            EnableControls();
        }
        /// <summary>
        /// Start a new print job
        /// </summary>
        private void StartPrintJob() {
            if (image == null) {
                return;
            }
            if (!SetupPrinter()) {
                MessageBox.Show("Failed to setup printer");
                return;
            }
            PreLoadPrintJob test = new PreLoadPrintJob(UserBitsPerPixel, image, UserYTop, UserCopies, UserRepeatMode, jobid++);
            // Move status to LOADING if Meteor has hardware connected - refresh the display 
            // immediately because sending the print data to Meteor in PreLoadPrintJob.Start
            // can take a significant amount of time for a large image
            if (latestPrinterStatus == PRINTER_STATUS.IDLE) {
                textBoxStatus.Text = PRINTER_STATUS.LOADING.ToString();
                bJobStarting = true;
                textBoxStatus.Refresh();
            }
            Cursor.Current = Cursors.WaitCursor;
            eRET rVal = test.StartScan();//wimmie aangepast
            Cursor.Current = Cursors.Default;
            if (rVal != eRET.RVAL_OK) {
                string Err = string.Format("Failed to start print job\n\n{0}", rVal);
                MessageBox.Show(Err, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxStatus.Text = latestPrinterStatus.ToString();
            }
            EnableControls();
        }

        PreLoadPrintJob PreLoadedPrintJob;
        public void PreloadPrintJob() //roep deze 1e keer aan
        {
            if (image == null)
            {
                return;
            }
            if (!SetupPrinter())
            {
                MessageBox.Show("Failed to setup printer");
                return;
            }
            PreLoadedPrintJob = new PreLoadPrintJob(UserBitsPerPixel, image, UserYTop, UserCopies, UserRepeatMode, jobid++);
            // Move status to LOADING if Meteor has hardware connected - refresh the display 
            // immediately because sending the print data to Meteor in PreLoadPrintJob.Start
            // can take a significant amount of time for a large image
            if (latestPrinterStatus == PRINTER_STATUS.IDLE)
            {
                textBoxStatus.Text = PRINTER_STATUS.LOADING.ToString();
                bJobStarting = true;
                textBoxStatus.Refresh();
            }
            Cursor.Current = Cursors.WaitCursor;

        }
        public void SetHome()
        {
            eRET rVal = PreLoadedPrintJob.SetHome();
        }

        public void Spit(int number)
        {
            eRET rVal = PreLoadedPrintJob.Spit(number);
        }
        public void StartScanLane(int lane) //roep deze voor elke lane aan
        { 
            eRET rVal = PreLoadedPrintJob.StartScan(lane);//wimmie aangepast
            Cursor.Current = Cursors.Default;
            if (rVal != eRET.RVAL_OK)
            {
                string Err = string.Format("Failed to start print job\n\n{0}", rVal);
                MessageBox.Show(Err, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxStatus.Text = latestPrinterStatus.ToString();
            }
            EnableControls();
        }

        internal void StartScanLane(int lane, int pixelshift)
        {
            eRET rVal = PreLoadedPrintJob.StartScan(lane, pixelshift);//wimmie aangepast
            Cursor.Current = Cursors.Default;
            if (rVal != eRET.RVAL_OK)
            {
                string Err = string.Format("Failed to start print job\n\n{0}", rVal);
                MessageBox.Show(Err, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxStatus.Text = latestPrinterStatus.ToString();
            }
            EnableControls();
        }



        /// <summary>
        /// Set the enabled state of the various controls.  This depends on
        /// whether we are currently connected to Meteor, and whether there
        /// is a print job starting or in progress.
        /// </summary>
        private void EnableControls() {
            bool JobInProgress = latestPrinterStatus == PRINTER_STATUS.READY ||
                                 latestPrinterStatus == PRINTER_STATUS.PRINTING ||
                                 bJobStarting;
            bool ImageLoaded = image != null;
            bool Connected = (latestPrinterStatus != PRINTER_STATUS.DISCONNECTED);

            groupBoxControl.Enabled = Connected;
            groupBoxTemperatures.Enabled = Connected;
            groupBoxSetup.Enabled = Connected && !JobInProgress;
            groupBoxResolution.Enabled = Connected && !JobInProgress;
            groupBoxPrintClock.Enabled = Connected && !JobInProgress;

            buttonStartPrint.Enabled = (image != null) && !JobInProgress;
            buttonLoadImage.Enabled = !JobInProgress;
            buttonStopPrint.Enabled = JobInProgress && !bJobAborting;
            numericUpDownYTop.Enabled = !JobInProgress;

            numericUpDownFrequency.Enabled = radioButtonInternalEncoder.Checked;
        }

        /// <summary>
        /// Helper method for EnableBppRadioButtons.  Called for each of the three bpp radio
        /// buttons in sequence.
        /// </summary>
        /// <param name="enable">Zero to disable, non-zero to enable</param>
        /// <param name="button">Radio button to enable/disable</param>
        /// <param name="firstEnabled">Set with button if this is the first enabled control</param>
        /// <returns>true if the currently set bpp value is now unavailable</returns>
        private static bool RadioButtonCheck(Int32 enable, RadioButton button, ref RadioButton firstEnabled) {
            bool retval = false;
            if (enable != 0) {
                button.Enabled = true;
                if (firstEnabled == null) {
                    firstEnabled = button;
                }
            } else {
                button.Enabled = false;
                if (button.Checked) {
                    retval = true;
                }
            }
            return retval;
        }

        /// <summary>
        /// Act on a change in the valid bits-per-pixel bitmask returned by Meteor, to
        /// (1) enable/disable the appropriate 1,2 or 4bpp radio buttons and (2) make
        /// sure that the currently selected bpp value is valid.  
        /// </summary>
        private void EnableBppRadioButtons() {
            RadioButton firstEnabled = null;
            bool changeSelection = RadioButtonCheck(lastSupportedBppBitmask & 0x02, radioButton1bpp, ref firstEnabled);
            changeSelection |= RadioButtonCheck(lastSupportedBppBitmask & 0x04, radioButton2bpp, ref firstEnabled);
            changeSelection |= RadioButtonCheck(lastSupportedBppBitmask & 0x10, radioButton4bpp, ref firstEnabled);
            if (changeSelection && firstEnabled != null) {
                firstEnabled.Checked = true;
            }
        }
        
        /// <summary>
        /// Set up the printer prior to starting a print job.
        ///
        /// PiSetAndValidateParam blocks until the parameters have been successfully set (or have failed to set
        /// - e.g. if there is an out of range value).  
        /// 
        /// This must be used here in preference to the asynchronous method PiSetParam to guarantee that the
        /// values are set in Meteor before the print job is started.
        ///
        /// </summary>
        /// <returns>Success / failure</returns>
        private bool SetupPrinter() {
            if (PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_PRINT_CLOCK_HZ, UserPrintClock) != eRET.RVAL_OK) {
                return false;
            }
            if (PrinterInterfaceCLS.PiSetAndValidateParam((int)eCFGPARAM.CCP_BITS_PER_PIXEL, UserBitsPerPixel) != eRET.RVAL_OK) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Abort any in-progress print job
        /// </summary>
        private void AbortPrintJob() {
            // No longer starting a job
            bJobStarting = false;
            bJobAborting = true;
            // Update display status and refresh, as the abort can take a few seconds
            buttonStopPrint.Enabled = false;
            buttonStopPrint.Refresh();
            textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            textBoxStatus.Refresh();
            // Send the abort command to Meteor.  This will halt any in-progress
            // print, and clear out all print buffers
            PrinterInterfaceCLS.PiAbort();
            // Wait until the abort has completed
            Cursor.Current = Cursors.WaitCursor;
            status.WaitNotBusy();
            Cursor.Current = Cursors.Default;
        }

        // -- Timers for handling Meteor connection and status --
        #region StatusHandlers
        /// <summary>
        /// Called periodically to (a) connect to Meteor (if the connection is
        /// not already open); (b) retrieve the status from Meteor; (c) update
        /// the temperature set points if required.
        /// </summary>
        private void timerMeteorStatus_Tick(object sender, EventArgs e)
        {
            CreateAVFlexologicIPC();
            AVPrintIPCObject = (AVPrintIPC)AVPrintIPC.thisAVPrintIPC;

            if (AVPrintIPCObject == null)
                IPCStatus.Text = "null";
            else
            {              
                AVPrintIPCObject.HandleTasks(this);
                IPCStatus.Text = AVPrintIPCObject.GetLane().ToString();
            }
            if (inTimer) {
                return;
            }
            inTimer = true;
            try {
                HandleMeteorStatus();
            }
            // If an exception is thrown above, display it to assist trouble shooting
            catch (Exception Exc) {
                string Msg = "Exception thrown:\r\n\r\n" + Exc.ToString();
                MessageBox.Show(Msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            inTimer = false;
        }
        /// <summary>
        /// Create Flexolocic IPC server
        /// </summary>
        private IpcServerChannel FlexologicIPCchannel;
        //   TcpChannel channel;
        public AVPrintIPC AVPrintIPCObject;
   
        private bool CreateAVFlexologicIPC()
        {
            try
            {
                if (FlexologicIPCchannel == null)
                {
                    FlexologicIPCchannel = new IpcServerChannel("FlexologicPrint");
                    ChannelServices.RegisterChannel(FlexologicIPCchannel, true);
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof(AVPrintIPC), "AVPrintIPC", WellKnownObjectMode.Singleton);
                    //  channel = new TcpChannel(8182);
                    //  ChannelServices.RegisterChannel(channel,true);
                    //  RemotingConfiguration.RegisterWellKnownServiceType(typeof(AVPrintIPC), "AVPrintIPC", WellKnownObjectMode.Singleton);
                }
            }
            catch (Exception Exc)
            {
                string Msg = "Exception thrown:\r\n\r\n" + Exc.ToString();
                MessageBox.Show(Msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            return true;
        }


        /// <summary>
        /// Called periodically to handle Meteor initialisation and status
        /// </summary>
        private void HandleMeteorStatus() {
            // Get the Meteor status / open a connection to Meteor if one doesn't 
            // already exist
            PRINTER_STATUS Status = status.GetStatus();
            if (Status == PRINTER_STATUS.READY || 
                Status == PRINTER_STATUS.PRINTING || 
                Status == PRINTER_STATUS.DISCONNECTED) {
                bJobStarting = false;
            } else {
                bJobAborting = false;
            }
            latestPrinterStatus = Status;
            // Update the head/aux temperature setpoints if the values in the inteface
            // have been changed
            if (status.Connected) {
                if (lastSetHeadTemperature != UserHeadTemperature) {
                    // Set target head temperature globally (PiSetParam with pcc = 0, head = 0)
                    if (PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_HEAD_TEMP, UserHeadTemperature) == eRET.RVAL_OK) {
                        lastSetHeadTemperature = UserHeadTemperature;
                    }
                }
                if (lastSetAuxTemperature != UserAuxTemperature) {
                    // Set target auxiliary temperature globally (PiSetParam with pcc = 0, head = 0)
                    if (PrinterInterfaceCLS.PiSetParam((int)eCFGPARAM.CCP_AUX_TEMP, UserAuxTemperature) == eRET.RVAL_OK) {  // Set globally
                        lastSetAuxTemperature = UserAuxTemperature;
                    }
                }
                if (lastSupportedBppBitmask != status.SupportedBppBitmask) {
                    lastSupportedBppBitmask = status.SupportedBppBitmask;
                    EnableBppRadioButtons();
                }
                // Meteor will reject a PiSetHeadPower command if any of the PCCs are still in the process
                // of changing the head power status
                if (checkBoxEnableHeadPower.Enabled != status.HeadPowerIdle) {
                    checkBoxEnableHeadPower.Enabled = status.HeadPowerIdle;
                }
                // If the Meteor head power state is stable, check that we're displaying the correct state
                if (status.HeadPowerIdle) {
                    if (status.HeadPowerEnabled != checkBoxEnableHeadPower.Checked) {
                        inSetHeadPower = true;
                        checkBoxEnableHeadPower.Checked = status.HeadPowerEnabled;
                        inSetHeadPower = false;
                    }
                }
            } else {
                lastSetHeadTemperature = -1;
                lastSetAuxTemperature  = -1;
            }
            // Update the enabled state of the controls to reflect the Meteor status
            EnableControls();
            // Update the status text
            if ( bJobStarting ) {
                textBoxStatus.Text = PRINTER_STATUS.LOADING.ToString();
            } else if ( bJobAborting ) {
                textBoxStatus.Text = PRINTER_STATUS.ABORTING.ToString();
            } else {
                textBoxStatus.Text = Status.ToString();
            }
        }
        #endregion

        // -- Save and load of parameters set by the user --
        #region Settings
        void SaveSettings() {
            try {
                Properties.Settings.Default.YTop = (int)numericUpDownYTop.Value;
                Properties.Settings.Default.PrintFrequency = numericUpDownFrequency.Value;
                Properties.Settings.Default.Copies = numericUpDownCopies.Value;
                Properties.Settings.Default.RepeatMode = (int)UserRepeatMode;
                Properties.Settings.Default.PrintResolution = UserBitsPerPixel;
                Properties.Settings.Default.HeadTemperature = numericUpDownHeadTemperatureSetPoint.Value;
                Properties.Settings.Default.HeadTemperatureEnabled = checkBoxHeadTemperatureControl.Checked;
                Properties.Settings.Default.AuxTemperature = numericUpDownAuxTemperatureSetPoint.Value;
                Properties.Settings.Default.AuxTemperatureEnabled = checkBoxAuxTemperatureControl.Checked;
                Properties.Settings.Default.ImageFileName = openFileDialogLoadImage.FileName;
                Properties.Settings.Default.ExternalEncoder = UserExternalEncoder;
                Properties.Settings.Default.Save();
            }
            catch (Exception e) {
                MessageBox.Show("SaveSettings exception: \r\n" + e.Message);
            }
        }

        void LoadSettings() {
            try {
                numericUpDownYTop.Value = Properties.Settings.Default.YTop;
                numericUpDownFrequency.Value = Properties.Settings.Default.PrintFrequency;
                numericUpDownCopies.Value = Properties.Settings.Default.Copies;
                UserRepeatMode = (REPEAT_MODE)Properties.Settings.Default.RepeatMode;
                UserBitsPerPixel = Properties.Settings.Default.PrintResolution;
                numericUpDownHeadTemperatureSetPoint.Value = Properties.Settings.Default.HeadTemperature;
                checkBoxHeadTemperatureControl.Checked = Properties.Settings.Default.HeadTemperatureEnabled;
                numericUpDownAuxTemperatureSetPoint.Value = Properties.Settings.Default.AuxTemperature;
                checkBoxAuxTemperatureControl.Checked = Properties.Settings.Default.AuxTemperatureEnabled;
                openFileDialogLoadImage.FileName = Properties.Settings.Default.ImageFileName;
                UserExternalEncoder = Properties.Settings.Default.ExternalEncoder;
                if (File.Exists(openFileDialogLoadImage.FileName)) {
                    LoadImage();
                } else {
                    openFileDialogLoadImage.FileName = "";
                }
            }
            catch (Exception) {// Ignore any load exception and fall back on default form values
                               // (can happen if valid ranges in the up/down controls are changed)
            }
        }
        #endregion

        // -- Form properties set by the user --
        #region UserProperties
        /// <summary>
        /// Value for the Meteor CCP_PRINT_CLOCK_HZ parameter.
        /// Zero means use external encoder.  
        /// A non zero value sets the master internal print clock frequency.
        /// </summary>
        private int UserPrintClock {
            get {
                if (radioButtonExternalEncoder.Checked) {
                    return 0;
                } else {
                    return (int)(numericUpDownFrequency.Value * 1000);
                }
            }
        }
        private bool UserExternalEncoder {
            get {
                return radioButtonExternalEncoder.Checked;
            }
            set {
                radioButtonInternalEncoder.Checked = !value;
                radioButtonExternalEncoder.Checked = value;
            }
        }
        private int UserYTop {
            get {
                return (int)(numericUpDownYTop.Value);
            }
        }
        private int UserCopies {
            get {
                return (int)numericUpDownCopies.Value;
            }
        }
        private REPEAT_MODE UserRepeatMode {
            get {
                return radioButtonSeamless.Checked ? REPEAT_MODE.SEAMLESS : REPEAT_MODE.DISCRETE;
            }
            set {
                radioButtonSeamless.Checked = (value == REPEAT_MODE.SEAMLESS);
                radioButtonDiscrete.Checked = (value == REPEAT_MODE.DISCRETE);
            }
        }
        private int UserBitsPerPixel {
            get {
                if (radioButton1bpp.Checked) {
                    return 1;
                } else if (radioButton2bpp.Checked) {
                    return 2;
                } else {
                    return 4;
                }
            }
            set {
                switch (value) {
                    case 1:
                        radioButton1bpp.Checked = true;
                        radioButton2bpp.Checked = false;
                        radioButton4bpp.Checked = false;
                        break;
                    case 2:
                        radioButton1bpp.Checked = false;
                        radioButton2bpp.Checked = true;
                        radioButton4bpp.Checked = false;
                        break;
                    default:
                        radioButton1bpp.Checked = false;
                        radioButton2bpp.Checked = false;
                        radioButton4bpp.Checked = true;
                        break;
                }
            }
        }
        private int UserHeadTemperature {
            get {
                return checkBoxHeadTemperatureControl.Checked ? (int)(numericUpDownHeadTemperatureSetPoint.Value * 10) : 0;
            }
        }
        private int UserAuxTemperature {
            get {
                return checkBoxAuxTemperatureControl.Checked ? (int)(numericUpDownAuxTemperatureSetPoint.Value * 10) : 0;
            }
        }
        #endregion

        // -- Handlers for user control interaction --
        #region UserInteraction
        private void buttonLoadImage_Click(object sender, EventArgs e) {
            if (openFileDialogLoadImage.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                LoadImage();
            }
        }

        private void buttonStartPrint_Click(object sender, EventArgs e) {
            StartPrintJob();
        }

        private void buttonStopPrint_Click(object sender, EventArgs e) {
            AbortPrintJob();
        }

        private void FormMeteorMonoPrint_FormClosing(object sender, FormClosingEventArgs e) {
            SaveSettings();
            status.Disconnect();
            timerMeteorStatus.Enabled = false;
        }

        private void checkBoxHeadTemperatureControl_CheckedChanged(object sender, EventArgs e) {
            numericUpDownHeadTemperatureSetPoint.Enabled = checkBoxHeadTemperatureControl.Checked;
        }

        private void checkBoxAuxTemperatureControl_CheckedChanged(object sender, EventArgs e) {
            numericUpDownAuxTemperatureSetPoint.Enabled = checkBoxAuxTemperatureControl.Checked;
        }

        private void checkBoxEnableHeadPower_CheckedChanged(object sender, EventArgs e) {
            if (inSetHeadPower) {
                return;
            }
            inSetHeadPower = true;
            if (status.Connected) {
                eRET rVal = PrinterInterfaceCLS.PiSetHeadPower(checkBoxEnableHeadPower.Checked ? 1 : 0);
                if (rVal != eRET.RVAL_OK) {
                    MessageBox.Show("PiSetHeadPower failed with " + rVal.ToString() +
                                    "\n\nPlease check that all PCC cards are connected and have completed initialisation",
                                    Application.ProductName,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk);
                    checkBoxEnableHeadPower.Checked = false;
                } else {
                    // Prevent further PiSetHeadPower commands being sent until the status reports HeadPowerIdle
                    checkBoxEnableHeadPower.Enabled = false;
                }
            }
            inSetHeadPower = false;
        }
        #endregion
    }

   
}
