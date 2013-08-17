using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Chocobot.Datatypes;
using System.Collections.Generic;

namespace Chocobot.MemoryStructures.Map
{
    internal class MapMarker
    {

        private readonly Coordinate _coordinate;
        private readonly BitmapImage _icon;
        private readonly int _pixelsX;
        private readonly int _pixelsY;

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return (BitmapImage) i;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }

        public MapMarker(Coordinate position, Bitmap icon)
        {
            _icon = Bitmap2BitmapImage(icon);
            _coordinate = position;
            _pixelsX = icon.Width;
            _pixelsY = icon.Height;
        }


        public MapMarker(Coordinate position, BitmapImage icon)
        {
            _icon = icon;
            _coordinate = position;

            _pixelsX = _icon.PixelWidth;
            _pixelsY = _icon.PixelHeight;
        }

        public MapMarker(Coordinate position, string icon)
        {

            _icon = MarkerDatabase.Instance.Icons[icon.ToLower()];
            _coordinate = position;

            _pixelsX = _icon.PixelWidth;
            _pixelsY = _icon.PixelHeight;
        }

        #region "Properties"
        public int PixelsX
        {
            get { return _pixelsX;  }
        }

        public int PixelsY
        {
            get { return _pixelsY; }
        }

        public Coordinate Coordinate
        {
            get { return _coordinate; }
        }

        public BitmapImage Icon
        {
            get { return _icon; }
        }

        #endregion
    }
}