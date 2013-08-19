using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Chocobot.MemoryStructures.Map
{
    class MarkerDatabase
    {
        public static MarkerDatabase Instance = new MarkerDatabase();


        public readonly Dictionary<string, BitmapImage> Icons = new Dictionary<string, BitmapImage>();


        public MarkerDatabase()
        {
            LoadMarkers();
        }

        private void LoadMarkers()
        {
            Icons.Add("aethercrystal", new BitmapImage(new Uri("pack://application:,,/Resources/aethercrystal.png"))); //Bitmap2BitmapImage(Properties.Resources.aethercrystal));
            Icons.Add("zone", new BitmapImage(new Uri("pack://application:,,/Resources/zone.png"))); //Bitmap2BitmapImage(Properties.Resources.zone));
            Icons.Add("bell", new BitmapImage(new Uri("pack://application:,,/Resources/bell.png"))); //Bitmap2BitmapImage(Properties.Resources.bell));
            Icons.Add("boat", new BitmapImage(new Uri("pack://application:,,/Resources/boat.png"))); //Bitmap2BitmapImage(Properties.Resources.boat));
            Icons.Add("town", new BitmapImage(new Uri("pack://application:,,/Resources/town.png"))); //Bitmap2BitmapImage(Properties.Resources.town));
            Icons.Add("stepup", new BitmapImage(new Uri("pack://application:,,/Resources/stepup.png"))); //Bitmap2BitmapImage(Properties.Resources.stepup));
            Icons.Add("stepdown", new BitmapImage(new Uri("pack://application:,,/Resources/stepdown.png"))); //Bitmap2BitmapImage(Properties.Resources.stepdown));
            Icons.Add("chocobo", new BitmapImage(new Uri("pack://application:,,/Resources/chocobo.png"))); //Bitmap2BitmapImage(Properties.Resources.chocobo));
            Icons.Add("crystal", new BitmapImage(new Uri("pack://application:,,/Resources/crystal.png"))); //Bitmap2BitmapImage(Properties.Resources.crystal));

            Icons.Add("boss", new BitmapImage(new Uri("pack://application:,,/Resources/boss.png"))); //Bitmap2BitmapImage(Properties.Resources.boss));
            Icons.Add("dungeon", new BitmapImage(new Uri("pack://application:,,/Resources/dungeon.png"))); //Bitmap2BitmapImage(Properties.Resources.dungeon));
            Icons.Add("fate", new BitmapImage(new Uri("pack://application:,,/Resources/fate.png"))); //Bitmap2BitmapImage(Properties.Resources.fate));
            Icons.Add("fate2", new BitmapImage(new Uri("pack://application:,,/Resources/fate2.png"))); //Bitmap2BitmapImage(Properties.Resources.fate2));
            Icons.Add("fate3", new BitmapImage(new Uri("pack://application:,,/Resources/fate3.png"))); //Bitmap2BitmapImage(Properties.Resources.fate3));
            Icons.Add("monster", new BitmapImage(new Uri("pack://application:,,/Resources/monster.png"))); //Bitmap2BitmapImage(Properties.Resources.fate3));

            Icons.Add("bronze_chest", new BitmapImage(new Uri("pack://application:,,/Resources/bronze_chest.png"))); //Bitmap2BitmapImage(Properties.Resources.bronze_chest));
            Icons.Add("silver_chest", new BitmapImage(new Uri("pack://application:,,/Resources/silver_chest.png"))); //Bitmap2BitmapImage(Properties.Resources.silver_chest));
            Icons.Add("gold_chest", new BitmapImage(new Uri("pack://application:,,/Resources/gold_chest.png"))); //Bitmap2BitmapImage(Properties.Resources.gold_chest));

            Icons.Add("conjurer", new BitmapImage(new Uri("pack://application:,,/Resources/conjurer.png"))); //Bitmap2BitmapImage(Properties.Resources.conjurer));
            Icons.Add("lancer", new BitmapImage(new Uri("pack://application:,,/Resources/lancer.png"))); //Bitmap2BitmapImage(Properties.Resources.lancer));
            Icons.Add("gladiator", new BitmapImage(new Uri("pack://application:,,/Resources/gladiator.png"))); //Bitmap2BitmapImage(Properties.Resources.gladiator));
            Icons.Add("pugilist", new BitmapImage(new Uri("pack://application:,,/Resources/pugilist.png"))); //Bitmap2BitmapImage(Properties.Resources.gladiator));
            Icons.Add("thaumaturge", new BitmapImage(new Uri("pack://application:,,/Resources/thaumaturge.png"))); //Bitmap2BitmapImage(Properties.Resources.thaumaturge));

            Icons.Add("field", new BitmapImage(new Uri("pack://application:,,/Resources/field.png"))); //Bitmap2BitmapImage(Properties.Resources.field));
            Icons.Add("field2", new BitmapImage(new Uri("pack://application:,,/Resources/field2.png"))); //Bitmap2BitmapImage(Properties.Resources.field2));
            Icons.Add("mining", new BitmapImage(new Uri("pack://application:,,/Resources/mining.png"))); //Bitmap2BitmapImage(Properties.Resources.mining));

            Icons.Add("culinarian", new BitmapImage(new Uri("pack://application:,,/Resources/culinarian.png"))); //Bitmap2BitmapImage(Properties.Resources.culinarian));
            Icons.Add("leatherworker", new BitmapImage(new Uri("pack://application:,,/Resources/leatherworker.png"))); //Bitmap2BitmapImage(Properties.Resources.leatherworker));
            Icons.Add("miner", new BitmapImage(new Uri("pack://application:,,/Resources/miner.png"))); //Bitmap2BitmapImage(Properties.Resources.miner));
            Icons.Add("fisher", new BitmapImage(new Uri("pack://application:,,/Resources/fisher.png"))); //Bitmap2BitmapImage(Properties.Resources.fisher));
            Icons.Add("botanist", new BitmapImage(new Uri("pack://application:,,/Resources/botanist.png"))); //Bitmap2BitmapImage(Properties.Resources.botanist));
            Icons.Add("carpenter", new BitmapImage(new Uri("pack://application:,,/Resources/carpenter.png"))); //Bitmap2BitmapImage(Properties.Resources.carpenter));
            Icons.Add("weaver", new BitmapImage(new Uri("pack://application:,,/Resources/weaver.png"))); //Bitmap2BitmapImage(Properties.Resources.weaver));
            Icons.Add("alchemist", new BitmapImage(new Uri("pack://application:,,/Resources/alchemist.png"))); //Bitmap2BitmapImage(Properties.Resources.alchemist));
            Icons.Add("armorer", new BitmapImage(new Uri("pack://application:,,/Resources/armorer.png"))); //Bitmap2BitmapImage(Properties.Resources.armorer));
            Icons.Add("blacksmith", new BitmapImage(new Uri("pack://application:,,/Resources/blacksmith.png"))); //Bitmap2BitmapImage(Properties.Resources.blacksmith));
            Icons.Add("goldsmith", new BitmapImage(new Uri("pack://application:,,/Resources/goldsmith.png"))); //Bitmap2BitmapImage(Properties.Resources.goldsmith));
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            try
            {
                return (BitmapImage)i;
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }

    }
}
