using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using W8AVMOM;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using PdfToImage;
using System.IO;

namespace AG_Interface
{
    public partial class Form1 : Form
    {

        ArrayGraphicsInterface AGI;
        private PDFConvert converter = null;// = new PDFConvert();
        public Form1()
        {
            InitializeComponent();
            AGI = new ArrayGraphicsInterface(listBox1);
            string statusmassage="";
            if (AGI.connected)
             statusmassage="Inkjet Connected";
            else
                statusmassage="Inkjet Problem; Cannot connect";

               listBox1.Items.Add(statusmassage); 
            this.Text = " " + statusmassage;
            timer1.Enabled = true;        
        }

        private void button1_Click(object sender, EventArgs e)
        {

            AGI = new ArrayGraphicsInterface(listBox1);
            string statusmassage = "";
            if (AGI.connected)
            {
                statusmassage = "Inkjet Connected";
            }
            else
                statusmassage = "Inkjet Problem; Cannot connect";
            listBox1.Items.Add(statusmassage);
            this.Text = " " + statusmassage;
            timer1.Enabled = true;                  
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

        internal void SetHome()
        {
            AGI.TriggerPulseGenerator(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //IPC interface to W8AVMOM
            //create the IPC channel if not yet there
            CreateAVFlexologicIPC();
            //seems crazy, let the other end of the ip channel create the AVPrintIPCObject 
            AVPrintIPCObject = (AVPrintIPC)AVPrintIPC.thisAVPrintIPC;

            if (AVPrintIPCObject == null)
                IPCStatus.Text = "null";
            else
            {
                AVPrintIPCObject.HandleTasks(this);
                IPCStatus.Text = AVPrintIPCObject.GetLane().ToString();
            }


            //AGI interface handling
            try
            {
                AGMessage message = null ;
                    
                message = AGI.ReadMessage(AGI.networkStream);
           //     if (message == null) message = AGI.ReadMessage(AGI.networkStreamExtra);

                if(message!=null)
                {
                  
                    string MessageString ="ID:"+ message.MessageMsgID.ToString("X") + ":";
                 

                    if (message.MessageMsgID == 0x34)//error
                    {
                        int n = message.MessageDataLen;
                        if (n > 100) n = 100;
                        for (int i = 0; i < n; i++)
                        {
                            int c = message.MessageData[i];
                            if(c!=0) MessageString += Char.ConvertFromUtf32(message.MessageData[i]);
                        }
                    }
                    else
                    {
                        int n = message.MessageDataLen;
                        if (n > 50) n = 50;
                        for (int i = 0; i < n; i++)
                        {
                            MessageString += message.MessageData[i].ToString("X") + " ";
                        }
                    }
                  
                    if (message.MessageMsgID == 0x06)//next buffer fillable
                    {
                        byte pageprinted = message.MessageData[26];
                        if (pageprinted == 1)
                        {



                            //load new print buffer
                         //   listBox1.Items.Add("refill buffer:" + AGI.CurrentBufferNumber.ToString());
                            AGI.FillNextPrintBufferRLE(AVPrintIPCObject.Calibrationlines);
                            MessageString += " REFILL P1";
                           // Bufferdata.Image = AGI.GetLastBufferDateAsImage();
                        }
                        if (pageprinted == 0)
                        {
                            //2nd prefill line                
                            AGI.FillNextPrintBufferRLE(AVPrintIPCObject.Calibrationlines);
                            MessageString += " REFILL P0";
                        }
                    }
                         
                    listBox1.Items.Add(MessageString);
                }                
            }
            catch (Exception err)
            {
                err = null;
            }              
        }

        internal void SetContrast(int req_contrast)
        {
            AGI.SetContrast((byte)req_contrast);
        }
        internal void SetXCalibration(double xcal)
        {
            AGI.SetXcalibration(xcal);
        }
        
        internal void InitializePrint()
        {
            // throw new NotImplementedException();
            AGI.InitializePrint();
            AGI.WhiteTreshhold = AVPrintIPCObject.WhiteTreshhold;
        }
  
        internal void StartScanLane(int req_printlane, int pixelshift,bool cal)
        {
           // throw new NotImplementedException();
            AGI.UploadLaneNoWait(req_printlane,AVPrintIPCObject.Calibrationlines);
        }

        internal void ProgramXML(int programxmltype, string programxmlpath)
        {
            AGI.ProgramXML(programxmltype, programxmlpath);
        }

        internal void StartScanLane(int req_printlane,bool cal)
        {
            AGI.UploadLaneNoWait(req_printlane,cal);
        }

        internal void DeactivateHead()
        {
            // throw new NotImplementedException();
            AGI.DeactivateHead();
        }
        internal void PreloadPrintJob()
        {
           // throw new NotImplementedException();
        }

        internal int GetImageHeight()
        {
            return AGI.FullBitmapHeight;
        }

        internal int GetImageWidth()
        {
            return AGI.FullBitmapWidth;
        }

        internal void LoadImage(string req_load_ImagePath, double printwidth, double printheight)
        {
            ConvertSinglePDFtoBitmap(req_load_ImagePath);
            AGI.SetFullBitmap = FullBitmap;
        }

        public void CreateLRCrosses(string crossesstring, double printwidth, double printheight,int crosstype)
        {
           // ConvertSinglePDFtoBitmap(req_load_ImagePath);
            bool LRmarks = false;
            int dpixy = 300;
            int pixelsx = (int) (printwidth * dpixy / 25400);
            int pixelsy = (int)(printheight * dpixy / 25400);
            FullBitmap = new Bitmap(pixelsx, pixelsy,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < pixelsx; i++)
                for (int j = 0; j < pixelsy; j++)
                    FullBitmap.SetPixel(i, j, Color.White);
            //now draw the crosses
            
            int crosssize=75;
            int x1=crosssize;
            int y1= pixelsy/2;
            int x2=pixelsx-crosssize;
            int y2= pixelsy/2;
            if (crosstype == 1)
            {
                drawEasyregcrossat(x1, y1, crosssize, FullBitmap);
                drawEasyregcrossat(x2, y2, crosssize, FullBitmap);
                drawEasyregcrossat((x1 + x2) / 2, y1, crosssize, FullBitmap);
            }
            if (LRmarks)
            {
                for (int i = 0; i < pixelsy; i++)
                {
                    FullBitmap.SetPixel(3, i, Color.Black);
                    FullBitmap.SetPixel(4, i, Color.Black);
                    FullBitmap.SetPixel(5, i, Color.Black);
                    FullBitmap.SetPixel(pixelsx - 1, i, Color.Black);
                    FullBitmap.SetPixel(pixelsx - 2, i, Color.Black);
                    FullBitmap.SetPixel(pixelsx - 3, i, Color.Black);
                }
            }
            Graphics g = Graphics.FromImage(FullBitmap);
            //Rectangle printrect = new Rectangle(x1 + (crosssize * 4), y2 - (crosssize), crosssize * 10, crosssize*2);
            //test if this is mirrored
            Rectangle printrect = new Rectangle(x1 + (crosssize * 4), y2 - (crosssize), crosssize * 10, crosssize * 2);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.DrawString(crossesstring,new Font("Tahoma",crosssize),Brushes.Black,printrect);
            g.Flush();


            AGI.SetFullBitmap = FullBitmap;
            pictureBox1.Image = FullBitmap;
        }

        private void drawcrossat(int xm, int ym,int crosssize, Bitmap FullBitmap)
        {
            for (int i = 0; i < crosssize; i++)
            {
                FullBitmap.SetPixel(xm + i, ym,Color.Black);
                FullBitmap.SetPixel(xm - i, ym, Color.Black);
                FullBitmap.SetPixel(xm, ym + i, Color.Black);
                FullBitmap.SetPixel(xm, ym - i, Color.Black);
                FullBitmap.SetPixel(xm - 1, ym + i, Color.Black);
                FullBitmap.SetPixel(xm - 1, ym - i, Color.Black);
                FullBitmap.SetPixel(xm + 1, ym + i, Color.Black);
                FullBitmap.SetPixel(xm + 1, ym - i, Color.Black);
            }
        }
        private void drawEasyregcrossat(int xm, int ym, int crosssize, Bitmap FullBitmap)
        {
            for (int i = 1; i < crosssize; i++)
            {
                for (int j = 1; j < crosssize; j++)
                {
                    FullBitmap.SetPixel(xm + i, ym + j, Color.Black);
                    FullBitmap.SetPixel(xm - i, ym - j, Color.Black);
                   
                }
            }
        }

        internal void Spit(int req_spit)
        {
          //  throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x25);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x58);
        }

      
        private void button3_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x16);
        }


