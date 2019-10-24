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
        PDFConvert converter = new PDFConvert();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            AGI = new ArrayGraphicsInterface();
            if (AGI.connected) listBox1.Items.Add("Connected");
           
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
            //throw new NotImplementedException();
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
                AGMessage message = AGI.ReadMessage();
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
                    listBox1.Items.Add(MessageString);
                    if (message.MessageMsgID == 0x06)//error
                    {
                        byte pageprinted = message.MessageData[26];
                        if (pageprinted == 1)
                        {
                            //load new print buffer
                         //   listBox1.Items.Add("refill buffer:" + AGI.CurrentBufferNumber.ToString());
                           // AGI.FillNextPrintBuffer();
                           // Bufferdata.Image = AGI.GetLastBufferDateAsImage();
                        }
                    }
                        
                }                
            }
            catch (Exception err)
            {
                err = null;
            }              
        }

        internal void StartScanLane(int req_printlane, int pixelshift)
        {
           // throw new NotImplementedException();
            AGI.StartPrintLaneNoWait(req_printlane);
        }

        internal void StartScanLane(int req_printlane)
        {
            AGI.StartPrintLaneNoWait(req_printlane);
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
            return AGI.FullBitmapHeight;
        }

        internal void LoadImage(string req_load_ImagePath, double printwidth, double printheight)
        {
            ConvertSinglePDFtoBitmap(req_load_ImagePath);
            AGI.SetFullBitmap = FullBitmap;
        }

        public void CreateLRCrosses(string crossesstring, double printwidth, double printheight)
        {
           // ConvertSinglePDFtoBitmap(req_load_ImagePath);
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
            drawEasyregcrossat(x1, y1, crosssize, FullBitmap);
            drawEasyregcrossat(x2, y2, crosssize, FullBitmap);
            drawEasyregcrossat((x1 + x2) / 2, y1, crosssize, FullBitmap);

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
            for (int i = 0; i < crosssize; i++)
            {
                for (int j = 0; j < crosssize; j++)
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

        private void button5_Click(object sender, EventArgs e)
        {

            AGI.DirectPrintBufferNoWait(listBox1);        
            timer1.Enabled = true;          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x16);
        }
        public string PDFFilename =  "C:\\W8 AVMOM\\W8 AVMOM\\Images\\MarkenValeLabel[2].pdf";
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
            
        }

        /// <summary>Convert a single file</summary>
        /// <remarks>this function PRETEND that the filename is right!</remarks>
        ///
        private Bitmap FullBitmap;
        private void ConvertSinglePDFtoBitmap(string filename)
        {
            bool Converted = false;
            string extension = ".png";
            //Setup the converter
      
                converter.RenderingThreads = -1;  
                converter.TextAlphaBit = -1;
            converter.OutputToMultipleFile = false;
            converter.FirstPageToConvert = -1;
            converter.LastPageToConvert = -1;
            converter.FitPage = false;
            converter.JPEGQuality = (int)10;
            converter.OutputFormat = "png256";
            System.IO.FileInfo input = new FileInfo(filename);
            string output = string.Format("{0}\\{1}{2}", input.Directory, input.Name, extension);
            //If the output file exist alrady be sure to add a random name at the end until is unique!
            while (File.Exists(output))
            {
                output = output.Replace(extension, string.Format("{1}{0}", extension, DateTime.Now.Ticks));
            }
           
            //!!! converteren at png256. Result is 8pp indexed when bitmap is opened.
            converter.ResolutionX = 150;
            converter.ResolutionY = 150;
            Converted = converter.Convert(input.FullName, output);
        //    txtArguments.Text = converter.ParametersUsed;
            if (Converted)
            {
             //   lblInfo.Text = string.Format("{0}:File converted!", DateTime.Now.ToShortTimeString());
             //   txtArguments.ForeColor = Color.Black;
                FullBitmap = new Bitmap(output);
                pictureBox1.Image = FullBitmap;
                Color c = FullBitmap.GetPixel(10, 20);//a test..
               
            }
            else
            {
             //   lblInfo.Text = string.Format("{0}:File NOT converted! Check Args!", DateTime.Now.ToShortTimeString());
             //   txtArguments.ForeColor = Color.Red;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void PrintLane_Click(object sender, EventArgs e)
        {
            AGI.DebugStartPrintLaneNoWait(0);
            Bufferdata.Image = AGI.GetLastBufferDateAsImage();
        }
        int FillBufferFromImageCountBuffNr=0;
        private void button1_Click_1(object sender, EventArgs e)
        {
            AGI.FillBufferFromImage(0, FillBufferFromImageCountBuffNr++);
            Bufferdata.Image = AGI.GetLastBufferDateAsImage();
        }

        private void ResetBuffers_Click(object sender, EventArgs e)
        {
            AGI.SendMessage(0x86);
        }
    }
}
