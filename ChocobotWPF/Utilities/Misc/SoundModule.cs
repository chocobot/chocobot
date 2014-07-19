using System.Diagnostics;
using System.Media;
using System.Threading;

namespace Chocobot.Utilities.Misc
{
    class SoundModule
    {

        private static readonly Stopwatch Timeout = new Stopwatch();
        private static string _lastSound = "";

        public static void PlayAsyncSound(string sound)
        {

            if (sound == _lastSound && (Timeout.Elapsed.TotalSeconds < 10 || Timeout.IsRunning == false))
            {
                return;
            }

            Timeout.Restart();
            _lastSound = sound;

            ThreadPool.QueueUserWorkItem(ignoredState =>
            {
                using (
                    var player =
                        new SoundPlayer(
                            System.IO.Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly
                                    ().Location) + "\\" + sound))
                {
                    player.Play();
                }
            });
        }
    }
}