        public string PDFFilename = "C:\\W8 AVMOM\\W8 AVMOM\\Images\\20001 Flexologic.pdf";
        private void PDFConvert_Click(object sender, EventArgs e)
   
        {
            #region Check if DLL is present
            //First check if the dll that i use is present!
            if (!System.IO.File.Exists(Application.StartupPath + "\\gsdll64.dll"))
            {
              //  lblDllInfo.Font = new Font(lblDllInfo.Font.FontFamily, 10, FontStyle.Bold);
              //  lblDllInfo.ForeColor = Color.Red;
              //  txtArguments.Text = "Download: http://mirror.cs.wisc.edu/pub/mirrors/ghost/GPL/gs863/gs863w32.exe";
                MessageBox.Show("The library 'gsdll64.dll' required to run this program is not present! download GhostScript and copy \"gsdll64.dll\" to this program directory");
                return;
            }
            //Ok now check what version is!

            converter = new PDFConvert();
            GhostScriptRevision version = converter.GetRevision();
            lblVersion.Text = version.intRevision.ToString() + " " + version.intRevisionDate;
            #endregion
            if (true)
            {

                #region Check input of the user
                if (string.IsNullOrEmpty(PDFFilename))
                {
                    MessageBox.Show("Insert a filename!");
                    return;
                }
                if (!File.Exists(PDFFilename))
                {
                    MessageBox.Show("The file can't be found");
                    return;
                }
                #endregion
                //Convert the file
                ConvertSinglePDFtoBitmap(PDFFilename);
                AGI.SetFullBitmap=FullBitmap;
            }
            converter = null;
        }

