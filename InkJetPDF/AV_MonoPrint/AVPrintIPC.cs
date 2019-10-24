using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Ttp.Meteor.MeteorMonoPrint;

namespace W8AVMOM
{

    public class AVPrintIPC : MarshalByRefObject
    {
       
        public  string req_load_ImagePath;
        public int req_printlane;
        private bool req_home;
        private int req_spit;
        public int printlane;
        public int pixelshift;

        public Bitmap PreviewImage;
        public int FullImageWidth;
        public int FullImageHeight;
        public static object thisAVPrintIPC;
        public double Printwidth;
        public double Printheight;

        public AVPrintIPC()
        {
            thisAVPrintIPC = this;
            req_printlane = -1;
            req_load_ImagePath = null;
        }

        public void LoadImage(string fileandpath,double printwidth,double printheight)
        {
            req_load_ImagePath = fileandpath;
            Printwidth = printwidth;
            Printheight = printheight;
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

        public void PrintShiftLane(int lanenr)
        {
            if (lanenr >= 0)
            {
                req_printlane = lanenr;
                printlane = lanenr;            
            }
        }

        public void Home()
        {
            req_home = true;
        }
        public void Spit(int number )
        {
            req_spit = number;
        }
        public int Status { get
            { return 1; }
            set { } }
    
        public int GetLane()
        {
            return printlane;
        }

        public Bitmap GetPreviewImage()
        {
            return PreviewImage;
        }


        public void HandleTasks(FormMeteorMonoPrint MeteorMainThread)
        {

            if (req_home)
            {
                MeteorMainThread.SetHome();
                req_home = false;
            }


            if (req_spit!=0)
            {
                MeteorMainThread.Spit(req_spit);
                req_spit = 0;
            }

            if (req_load_ImagePath != null)
            {
                try
                {
                    MeteorMainThread.LoadImage(req_load_ImagePath, Printwidth, Printheight);
                    PreviewImage = new Bitmap(MeteorMainThread.pictureBoxPrintData.Image);
                    FullImageWidth = MeteorMainThread.GetImageWidth();
                    FullImageHeight = MeteorMainThread.GetImageHeight();
                    MeteorMainThread.PreloadPrintJob();
                }
                catch
                { }//ooops

                req_load_ImagePath = null;
            }

            if (req_printlane >= 0)
            {
                if(pixelshift == 0)
                    MeteorMainThread.StartScanLane(req_printlane);
                else
                    MeteorMainThread.StartScanLane(req_printlane,pixelshift);
                req_printlane = -1;
            }
        }
        
    }

}

