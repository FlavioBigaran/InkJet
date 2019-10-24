using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ttp.Meteor.MeteorMonoPrint
{
    /// <summary>
    /// Object which loads a .bmp file and converts it into an image command for Meteor
    /// </summary>
    public class MeteorBitmapImage : IMeteorImageData
    {
        /// <summary>
        /// Data loaded from the bitmap file
        /// </summary>
        private Bitmap bitmap;
        /// <summary>
        /// Name of the source bitmap file with the path removed
        /// </summary>
        private string basename;

        /// <summary>
        /// Constructor
        /// </summary>
        public MeteorBitmapImage() {
        }

        public double org_dpi_x;
        public double org_dpi_y;
        public double target_x_DPI;
        public double target_y_DPI;
        public double xscale;
        public double yscale;
        public int orgwidht;
        public int orgHeight;
        public int targetwidth;
        public int targetheight;
        public double xstep;
        public double ystep;
        public double platemmX = 900;
        public double platemmY = 600;
        #region IMeterImageData
        public bool Load(string Path,double printwidth,double printheight) {
            try {
               
                bitmap = new Bitmap(Path);
                platemmX = printwidth / 1000;
                platemmY = printheight / 1000;
                org_dpi_x = bitmap.HorizontalResolution;
                 org_dpi_y = bitmap.VerticalResolution;
                 target_x_DPI = 600;
                 target_y_DPI = 300;
                 double targetpixelsx = platemmX * target_x_DPI / 25.4;
                 double targetpixelsy = platemmY * target_y_DPI / 25.4;
                  orgwidht = bitmap.Width;
                 orgHeight = bitmap.Height;

                xscale = targetpixelsx / orgwidht;
                yscale = targetpixelsy / orgHeight;

                // xscale = target_x_DPI / org_dpi_x;
                //  yscale = target_y_DPI / org_dpi_y;
               
                 targetwidth = (int)(xscale * orgwidht);
                 targetheight = (int)(yscale * orgHeight);
                 xstep = 1.0 / xscale;
                 ystep = 1.0 / yscale;

                /*   //  Bitmap bm1 =(Bitmap)  Image.FromFile(Path);    // this locks the file
                   bitmap = new Bitmap((int)xscale * orgwidht, (int)yscale * orgHeight, PixelFormat.Format1bppIndexed);  // the output image
                   Graphics g = Graphics.FromImage(bitmap);      // so we can draw on bitmap
                   g.ScaleTransform((float)xscale, (float)yscale);                // some transformation
                   g.DrawImage((Image)org_bitmap, new Rectangle(0, 0, (int)xscale * orgwidht, (int)yscale * orgHeight));
                   org_bitmap.Dispose();                             // unlocks the file
                   bitmap.Save("tempconvertfile.bmp", ImageFormat.Bmp);           // save output image to same or other file
                   g.Dispose();
                   */
              //  bitmap = new Bitmap(org_bitmap, (int) (org_bitmap.Width * target_x_DPI / org_dpi_x) , (int) (org_bitmap.Width * target_y_DPI / org_dpi_y));
                /*

                

                bitmap = new Bitmap(targetwidth, targetheight, PixelFormat.Format8bppIndexed);  // the output image
                double currenty = 0;
                for (int j=0;j<targetheight;j++)
                {                  
                    double currentx = 0;
                    for (int i = 0; i < targetwidth; i++)                       
                    {
                        currentx += xstep;
                        bitmap.SetPixel(i, j, org_bitmap.GetPixel((int)currentx,(int) currenty));
                    }
                    currenty += ystep;
                }
                */
                basename = Path.Substring(Path.LastIndexOf('\\') + 1);
                return true;
            }
            catch (Exception e) {
                MessageBox.Show("Failed to open file " + Path + "\r\n\r\n" + e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
        
            }
        }

        public int[] GetBigImageCommand(int yTop, int trueBpp)
        {
            return GetImageCommand(yTop, trueBpp, true);
        }

        public int[] GetBigImageCommandSwath(int yTop, int trueBpp,int SwathNr,int SwathHeight,int pixshift)
        {
            return GetImageCommandSwath(yTop, trueBpp, true,  SwathNr,  SwathHeight, pixshift);
        }

        public string GetBaseName()         { return basename; }
        public Bitmap GetPreviewBitmap()    { return bitmap; }
        public int GetDocWidth()            { return bitmap.Width; } 
        public int GetDocHeight()           { return bitmap.Height; }
        #endregion

        /// <summary>
        /// Allocates and fills an image command buffer to be sent to Meteor via
        /// PiSendCommand.  The image data in the buffer comes from the previously
        /// loaded bitmap.
        /// 
        /// The standard command for sending image data to Meteor is PCMD_IMAGE.
        /// If a large image (> 60MB) needs to be sent in one command then
        /// PCMD_BIGIMAGE command must be used.
        /// </summary>
        /// <param name="yTop">Y position in pixels of the image</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <param name="useBigImage">Use the standard PCMD_IMAGE or PCMD_BIGIMAGE</param>
        /// <returns>Image command to send to Meteor.  null if memory allocation fails</returns>
        private int[] GetImageCommand(int yTop, int trueBpp, bool useBigImage) {
            // Meteor sends print data to the hardware as 1,2 or 4bpp.
            // Some heads accept 3bpp data; some accept 4bpp.
            // For 3bpp heads we need to use the least significant 3 bits in the 4bpp data.
            int outbpp = (trueBpp == 3) ? 4 : trueBpp;
            // Image dimensions
            int width = bitmap.Width;
            int height = bitmap.Height;
            // The width of the image data buffer sent to Meteor must be a multiple
            // of DWORDs
            int bufwidthDWORDs = (((width * outbpp) + 31) >> 5);
            // Meteor image buffer size in DWORDs
            int isize = bufwidthDWORDs * height;
            // Header size is different for PCMD_IMAGE and PCMD_BIGIMAGE
            int hdrsize = useBigImage ? 5 : 4;

            // Allocate memory for image + header.  Note that this buffer will be
            // initialised to zero by the framework.
            int[] buff = new int[isize + 7];
            if(null == buff) {
                return null;
            }
            // Fill in the command header
            buff[0] = useBigImage ? (int)CtrlCmdIds.PCMD_BIGIMAGE 
                                  : (int)CtrlCmdIds.PCMD_IMAGE;   // Command
            buff[1] = isize + hdrsize;              // Dword count
            buff[2] = 1;                            // Plane
            buff[3] = 1;                            // Xleft
            buff[4] = yTop;                         // Ytop
            buff[5] = targetwidth;                        // Width
            if (useBigImage) {
                buff[6] = height;                   // Height (PCMD_BIGIMAGE only)
            }
            int dp = 2 + hdrsize;                   // Index of first data DWORD

            // Copy the image data line by line from the bitmap and write it into the Meteor 
            // command buffer at the requested resolution
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int srcstrideBytes = bmpData.Stride;
            byte[] lineData = new byte[4 * bufwidthDWORDs * (32 / outbpp)]; // 32 bits per pixel for the maximum Meteor buffer line length
                                                                            // to protect against falling off the end of actual pixel data
                                                                            // in the below loop
            for(int y = 0; y < height; y++) {



                Marshal.Copy(new IntPtr(bmpData.Scan0.ToInt64() + y * srcstrideBytes), lineData, 0, srcstrideBytes);
                if(trueBpp == 1) {
                    for(int x = 0; x < targetwidth; x += 4) {
                        int byteoff = x >> 2;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, (int) (x * 4* xstep));
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 1 msbs

                        // Get second source pixel
                        byte b2 = ArgbToGrey(lineData, (int)((x + 1) * 4 * xstep));
                        b2 ^= 0xFF;     // Invert for printer
                        b2 >>= 5;       // Only interested in 1 msbs

                        // Get third source pixel
                        byte b3 = ArgbToGrey(lineData, (int)((x + 2) * 4 * xstep));
                        b3 ^= 0xFF;     // Invert for printer
                        b3 >>= 6;       // Only interested in 1 msbs

                        // Get fourth source pixel
                        byte b4 = ArgbToGrey(lineData, (int)((x + 3) * 4 * xstep));
                        b4 ^= 0xFF;     // Invert for printer
                        b4 >>= 7;       // Only interested in 1 msbs

                        b = (byte)((b & 0x08) | (b2 & 0x4) | (b3 & 0x02) | (b4 & 0x01));    // Align pixels

                        buff[dp] |= ((int)b) << shift;          // Write both pixels

                        if((x >= width - 4) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                } else if(trueBpp == 2) {
                    int lbuff = bmpData.Stride * y;
                    for(int x = 0; x < width; x += 2) {
                        int byteoff = x >> 1;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 2 msbs

                        // Get second source pixel
                        byte b2 = ArgbToGrey(lineData, (x + 1) * 4);
                        b2 ^= 0xFF;     // Invert for printer
                        b2 >>= 6;       // Only interested in 2 msbs

                        b = (byte)((b & 0x0C) | (b2 & 0x3));    // Align pixels
                        buff[dp] |= ((int)b) << shift;          // Write both pixels
                        if((x >= width - 2) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                } else {	// trueBpp ==  3 or 4
                    byte bppMask = (byte)((1 << trueBpp) - 1);  // Make to suit the true bpp
                    int lbuff = bmpData.Stride * y;
                    for(int x = 0; x < width; x += 1) {
                        int byteoff = x;
                        int shift = 4 * (7 - (byteoff & 7));	// Bit count to shift left

                        // Get first source pixel
                        byte b = ArgbToGrey(lineData, x * 4);
                        b ^= 0xFF;      // Invert for printer
                        b >>= 4;        // Only interested in 4 msbs

                        if (trueBpp == 3) { //we are dealing with a smaller amount of bits at the head
                            b >>= 1;
                        }

                        b &= bppMask;      // Mask upper nibble
                        buff[dp] |= ((int)b) << shift;          // Write both pixels
                        if((x >= width - 1) || ((byteoff & 7) == 7)) {
                            dp++;
                        }
                    }
                }
            }
            bitmap.UnlockBits(bmpData);
            return buff;
        }
        private int[] GetImageCommandSwath(int yTop, int trueBpp, bool useBigImage, int swathNr, int swathHeight,int shiftpixels)
        {
            //Normal scan, but with swatch (scan line) selection
            // Meteor sends print data to the hardware as 1,2 or 4bpp.
            // Some heads accept 3bpp data; some accept 4bpp.
            // For 3bpp heads we need to use the least significant 3 bits in the 4bpp data.
            int outbpp = (trueBpp == 3) ? 4 : trueBpp;
            // Image dimensions
            int width = bitmap.Width;
            int height = bitmap.Height;
            // The width of the image data buffer sent to Meteor must be a multiple
            // of DWORDs
            int bufwidthDWORDs = (((targetwidth * outbpp) + 31) >> 5);
            // Meteor image buffer size in DWORDs
            int isize = bufwidthDWORDs * swathHeight;
            // Header size is different for PCMD_IMAGE and PCMD_BIGIMAGE
            int hdrsize = useBigImage ? 5 : 4;

            // Allocate memory for image + header.  Note that this buffer will be
            // initialised to zero by the framework.
            int[] buff = new int[isize + 7];
            if (null == buff)
            {
                return null;
            }
            // Fill in the command header
            buff[0] = useBigImage ? (int)CtrlCmdIds.PCMD_BIGIMAGE
                                  : (int)CtrlCmdIds.PCMD_IMAGE;   // Command
            buff[1] = isize + hdrsize;              // Dword count
            buff[2] = 1;                            // Plane
            buff[3] = 1;                            // Xleft
            buff[4] = yTop;                         // Ytop
            buff[5] = targetwidth;                        // Width
            if (useBigImage)
            {
                buff[6] = swathHeight;                   // Height (PCMD_BIGIMAGE only)
            }
            int dp = 2 + hdrsize;                   // Index of first data DWORD

            // Copy the image data line by line from the bitmap and write it into the Meteor 
            // command buffer at the requested resolution
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);//alleen swath locken?
            int srcstrideBytes = bmpData.Stride;
            //    byte[] lineData = new byte[4 * bufwidthDWORDs * (32 / outbpp)+100]; // 32 bits per pixel for the maximum Meteor buffer line length
            byte[] lineData = new byte[srcstrideBytes + 100];                                                         // to protect against falling off the end of actual pixel data
                                                                            // in the below loop
            for (int y = 0; y < swathHeight; y++)//Just one swath
            {
                int scanline =(int)( ystep * (double) (y + shiftpixels + (swathNr * swathHeight)));
                if ((scanline < height) && (scanline >= 0))
                    {
                    Marshal.Copy(new IntPtr(bmpData.Scan0.ToInt64() + scanline * srcstrideBytes), lineData, 0, srcstrideBytes);
                    if (trueBpp == 1)
                    {
                        for (int x = 0; x < targetwidth; x += 4)
                        {
                            int byteoff = x >> 2;
                            int shift = 4 * (7 - (byteoff & 7));    // Bit count to shift left
                            int xscaled =(int)( (double)x * xstep);

                            // Get first source pixel
                            byte b = ArgbToGrey(lineData, (int)((xscaled + 0) * 4 ));
                            b ^= 0xFF;      // Invert for printer
                            b >>= 4;        // Only interested in 1 msbs

                            // Get second source pixel
                            byte b2 = ArgbToGrey(lineData, (int)((xscaled + 1) * 4));
                            b2 ^= 0xFF;     // Invert for printer
                            b2 >>= 5;       // Only interested in 1 msbs

                            // Get third source pixel
                            byte b3 = ArgbToGrey(lineData, (int)((xscaled + 2) * 4));
                            b3 ^= 0xFF;     // Invert for printer
                            b3 >>= 6;       // Only interested in 1 msbs

                            // Get fourth source pixel
                            byte b4 = ArgbToGrey(lineData, (int)((xscaled + 3) * 4));
                            b4 ^= 0xFF;     // Invert for printer
                            b4 >>= 7;       // Only interested in 1 msbs

                            b = (byte)((b & 0x08) | (b2 & 0x4) | (b3 & 0x02) | (b4 & 0x01));    // Align pixels

                            buff[dp] |= ((int)b) << shift;          // Write both pixels

                            if ((x >= targetwidth - 4) || ((byteoff & 7) == 7))
                            {
                                dp++;
                            }
                        }
                    }
                    else if (trueBpp == 2)
                    {
                        int lbuff = bmpData.Stride * y;
                        for (int x = 0; x < width; x += 2)
                        {
                            int byteoff = x >> 1;
                            int shift = 4 * (7 - (byteoff & 7));    // Bit count to shift left

                            // Get first source pixel
                            byte b = ArgbToGrey(lineData, x * 4);
                            b ^= 0xFF;      // Invert for printer
                            b >>= 4;        // Only interested in 2 msbs

                            // Get second source pixel
                            byte b2 = ArgbToGrey(lineData, (x + 1) * 4);
                            b2 ^= 0xFF;     // Invert for printer
                            b2 >>= 6;       // Only interested in 2 msbs

                            b = (byte)((b & 0x0C) | (b2 & 0x3));    // Align pixels
                            buff[dp] |= ((int)b) << shift;          // Write both pixels
                            if ((x >= width - 2) || ((byteoff & 7) == 7))
                            {
                                dp++;
                            }
                        }
                    }
                    else
                    {   // trueBpp ==  3 or 4
                        byte bppMask = (byte)((1 << trueBpp) - 1);  // Make to suit the true bpp
                        int lbuff = bmpData.Stride * y;
                        for (int x = 0; x < width; x += 1)
                        {
                            int byteoff = x;
                            int shift = 4 * (7 - (byteoff & 7));    // Bit count to shift left

                            // Get first source pixel
                            byte b = ArgbToGrey(lineData, x * 4);
                            b ^= 0xFF;      // Invert for printer
                            b >>= 4;        // Only interested in 4 msbs

                            if (trueBpp == 3)
                            { //we are dealing with a smaller amount of bits at the head
                                b >>= 1;
                            }

                            b &= bppMask;      // Mask upper nibble
                            buff[dp] |= ((int)b) << shift;          // Write both pixels
                            if ((x >= width - 1) || ((byteoff & 7) == 7))
                            {
                                dp++;
                            }
                        }
                    }
                }
            }
            bitmap.UnlockBits(bmpData);
            return buff;
        }
        /// <summary>
        /// Converts an ARGB value to greyscale
        /// </summary>
        /// <param name="image">Byte array of pixel data</param>
        /// <param name="offset">Byte offset of ARGB value</param>
        /// <returns>8-bit greyscale value</returns>
        private byte ArgbToGrey(byte[] image, int offset) {
            return (byte)((float)image[offset + 2] * 0.3 + (float)image[offset + 1] * 0.59 + (float)image[offset + 0] * 0.11);
        }

        #region IDisposable
        public void Dispose()
        {
            if (bitmap != null) { bitmap.Dispose(); }
        }
        #endregion
    }
}
