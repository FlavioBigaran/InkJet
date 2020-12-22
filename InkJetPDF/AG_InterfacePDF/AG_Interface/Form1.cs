using System;
using System.Drawing;
using System.Windows.Forms;
using W8AVMOM;
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
			listBox1.Items.Add("InkJet Compatible to AVMOM V2.7.2.00");
			OpenAGPrinthead_Click(this, null);
		}

		private void OpenAGPrinthead_Click(object sender, EventArgs e)
		{
			AGI = new ArrayGraphicsInterface(listBox1);
			string statusmassage = AGI.connected ? "Inkjet Connected" : "Inkjet Problem; Cannot connect";

			listBox1.Items.Add(statusmassage);
			Text = " " + statusmassage;
			timer1.Enabled = true;
		}

		/// <summary>
		/// Create Flexologic IPC server
		/// </summary>
		private IpcServerChannel FlexologicIPCchannel;
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
				}
			}
			catch (Exception Exc)
			{
				string Msg = "Exception thrown:" + Environment.NewLine + Exc.ToString();
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
							AGI.FillNextPrintBufferRLE(AVPrintIPCObject.Calibrationlines);
							MessageString += " REFILL P1";
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
			catch { }
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
			AGI.InitializePrint();
			AGI.WhiteTreshhold = AVPrintIPCObject.WhiteTreshhold;
		}

		internal void StartScanLane(int req_printlane)
		{
			AGI.UploadLaneNoWait(req_printlane,AVPrintIPCObject.Calibrationlines);
		}

		internal void ProgramXML(int programxmltype, string programxmlpath)
		{
			AGI.ProgramXML(programxmltype, programxmlpath);
		}

		internal void SetIP(string ipadress)
		{
			AGI = new ArrayGraphicsInterface(listBox1, ipadress);
		}

		internal void StartScanLane(int req_printlane,bool cal)
		{
			AGI.UploadLaneNoWait(req_printlane,cal);
		}

		internal void DeactivateHead()
		{
			AGI.DeactivateHead();
		}

		internal void PreloadPrintJob()
		{
			// throw new NotImplementedException();
		}

		internal int ImageHeight { get{ return AGI.FullBitmapHeight; } }

		internal int ImageWidth { get { return AGI.FullBitmapWidth; } }

		internal void LoadImage(string req_load_ImagePath)
		{
			ConvertSinglePDFtoBitmap(req_load_ImagePath);
			AGI.SetFullBitmap = FullBitmap;
		}

		internal void FlipImage(RotateFlipType type)
		{
			string input = "flip";
			int extensioncounter = 0;
			string output = string.Format("{0}\\{1}{2}{3}", conversiondirectory, input, extensioncounter, extension);

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
				output = string.Format("{0}\\{1}{2}{3}", conversiondirectory, input, extensioncounter, extension);
			}
			FileInfo bitmapfilenameinfo = new FileInfo(output);

			if (!bitmapfilenameinfo.Exists)
			{
				FullBitmap.RotateFlip(type);
				AGI.SetFullBitmap = FullBitmap;
				FullBitmap.Save(output);
			}
			if (AVPrintIPCObject != null)
				AVPrintIPCObject.ConvertedFile = output;
			pictureBox1.Image = FullBitmap;
			converter = null;
		}

		public void CreateLRCrosses(string crossesstring, double printwidth, double printheight,int crosstype)
		{
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
				DrawEasyregcrossat(x1, y1, crosssize, FullBitmap);
				DrawEasyregcrossat(x2, y2, crosssize, FullBitmap);
				DrawEasyregcrossat((x1 + x2) / 2, y1, crosssize, FullBitmap);
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

		private void DrawEasyregcrossat(int xm, int ym, int crosssize, Bitmap FullBitmap)
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

		internal void Spit()
		{
		  //  throw new NotImplementedException();
		}

		private void StartBtn_Click(object sender, EventArgs e)
		{
			AGI.SendMessage(0x25);
		}

		private void SystemReadyBtn_Click(object sender, EventArgs e)
		{
			AGI.SendMessage(0x58);
		}

		private void StopBtn_Click(object sender, EventArgs e)
		{
			AGI.SendMessage(0x16);
		}


		public string PDFFilename = "C:\\W8 AVMOM\\W8 AVMOM\\Images\\20001 Flexologic.pdf";
		private void PDFConvert_Click(object sender, EventArgs e)
		{
			#region Check if DLL is present
			//First check if the dll that i use is present!
			if (!System.IO.File.Exists("C:\\Windows\\System32\\gsdll64.dll"))
			{
				//  lblDllInfo.Font = new Font(lblDllInfo.Font.FontFamily, 10, FontStyle.Bold);
				//  lblDllInfo.ForeColor = Color.Red;
				//  txtArguments.Text = "Download: http://mirror.cs.wisc.edu/pub/mirrors/ghost/GPL/gs863/gs863w32.exe";
				MessageBox.Show("The library 'gsdll64.dll' required to run this program is not present! download GhostScript and copy \"gsdll64.dll\" to the folder C:\\Windows\\System32");
				return;
			}
			//Ok now check what version is!

			converter = new PDFConvert();
			GhostScriptRevision version = converter.GetRevision();
			lblVersion.Text = version.intRevision.ToString() + " " + version.intRevisionDate;
			#endregion
			#region Check input of the user
			OpenFileDialog browser = new OpenFileDialog
			{
				DefaultExt = "PDF",
				Filter = "PDF files (*.pdf)|*.PDF"
			};
			DialogResult resultpath = browser.ShowDialog();
			if (resultpath == DialogResult.OK)
			{
				PDFFilename = browser.FileName;
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
				AGI.SetFullBitmap = FullBitmap;
				converter = null;
			}
		}

		/// <summary>Convert a single file</summary>
		/// <remarks>this function PRETEND that the filename is right!</remarks>
		///
		private Bitmap FullBitmap;
		public string conversiondirectory = @"C:\W8 AVMOM\temp";
		private readonly string extension = ".png";
		private void ConvertSinglePDFtoBitmap(string filename)
		{
			//Setup the converter
			converter = new PDFConvert
			{
				RenderingThreads = -1,
				TextAlphaBit = -1,
				OutputToMultipleFile = false,
				FirstPageToConvert = -1,
				LastPageToConvert = -1,
				FitPage = false,
				JPEGQuality = (int)10,
				OutputFormat = "png256"
			};
			FileInfo input = new FileInfo(filename);

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
			FileInfo bitmapfilenameinfo = new FileInfo(output);

			bool Converted;
			if (!bitmapfilenameinfo.Exists)
			{
				//!!! convert at png256. Result is 8pp indexed when bitmap is opened.
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
				FullBitmap.GetPixel(10, 20);//a test..
			}
		}

		private void ResetBuffers_Click(object sender, EventArgs e)
		{
			AGI.SendMessage(0x86);
		}

		internal void StartPrint()
		{
			AGI.StartPrint();
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
