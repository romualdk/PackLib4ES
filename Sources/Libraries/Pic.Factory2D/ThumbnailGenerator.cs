#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using DesLib4NET;

using log4net;
#endregion

namespace Pic.Factory2D
{
    public class ThumbnailGenerator
    {
        #region Static methods
        public static void GenerateImage(Size size, PicFactory factory, bool showCotations, out Image bmp)
        {
            // get bounding box
            Box2D box = Tools.BoundingBox(factory, 0.05);
            // draw image
            PicGraphicsImage picImage = new PicGraphicsImage(size, box);
            factory.Draw(picImage, showCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation);

            bmp = picImage.Bitmap;            
        }

        public static void GenerateImage(Size size, string filePath, string thumbnailFilePath)
        {
            // load file
            PicFactory factory = new PicFactory();
            string fileExt = Path.GetExtension(filePath);
            if (string.Equals(".des", fileExt, StringComparison.CurrentCultureIgnoreCase))
            {
                PicLoaderDes picLoaderDes = new PicLoaderDes(factory);
                using (DES_FileReader fileReader = new DES_FileReader())
                    fileReader.ReadFile(filePath, picLoaderDes);
            }
            else if (string.Equals(".dxf", fileExt, StringComparison.CurrentCultureIgnoreCase))
            {
                using (PicLoaderDXF picLoaderDxf = new PicLoaderDXF(factory))
                    picLoaderDxf.Load(filePath);
            }
            // draw image
            GenerateImage(size, factory, false, out Image bmp);
            bmp.Save(thumbnailFilePath);
        }
        #endregion
    }
}
