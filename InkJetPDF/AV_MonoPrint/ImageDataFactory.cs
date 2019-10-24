using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ttp.Meteor.MeteorMonoPrint
{
    /// <summary>
    /// Factory for creating an object that implements the IMeteorImageData interface
    /// from a file path.  The type of object to create is based on the file
    /// extension.
    /// </summary>
    class ImageDataFactory
    {
        public static IMeteorImageData Create(string FileName,double printwidth,double printheight) {
            IMeteorImageData image = null;
            int ExtensionIndex = FileName.LastIndexOf('.');
            if (ExtensionIndex != -1) {
                string FileExt = FileName.Substring(ExtensionIndex + 1).ToLower();
                switch (FileExt) {
                    case "bmp":
                    case "jpg":
                    case "tif":
                        image = new MeteorBitmapImage();
                        break;
                    default:
                        break;
                }
            }
            if (image != null) {
                if ( image.Load(FileName, printwidth, printheight) ) {
                    return image;
                }
            } else {
                MessageBox.Show("Unrecognised file type " + FileName,
                                Application.ProductName, 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
            return null;
        }
    }
}
