using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Ttp.Meteor.MeteorMonoPrint
{
    public enum REPEAT_MODE 
    {
        /// <summary>
        /// Multiple copies of the image will be joined with a zero pixel gap
        /// 
        /// One product detect signal will initiate the printing of all copies of
        /// the image within the print job
        /// </summary>
        SEAMLESS = 0,
        /// <summary>
        /// Each copy of the image will require its own product detect signal
        /// 
        /// The minimum gap between product detects must leave sufficient margin
        /// for the image plus head X-direction span
        /// </summary>
        DISCRETE = 1
    }

    /// <summary>
    /// Sample class to start a pre-load print job comprising a number of repeats of a single
    /// image
    /// 
    /// The repeats are either continuous with zero-pixel gap (which requires a single product
    /// detect to print all copies of the image) or discrete, where a product signal is required
    /// for each copy of the image.
    /// 
    /// In discrete mode there is a minimum gap between image copies of the head Y direction span
    /// 
    /// This example uses the Meteor PCMD_BIGIMAGE command.  This is similar to the standard
    /// PCMD_IMAGE; PCMD_BIGIMAGE must be used if the application needs to pass in more than
    /// 60MB of image data in one command.  
    /// 
    /// The only difference in the parameters is that PCMD_BIGIMAGE includes the image height.
    /// </summary>
    class PreLoadPrintJob
    {
        private int bpp;
        private IMeteorImageData image;
        private int ytop;
        private int copies;
        private REPEAT_MODE repeatmode;
        private int jobid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Bpp">Bits per pixel</param>
        /// <param name="Image">Image to print</param>
        /// <param name="YTop">Y position for the image</param>
        /// <param name="Copies">Number of copies of the image to be printed</param>
        /// <param name="RepeatMode">Repeat images seamlessly (zero gap) or on demand (individual product detects)</param>
        /// <param name="JobID">Meteor job ID</param>
        public PreLoadPrintJob(int Bpp, IMeteorImageData Image, int YTop, int Copies, REPEAT_MODE RepeatMode, int JobID) {
            this.bpp        = Bpp;
            this.image      = Image;
            this.ytop       = YTop;
            this.copies     = Copies;
            this.repeatmode = RepeatMode;
            this.jobid      = JobID;
        }

        /// <summary>
        /// Send the print job to Meteor.  When the method returns, all print data has been
        /// sent to Meteor - however, it has not necessarily all been sent to the hardware.
        /// </summary>
        /// <returns>Success / failure</returns>
        public eRET Start() {
            eRET rVal;
            // Meteor command to start a print job
            int[] StartJobCmd = new int[] { 
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command ID
                4,                              // Number of DWORD parameters
                jobid,                          // Job ID
                (int)eJOBTYPE.JT_PRELOAD,       // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                image.GetDocWidth()+2           // Needed for Left-To-Right printing only
            };

            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that thie never happens in this application.
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartJobCmd)) != eRET.RVAL_OK) {
                return rVal;              
            }
            // The start document command specifies the number of discrete copies
            // of the image which are required
            //
            int[] StartDocCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTPDOC, // Command ID
                1,                              // DWORD parameter count
                repeatmode == (REPEAT_MODE.DISCRETE) ? copies : 1 
            };
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartDocCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            // For seamless image repeats using the prelod data path, PCMD_REPEAT
            // must be sent after PCMD_STARTPDOC and before the image data.
            //
            if (copies > 1 && repeatmode == REPEAT_MODE.SEAMLESS) {
                int[] RepeatCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_REPEAT,    // Command ID
                    1,                              // DWORD parameter cound
                    copies
                };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(RepeatCmd)) != eRET.RVAL_OK) {
                    return rVal;
                }
            }
            // PCMD_BIGIMAGE must be used if the application needs to pass images 
            // which exceed 60MB in size to Meteor as one buffer.  (An alternative
            // is for the application to split up the data into smaller images,
            // each of which can used PCMD_IMAGE).
            //
            // The image data is sent through the Printer Interface to the Meteor
            // Print Engine in chunks.  The application must continually call 
            // PiSendCommand with the same buffer while the Print Engine
            // returns RVAL_FULL.
            //
            // Note that it is necessary to fix the location of the image command
            // in memory while carrying out this sequence, to prevent the garbage
            // collector from relocating the buffer (theoretically possible, but 
            // highly unlikely) between successive PiSendCommand calls.
            //
            int[] ImageCmd = image.GetBigImageCommand(ytop, bpp);
            unsafe {
                fixed (int* pImageCmd = ImageCmd) {
                    do {
                        rVal = PrinterInterfaceCLS.PiSendCommand(ImageCmd);
                    } while (rVal == eRET.RVAL_FULL);
                    if (rVal != eRET.RVAL_OK) {
                        return rVal;
                    }
                }
            }
            int[] EndDocCmd = new int[] {(int)CtrlCmdIds.PCMD_ENDDOC, 0};
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndDocCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            int[] EndJobCmd = new int[] {(int)CtrlCmdIds.PCMD_ENDJOB, 0};
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndJobCmd)) != eRET.RVAL_OK) {
                return rVal;
            }
            return eRET.RVAL_OK;
        }

        //Willem aangapast naar c++ voorbeeld
        /// <summary>
        /// Send the print job to Meteor.  When the method returns, all print data has been
        /// sent to Meteor - however, it has not necessarily all been sent to the hardware.
        /// </summary>
        /// <returns>Success / failure</returns>
        public eRET StartScan()
        {
            eRET rVal;
            int SwathPixelHeight = 320; //uitzoeken waar we deze vandaan moeten halen
            // Meteor command to start a print job
            

            int[] StartJobCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command ID
                4,                              // Number of DWORD parameters
                jobid,                          // Job ID
                (int)eJOBTYPE.JT_SCAN,       // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                image.GetDocWidth()+2           // Needed for Left-To-Right printing only
            };

            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that thie never happens in this application.
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartJobCmd)) != eRET.RVAL_OK)
            {
                return rVal;
            }
            
            // For seamless image repeats using the prelod data path, PCMD_REPEAT
            // must be sent after PCMD_STARTPDOC and before the image data.
            //
            //!!!!!!!!!!!!!!!!!deze niet?
            if (copies > 1 && repeatmode == REPEAT_MODE.SEAMLESS)
            {
                int[] RepeatCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_REPEAT,    // Command ID
                    1,                              // DWORD parameter cound
                    copies
                };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(RepeatCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }

            // The start document command specifies the number of discrete copies
            // of the image which are required
            //
            // Summary:
            //     StartScan command id
            //     Cmd[0] = PCMD_STARTSCAN
            //     Cmd[1] = 1 (DWORD count)
            //     Cmd[2] = Bit[0] Scan direction Ttp.Meteor.eSCANDIR; Bits[31:16] Scan offset (1/100th
            //     print clock - signed)

            int swaths =( image.GetDocHeight() / SwathPixelHeight) + 1;
            for (int sw = 0; sw < 4; sw++)
            {
                int[] StartDocCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTSCAN, // Command ID
                1,                              // DWORD parameter count
                (int) eSCANDIR.SD_FWD
            };
              //  if ((sw & 1) == 1) StartDocCmd[2] = (int)eSCANDIR.SD_REV; //terug =reverse
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
                // PCMD_BIGIMAGE must be used if the application needs to pass images 
                // which exceed 60MB in size to Meteor as one buffer.  (An alternative
                // is for the application to split up the data into smaller images,
                // each of which can used PCMD_IMAGE).
                //
                // The image data is sent through the Printer Interface to the Meteor
                // Print Engine in chunks.  The application must continually call 
                // PiSendCommand with the same buffer while the Print Engine
                // returns RVAL_FULL.
                //
                // Note that it is necessary to fix the location of the image command
                // in memory while carrying out this sequence, to prevent the garbage
                // collector from relocating the buffer (theoretically possible, but 
                // highly unlikely) between successive PiSendCommand calls.
                //
                int[] ImageCmd = image.GetBigImageCommandSwath(ytop, bpp, sw, 320,0);
                unsafe
                {
                    fixed (int* pImageCmd = ImageCmd)
                    {
                        do
                        {
                            rVal = PrinterInterfaceCLS.PiSendCommand(ImageCmd);
                        } while (rVal == eRET.RVAL_FULL);
                        if (rVal != eRET.RVAL_OK)
                        {
                            return rVal;
                        }
                    }
                }
                int[] EndDocCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDDOC, 0 };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }
            int[] EndJobCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDJOB, 0 };
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndJobCmd)) != eRET.RVAL_OK)
            {
                return rVal;
            }
            return eRET.RVAL_OK;
        }

        public eRET SetHome()
        {
            eRET rVal = PrinterInterfaceCLS.PiSetHome();         
            return rVal;
        }


        public eRET Spit(int number)
        {
            int pcc = 1;

            int head = 2;
            eRET rVal;


            rVal = PrinterInterfaceCLS.PiSetSignal((int) ((pcc<<16) + (head<<8) + SigTypes.SIG_SPIT),(int) number);

           // pcc = 2;

           // rVal = PrinterInterfaceCLS.PiSetSignal((int)((pcc << 16) + (head << 8) + SigTypes.SIG_SPIT), (int)1);
            return rVal;
        }


        public eRET StartScan(int lanenr)
        {
            eRET rVal;
            int SwathPixelHeight = 320; //uitzoeken waar we deze vandaan moeten halen
                                        // Meteor command to start a print job


            int[] StartJobCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command ID
                4,                              // Number of DWORD parameters
                jobid,                          // Job ID
                (int)eJOBTYPE.JT_SCAN,       // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                image.GetDocWidth()+2           // Needed for Left-To-Right printing only
            };

            // A start job command can fail if there is an existing print job
            // ready or printing in Meteor, or if a previous print job is still
            // aborting.  The sequencing of the main form's control enables should 
            // guarantee that thie never happens in this application.
            
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartJobCmd)) != eRET.RVAL_OK)
            {
                return rVal;
            }

            // For seamless image repeats using the prelod data path, PCMD_REPEAT
            // must be sent after PCMD_STARTPDOC and before the image data.
            //
            //!!!!!!!!!!!!!!!!!deze niet?
            if (copies > 1 && repeatmode == REPEAT_MODE.SEAMLESS)
            {
                int[] RepeatCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_REPEAT,    // Command ID
                    1,                              // DWORD parameter cound
                    copies
                };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(RepeatCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }

            // The start document command specifies the number of discrete copies
            // of the image which are required
            //
            // Summary:
            //     StartScan command id
            //     Cmd[0] = PCMD_STARTSCAN
            //     Cmd[1] = 1 (DWORD count)
            //     Cmd[2] = Bit[0] Scan direction Ttp.Meteor.eSCANDIR; Bits[31:16] Scan offset (1/100th
            //     print clock - signed)

            int swaths = (image.GetDocHeight() / SwathPixelHeight) + 1;
            int sw = lanenr;

            if(sw>=0)
            {
                int[] StartDocCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTSCAN, // Command ID
                1,                              // DWORD parameter count
                (int) eSCANDIR.SD_FWD
            };
                //  if ((sw & 1) == 1) StartDocCmd[2] = (int)eSCANDIR.SD_REV; //terug =reverse
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
                // PCMD_BIGIMAGE must be used if the application needs to pass images 
                // which exceed 60MB in size to Meteor as one buffer.  (An alternative
                // is for the application to split up the data into smaller images,
                // each of which can used PCMD_IMAGE).
                //
                // The image data is sent through the Printer Interface to the Meteor
                // Print Engine in chunks.  The application must continually call 
                // PiSendCommand with the same buffer while the Print Engine
                // returns RVAL_FULL.
                //
                // Note that it is necessary to fix the location of the image command
                // in memory while carrying out this sequence, to prevent the garbage
                // collector from relocating the buffer (theoretically possible, but 
                // highly unlikely) between successive PiSendCommand calls.
                //
                int[] ImageCmd = image.GetBigImageCommandSwath(ytop, bpp, sw, 320,0);
                unsafe
                {
                    fixed (int* pImageCmd = ImageCmd)
                    {
                        do
                        {
                            rVal = PrinterInterfaceCLS.PiSendCommand(ImageCmd);
                        } while (rVal == eRET.RVAL_FULL);
                        if (rVal != eRET.RVAL_OK)
                        {
                            return rVal;
                        }
                    }
                }
                int[] EndDocCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDDOC, 0 };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }
            int[] EndJobCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDJOB, 0 };
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndJobCmd)) != eRET.RVAL_OK)
            {
                return rVal;
            }
            return eRET.RVAL_OK;
        }


        public eRET StartScan(int lanenr,int pixshift)
        {
            eRET rVal;
            int SwathPixelHeight = 320; //uitzoeken waar we deze vandaan moeten halen
                                        // Meteor command to start a print job
            int[] StartJobCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTJOB,  // Command ID
                4,                              // Number of DWORD parameters
                jobid,                          // Job ID
                (int)eJOBTYPE.JT_SCAN,       // This job uses the preload data path
                (int)eRES.RES_HIGH,             // Print at full resolution
                image.GetDocWidth()+2           // Needed for Left-To-Right printing only
            };

            if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartJobCmd)) != eRET.RVAL_OK) return rVal;
            
            if (copies > 1 && repeatmode == REPEAT_MODE.SEAMLESS)
            {
                int[] RepeatCmd = new int[] {
                    (int)CtrlCmdIds.PCMD_REPEAT,    // Command ID
                    1,                              // DWORD parameter cound
                    copies
                };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(RepeatCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }

          
            int sw = lanenr;

            if (sw >= 0)
            {
                int[] StartDocCmd = new int[] {
                (int)CtrlCmdIds.PCMD_STARTSCAN, // Command ID
                1,                              // DWORD parameter count
                (int) eSCANDIR.SD_FWD
            };
                //  if ((sw & 1) == 1) StartDocCmd[2] = (int)eSCANDIR.SD_REV; //terug =reverse
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(StartDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
                
                int[] ImageCmd = image.GetBigImageCommandSwath(ytop, bpp, sw, 320, pixshift);
                unsafe
                {
                    fixed (int* pImageCmd = ImageCmd)
                    {
                        do
                        {
                            rVal = PrinterInterfaceCLS.PiSendCommand(ImageCmd);
                        } while (rVal == eRET.RVAL_FULL);
                        if (rVal != eRET.RVAL_OK)
                        {
                            return rVal;
                        }
                    }
                }
                int[] EndDocCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDDOC, 0 };
                if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndDocCmd)) != eRET.RVAL_OK)
                {
                    return rVal;
                }
            }
            int[] EndJobCmd = new int[] { (int)CtrlCmdIds.PCMD_ENDJOB, 0 };
            if ((rVal = PrinterInterfaceCLS.PiSendCommand(EndJobCmd)) != eRET.RVAL_OK)
            {
                return rVal;
            }
            return eRET.RVAL_OK;
        }
    }
}