        /// <summary>Convert a single file</summary>
        /// <remarks>this function PRETEND that the filename is right!</remarks>
        ///
        private Bitmap FullBitmap;
        public string bitmapfilename=@"C:\W8 AVMOM\W8 AVMOM\images\convertedpng.bmp";
        public string conversiondirectory = @"C:\W8 AVMOM\tempconvert";
        private void ConvertSinglePDFtoBitmap(string filename)
        {
            bool Converted = false;
            string extension = ".png";
            //Setup the converter
            converter = new PDFConvert();
            converter.RenderingThreads = -1;  
            converter.TextAlphaBit = -1;
            converter.OutputToMultipleFile = false;
            converter.FirstPageToConvert = -1;
            converter.LastPageToConvert = -1;
            converter.FitPage = false;
            converter.JPEGQuality = (int)10;
            converter.OutputFormat = "png256";
            System.IO.FileInfo input = new FileInfo(filename);

            int extensioncounter = 0;
            string output = string.Format("{0}\\{1}{2}{3}", conversiondirectory, input.Name, extensioncounter, extension);
        
            while (File.Exists(output))
                {
                try
                {
                    File.Delete(output);//throw away for next time.
                }
                catch
                {
                    //not allowed for some reason
                }            
                extensioncounter++; //do not use the just deleted file
                output = string.Format("{0}\\{1}{2}{3}", conversiondirectory, input.Name, extensioncounter, extension);
            }
            System.IO.FileInfo bitmapfilenameinfo = new FileInfo(output);

            if (!bitmapfilenameinfo.Exists)
            {       
                //!!! converteren at png256. Result is 8pp indexed when bitmap is opened.
                converter.ResolutionX = 300;//190.5
                converter.ResolutionY = 300;
                Converted = converter.Convert(input.FullName, output);
                
            }
            else
            {
                Converted = true;
            }
            converter = null;

            if (Converted)
            {
                if(AVPrintIPCObject!=null) AVPrintIPCObject.ConvertedFile = output;
                FullBitmap = new Bitmap(output);
                pictureBox1.Image = FullBitmap;
                Color c = FullBitmap.GetPixel(10, 20);//a test..
               
            }
            else
            {
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        int FillBufferFromImageCountBuffNr=0;
        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }

        private void ResetBuffers_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x86);
        }

        internal void StartPrint()
        {
            AGI.StartPrint();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void SetContrastBtn_Click(object sender, EventArgs e)
        {
            byte contrast = (byte) ContrastValueUpDown.Value;
            AGI.SetContrast(contrast);
        }

        private void TriggerGenerator0_Click(object sender, EventArgs e)
        {
            AGI.TriggerPulseGenerator(0);
        }

        private void TriggerGenerator1_Click(object sender, EventArgs e)
        {
            AGI.TriggerPulseGenerator(1);
        }
    }
}
