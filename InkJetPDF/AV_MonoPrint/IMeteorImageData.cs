using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Ttp.Meteor.MeteorMonoPrint
{
    /// <summary>
    /// Interface hiding the details of a file type when using the PreLoadPrintJob
    /// and related form object
    /// </summary>
    interface IMeteorImageData : IDisposable
    {
        /// <summary>
        /// Load the image file
        /// </summary>
        /// <param name="Path">Full path to the source image file</param>
        /// <returns>success(true) or failure(false)</returns>
        bool Load(string Path,double printwidth,double printheight);
        /// <summary>
        /// Return a formatted command buffer to send the image to Meteor using
        /// PrinterInterfaceCLS.PiSendCommand.
        /// </summary>
        /// <param name="yTop">Y position if the image in pixels</param>
        /// <param name="trueBpp">Bits per pixel</param>
        /// <returns>Image command buffer or null if memory allocation fails</returns>
        int[] GetBigImageCommand(int yTop, int trueBpp);
        int[] GetBigImageCommandSwath(int yTop, int trueBppm,int SwathNr,int SwathHeigh,int pixshift);
        /// <summary>
        /// Base name for display as determined from the image filename
        /// </summary>
        string GetBaseName();
        /// <summary>
        /// Gets the framework bitmap object to use for a preview display of the image
        /// </summary>
        Bitmap GetPreviewBitmap();
        /// <summary>
        /// Width of the image in pixels.  
        /// Note that this does **not** include any stride padding which may be necessary
        /// when the image data is sent to Meteor, where each line must be a whole number
        /// of DWORDs.
        /// </summary>
        int GetDocWidth();
        /// <summary>
        /// Height of the image in pixels.
        int GetDocHeight();
    }
}
