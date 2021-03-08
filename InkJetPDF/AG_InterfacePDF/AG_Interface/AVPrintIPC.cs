using AG_Interface;
using System;
using System.Drawing;

namespace W8AVMOM
{
	public class AVPrintIPC : MarshalByRefObject
	{
		public string req_load_ImagePath;
		public RotateFlipType req_flip_Image;
		public string req_create_crosses;
		public int req_printlane;
		public int req_contrast;
		public double req_xcal;
		public int req_programxml;
		public string req_setip = "";
		public string programxmlpath;
		private bool req_home;
		private int req_spit;
		private bool req_initializeprint;
		private bool req_startprint;
		private bool req_deactivate;
		public int printlane;
		public int pixelshift;
		public int FullImageWidth;
		public int FullImageHeight;
		public static object thisAVPrintIPC;
		public double Printwidth;
		public int Crosstype;
		public double Printheight;
		public int WhiteTreshhold;
		public string ConvertedFile { get; set; }
        public string ConvertedFileSmall { get; set; }
		public bool Converting { get; set; }
		public bool Calibrationlines { get; set; }
		public bool PrintheadConnected { get; set; }

		public AVPrintIPC()
		{
			thisAVPrintIPC = this;
			req_printlane = -1;
			req_contrast = -1;
			req_programxml = -1;
			req_load_ImagePath = null;
			req_flip_Image = RotateFlipType.RotateNoneFlipNone;
			SetWhiteLevelPercentage(90);
		}

		public void LoadImage(string fileandpath, double printwidth, double printheight)
		{
			req_load_ImagePath = fileandpath;
			Converting = true;
			Printwidth = printwidth;
			Printheight = printheight;
		}

		public void FlipImage(RotateFlipType type)
		{
			req_flip_Image = type;
			Converting = true;
		}

		public void CreateLRCrosses(string crossesstring, double printwidth, double printheight, int crosstype)
		{
			req_create_crosses = crossesstring;
			Converting = true;
			Printwidth = printwidth;
			Printheight = printheight;
			Crosstype = crosstype;
		}

		public void PrintLane(int lanenr)
		{
			req_printlane = lanenr;
			printlane = lanenr;
			pixelshift = 0;
		}

		public void SetShiftLane(int pixelshiftnr)
		{
			pixelshift = pixelshiftnr;
		}

		public void SetWhiteLevelPercentage(int whiteLevel)
		{
			WhiteTreshhold = (whiteLevel * 3 * 255) / 100;
		}

		public void PrintShiftLane(int lanenr)
		{
			if (lanenr >= 0)
			{
				req_printlane = lanenr;
				printlane = lanenr;
			}
		}
		//values 0..15
		public void SetContrast(int contrast)
		{
			if (contrast >= 0)
			{
				req_contrast = contrast;
			}
		}
		//bigger value is smaller image
		public void SetXCal(double xcal)
		{
			if (xcal >= 0)
			{
				req_xcal = xcal;
			}
		}
		public void ConnectToIP(string ip)
		{
			if (ip.Length > 1)
			{
				req_setip = ip;
			}
		}

		public void ProgramXMLconfig(int xmltypecode, string fullPath)
		{
			if (xmltypecode >= 0)
			{
				req_programxml = xmltypecode;
				programxmlpath = fullPath;
			}
		}

		public void InitializePrint(bool calibrationlines)
		{
			req_initializeprint = true;
			Calibrationlines = calibrationlines;
		}

		public void StartPrint()
		{
			req_startprint = true;
		}

		public void DeactivateHead()
		{
			req_deactivate = true;
		}

		public void Home()
		{
			req_home = true;
		}

		public void Spit(int number)
		{
			req_spit = number;
		}

		public int Status
		{
			get
			{
				PrintheadConnected = true;//TODO get info from main.
				if (PrintheadConnected == true)
					return 1;
				else
					return 0;
			}
			set { }
		}

		public int GetLane()
		{
			return printlane;
		}

		public void HandleTasks(Form1 MeteorMainThread)
		{
			///* todo implement on AGI
			if (req_home)
			{
				MeteorMainThread.SetHome();
				req_home = false;
			}

			if (req_deactivate)
			{
				MeteorMainThread.DeactivateHead();
				req_deactivate = false;
			}

			if (req_initializeprint)
			{
				MeteorMainThread.InitializePrint();
				req_initializeprint = false;
			}

			if (req_startprint)
			{
				MeteorMainThread.StartPrint();
				req_startprint = false;
			}

			if (req_spit != 0)
			{
				MeteorMainThread.Spit();
				req_spit = 0;
			}

			if (req_load_ImagePath != null)
			{
				try
				{
					MeteorMainThread.LoadImage(req_load_ImagePath);
					FullImageWidth = MeteorMainThread.ImageWidth;
					FullImageHeight = MeteorMainThread.ImageHeight;
					Converting = false;
					MeteorMainThread.PreloadPrintJob();
				}
				catch { }//ooops
				req_load_ImagePath = null;
			}

			if (req_flip_Image != RotateFlipType.RotateNoneFlipNone)
			{
				try
				{
					MeteorMainThread.FlipImage(req_flip_Image);
					Converting = false;
				}
				catch { }//ooops
				req_flip_Image = RotateFlipType.RotateNoneFlipNone;
			}

			if (req_create_crosses != null)
			{
				try
				{
					MeteorMainThread.CreateLRCrosses(req_create_crosses, Printwidth, Printheight, Crosstype);
					FullImageWidth = MeteorMainThread.ImageWidth;
					FullImageHeight = MeteorMainThread.ImageHeight;
					MeteorMainThread.PreloadPrintJob();
					Converting = false;
				}
				catch { }//ooops
				req_create_crosses = null;
			}

			if (req_contrast >= 0)
			{
				MeteorMainThread.SetContrast(req_contrast);
				req_contrast = -1;
			}

			if (req_xcal > 0.40)
			{
				MeteorMainThread.SetXCalibration(req_xcal);
				req_xcal = -1.00;
			}

			if (req_programxml >= 0)
			{
				MeteorMainThread.ProgramXML(req_programxml, programxmlpath);
				req_programxml = -1;
			}

			if (req_setip.Length > 1) //connect to specific IP
			{
				MeteorMainThread.SetIP(req_setip);
				req_setip = "";
			}

			if (req_printlane >= 0)
			{
				if (pixelshift == 0)
					MeteorMainThread.StartScanLane(req_printlane, Calibrationlines);
				else
					MeteorMainThread.StartScanLane(req_printlane);
				req_printlane = -1;
			}
		}
	}
}

