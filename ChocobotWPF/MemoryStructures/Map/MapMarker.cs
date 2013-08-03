using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Chocobot.Datatypes;

namespace Chocobot.MemoryStructures.Map
{
    internal class MapMarker
    {
        public enum MarkerIcon
        {
            Town,
            Crystal,
            Boat,
            Chocobo,
            Zone,
            Dungeon,
            Boss,
            Treasure1,
            Treasure2,
            Treasure3,
            Fate,
            Fate2,
            Fate3,
            Field,
            Field2,
            Mining,
            Miner,
            Fisher,
            Culinarian,
            Botanist,
            Alchemist,
            Armorer,
            Blacksmith,
            Carpenter,
            Leatherworker,
            Weaver
        }


        private readonly Coordinate _coordinate;
        private readonly BitmapImage _icon;

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
        }

        public MapMarker(Coordinate position, MarkerIcon icon)
        {
            switch (icon)
            {
                case MarkerIcon.Zone:
                    _icon = Bitmap2BitmapImage(Properties.Resources.zone);
                    break;
                case MarkerIcon.Alchemist:
                    _icon = Bitmap2BitmapImage(Properties.Resources.alchemist);
                    break;
                case MarkerIcon.Armorer:
                    _icon = Bitmap2BitmapImage(Properties.Resources.armorer);
                    break;
                case MarkerIcon.Blacksmith:
                    _icon = Bitmap2BitmapImage(Properties.Resources.blacksmith);
                    break;
                case MarkerIcon.Boat:
                    _icon = Bitmap2BitmapImage(Properties.Resources.boat);
                    break;
                case MarkerIcon.Boss:
                    _icon = Bitmap2BitmapImage(Properties.Resources.boss);
                    break;
                case MarkerIcon.Botanist:
                    _icon = Bitmap2BitmapImage(Properties.Resources.botanist);
                    break;
                case MarkerIcon.Carpenter:
                    _icon = Bitmap2BitmapImage(Properties.Resources.carpenter);
                    break;
                case MarkerIcon.Chocobo:
                    _icon = Bitmap2BitmapImage(Properties.Resources.chocobo);
                    break;
                case MarkerIcon.Crystal:
                    _icon = Bitmap2BitmapImage(Properties.Resources.crystal);
                    break;
                case MarkerIcon.Culinarian:
                    _icon = Bitmap2BitmapImage(Properties.Resources.culinarian);
                    break;
                case MarkerIcon.Dungeon:
                    _icon = Bitmap2BitmapImage(Properties.Resources.dungeon);
                    break;
                case MarkerIcon.Fate:
                    _icon = Bitmap2BitmapImage(Properties.Resources.fate);
                    break;
                case MarkerIcon.Fate2:
                    _icon = Bitmap2BitmapImage(Properties.Resources.fate2);
                    break;
                case MarkerIcon.Fate3:
                    _icon = Bitmap2BitmapImage(Properties.Resources.fate3);
                    break;
                case MarkerIcon.Field:
                    _icon = Bitmap2BitmapImage(Properties.Resources.field);
                    break;
                case MarkerIcon.Field2:
                    _icon = Bitmap2BitmapImage(Properties.Resources.field2);
                    break;
                case MarkerIcon.Fisher:
                    _icon = Bitmap2BitmapImage(Properties.Resources.fisher);
                    break;
                case MarkerIcon.Leatherworker:
                    _icon = Bitmap2BitmapImage(Properties.Resources.leatherworker);
                    break;
                case MarkerIcon.Miner:
                    _icon = Bitmap2BitmapImage(Properties.Resources.miner);
                    break;
                case MarkerIcon.Mining:
                    _icon = Bitmap2BitmapImage(Properties.Resources.mining);
                    break;
                case MarkerIcon.Town:
                    _icon = Bitmap2BitmapImage(Properties.Resources.town);
                    break;
                case MarkerIcon.Treasure1:
                    _icon = Bitmap2BitmapImage(Properties.Resources.bronze_chest);
                    break;
                case MarkerIcon.Treasure2:
                    _icon = Bitmap2BitmapImage(Properties.Resources.silver_chest);
                    break;
                case MarkerIcon.Treasure3:
                    _icon = Bitmap2BitmapImage(Properties.Resources.gold_chest);
                    break;
                case MarkerIcon.Weaver:
                    _icon = Bitmap2BitmapImage(Properties.Resources.weaver);
                    break;
                default:
                    _icon = Bitmap2BitmapImage(Properties.Resources.zone);
                    break;
            }

            _coordinate = position;
        }

        #region "Properties"

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