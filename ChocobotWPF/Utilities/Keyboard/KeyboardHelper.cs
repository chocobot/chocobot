// KeyBoardHelper.cs


using System;
using System.IO;
using System.Threading;
using Chocobot.Datatypes;
using Chocobot.Utilities.Memory;


namespace Chocobot.Utilities.Keyboard
{
    public static class KeyBoardHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void Alt(Keys key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)Keys.Menu, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.Char, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)Keys.Menu, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void Ctrl(Keys key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)Keys.ControlKey, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.Char, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)Keys.ControlKey, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"> </param>
        public static void SendNotify(byte[] bytes)
        {
            
            Thread.Sleep(100);
            var input = new MemoryStream(bytes);
            var reader = new BinaryReader(input);
            while (input.Position < input.Length)
            {
                UnsafeNativeMethods.SendNotifyMessageW(Constants.ProcessHandle, 0x102, (IntPtr)reader.ReadInt16(), null);
            }
            KeyPressNotify(Keys.Return);
            
        }

        /// <summary>
        /// </summary>
        public static void Paste()
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)Keys.ControlKey, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)Keys.V, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)Keys.V, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)Keys.ControlKey, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        private static void KeyPressNotify(Keys key)
        {
            UnsafeNativeMethods.SendNotifyMessageW(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)key, null);
            UnsafeNativeMethods.SendNotifyMessageW(Constants.ProcessHandle, WindowsMessageEvents.Char, (IntPtr)key, null);
            UnsafeNativeMethods.SendNotifyMessageW(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)key, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void KeyPress(Keys key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.Char, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)key, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void KeyDown(Keys key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyDown, (IntPtr)key, null);
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.Char, (IntPtr)key, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void KeyUp(Keys key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.KeyUp, (IntPtr)key, null);
        }


        /// <summary>
        /// </summary>
        /// <param name="key"> </param>
        public static void KeyPressIntPtr(IntPtr key)
        {
            UnsafeNativeMethods.SendMessage(Constants.ProcessHandle, WindowsMessageEvents.Char, key, null);
        }
    }
}