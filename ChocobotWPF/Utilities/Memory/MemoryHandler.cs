// MemoryHandler.cs


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Chocobot.Utilities.Keyboard;

namespace Chocobot.Utilities.Memory
{
    internal class MemoryHandler 
    {
        #region Property Bindings

        public Process Process { get; set; }
        public uint Address {  get; set; }
        public uint BaseAddress { get; set; }

        #endregion

        #region Private Structs

        public struct MemoryBlock
        {
            public long Start;
            public long Length;
        }

        #endregion

        #region Declarations

        #endregion

        public static MemoryHandler Instance = new MemoryHandler("ffxiv");

        public static Process FindProcess(string pName)
        {
            Process[] p = Process.GetProcessesByName(pName);
  
            if (p.Length > 0)
            {
                //For Each Process In p
                //If p(index).Id = SigFF.FFXI_PID(SigFF.PIDSelect.SelectedIndex).Id Then
                //Exit For
                // Else
                //index += 1
                // End If
                //Next
                return p[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="address"> </param>
        public MemoryHandler(Process process, uint address)
        {
            Process = process;
            Address = address;
            GetModuleEndAddress(Process);
            Datatypes.Constants.ProcessHandle = Process.MainWindowHandle;
            BaseAddress = (uint)Process.MainModule.BaseAddress.ToInt32();
        }


        /// <summary>
        /// </summary>
        public MemoryHandler(string Name)
        {

            Process = FindProcess(Name);
            GetModuleEndAddress(Process);
            Datatypes.Constants.ProcessHandle = Process.MainWindowHandle;
            BaseAddress = (uint)Process.MainModule.BaseAddress.ToInt32();
        }

        public string GetOpcode(int Size)
        {
            byte[] buffer1 = this.GetByteArray(Size);
            return BitConverter.ToString(buffer1);
        }

        public static int GetModuleEndAddress(Process ProcessInstance)
        {

            try
            {
                int EndAddress = ProcessInstance.MainModule.BaseAddress.ToInt32() + ProcessInstance.MainModule.ModuleMemorySize;

                return EndAddress;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="target"> </param>
        /// <param name="data"> </param>
        /// <returns> </returns>
        private static bool Poke(Process process, uint target, byte[] data)
        {
            var byteWritten = new IntPtr(0);
            return UnsafeNativeMethods.WriteProcessMemory(process.Handle, new IntPtr(target), data, new UIntPtr((UInt32)data.Length), ref byteWritten);
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="address"> </param>
        /// <param name="buffer"> </param>
        /// <returns> </returns>
        private static bool Peek(Process process, uint address, byte[] buffer)
        {
            try
            {
                var target = new IntPtr(address);
                return process != null && UnsafeNativeMethods.ReadProcessMemory(process.Handle, target, buffer, buffer.Length, 0);
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error peek. \n" + address.ToString());
                Debug.Print(ex.StackTrace);
                    Debug.Print(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static Process[] GetProcesses()
        {
            var result = Process.GetProcesses();
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> </returns>
        public static Process GetProcessById(int id)
        {
            try
            {
                var result = Process.GetProcessById(id);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public static Process GetProcessByName(string name)
        {
            var processes = Process.GetProcessesByName(name);
            if (processes.Length <= 0)
            {
                return null;
            }
            var result = processes[0];
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <returns> </returns>
        private static IEnumerable<ProcessModule> GetModules(Process process)
        {
            try
            {
                var modules = process.Modules;
                var result = new ProcessModule[modules.Count];
                for (var i = 0; i < modules.Count; i++)
                {
                    result[i] = modules[i];
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public static ProcessModule GetModuleByName(Process process, string name)
        {
            ProcessModule result = null;
            try
            {
                var modules = GetModules(process);
                foreach (var module in modules.Where(module => module.ModuleName.IndexOf(name, StringComparison.Ordinal) > -1))
                {
                    result = module;
                    break;
                }
            }
            catch
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="address"> </param>
        /// <returns> </returns>
        public static ProcessModule GetModuleByAddress(Process process, Int32 address)
        {
            try
            {
                var modules = GetModules(process);
                return (from module in modules let baseAddress = module.BaseAddress.ToInt32() where (baseAddress <= address) && (baseAddress + module.ModuleMemorySize >= address) select module).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="file"> </param>
        /// <returns> </returns>
        private static int GetModuleBaseAddress(Process process, string file)
        {
            var modCol = process.Modules;
            foreach (var procMod in modCol.Cast<ProcessModule>().Where(procMod => procMod.FileName == file))
            {
                return procMod.BaseAddress.ToInt32();
            }
            return -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="file"> </param>
        /// <returns> </returns>
        public static int GetModuleEndAddress(Process process, string file)
        {
            var modCol = process.Modules;
            foreach (var procMod in modCol.Cast<ProcessModule>().Where(procMod => procMod.FileName == file))
            {
                return procMod.BaseAddress.ToInt32() + procMod.ModuleMemorySize;
            }
            return -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <returns> </returns>
        public static MemoryBlock GetProcessMemoryBlock(Process process)
        {
            var counter = new UnsafeNativeMethods.ProcessMemoryCounters();
            UnsafeNativeMethods.GetProcessMemoryInfo(process.Handle, out counter, Marshal.SizeOf(counter));
            var block = new MemoryBlock { Start = process.MainModule.BaseAddress.ToInt64(), Length = counter.PagefileUsage };
            return block;
        }

        /// <summary>
        /// </summary>
        /// <param name="intLength"> </param>
        /// <returns> </returns>
        public string GetOperationCode(int intLength)
        {
            var buffer = GetByteArray(intLength);
            return BitConverter.ToString(buffer);
        }



        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public byte GetByte()
        {
            var data = new byte[1];
            Peek(Process, Address, data);
            return data[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="offset"> </param>
        /// <returns> </returns>
        public byte GetByte(uint offset)
        {
            var data = new byte[1];
            Peek(Process, Address + offset, data);
            return data[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="num"> </param>
        /// <returns> </returns>
        public byte[] GetByteArray(int num)
        {
            var data = new byte[num];
            Peek(Process, Address, data);
            return data;
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public double GetDouble()
        {
            var value = new byte[8];
            Peek(Process, Address, value);
            return BitConverter.ToDouble(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public float GetFloat()
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToSingle(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public short GetInt16()
        {
            var value = new byte[2];
            Peek(Process, Address, value);
            return BitConverter.ToInt16(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public int GetInt32()
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public long GetInt64()
        {
            var value = new byte[8];
            Peek(Process, Address, value);
            return BitConverter.ToInt64(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public string GetString()
        {
            var bytes = new byte[24];
            Peek(Process, Address, bytes);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="maxSize"> </param>
        /// <returns> </returns>
        public string GetString(int maxSize)
        {
            var bytes = new byte[maxSize];
            Peek(Process, Address, bytes);
            var realSize = 0;
            for (var i = 0; i < maxSize; i++)
            {
                if (bytes[i] != 0)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="size"> </param>
        /// <returns> </returns>
        public string GetStringBySize(int size)
        {
            var bytes = new byte[size];
            Peek(Process, Address, bytes);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public int GetProgram()
        {
            var value = new byte[30];
            Peek(Process, Address, value);
            return BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public UInt32 GetUInt32()
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToUInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public UInt16 GetUInt16()
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToUInt16(value, 0);
        }


        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public byte GetByte(uint Address, bool Temp)
        {
            var data = new byte[1];
            Peek(Process, Address, data);
            return data[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="offset"> </param>
        /// <returns> </returns>
        public byte GetByte(uint Address, uint offset)
        {
            var data = new byte[1];
            Peek(Process, Address + offset, data);
            return data[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="num"> </param>
        /// <returns> </returns>
        public byte[] GetByteArray(uint Address, int num)
        {
            var data = new byte[num];
            Peek(Process, Address, data);
            return data;
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public double GetDouble(uint Address)
        {
            var value = new byte[8];
            Peek(Process, Address, value);
            return BitConverter.ToDouble(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public float GetFloat(uint Address)
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToSingle(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public short GetInt16(uint Address)
        {
            var value = new byte[2];
            Peek(Process, Address, value);
            return BitConverter.ToInt16(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public int GetInt32(uint Address)
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public long GetInt64(uint Address)
        {
            var value = new byte[8];
            Peek(Process, Address, value);
            return BitConverter.ToInt64(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public string GetString(uint Address)
        {
            var bytes = new byte[24];
            Peek(Process, Address, bytes);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="maxSize"> </param>
        /// <returns> </returns>
        public string GetString(uint Address, int maxSize)
        {
            var bytes = new byte[maxSize];
            Peek(Process, Address, bytes);
            var realSize = 0;
            for (var i = 0; i < maxSize; i++)
            {
                if (bytes[i] != 0 && bytes[i] != 2)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="size"> </param>
        /// <returns> </returns>
        public string GetStringBySize(uint Address, int size)
        {
            var bytes = new byte[size];
            Peek(Process, Address, bytes);
            return Encoding.ASCII.GetString(bytes);
        }


        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public UInt32 GetUInt32(uint Address)
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToUInt32(value, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public UInt16 GetUInt16(uint Address)
        {
            var value = new byte[4];
            Peek(Process, Address, value);
            return BitConverter.ToUInt16(value, 0);
        }
        /// <summary>
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public T GetStructure<T>()
        {
            var lpBytesWritten = 0;
            var buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)));
            UnsafeNativeMethods.ReadProcessMemory(Process.Handle, new IntPtr(Address), buffer, Marshal.SizeOf(typeof(T)), ref lpBytesWritten);
            var retValue = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeCoTaskMem(buffer);
            return retValue;
        }


        /// <summary>
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public T GetStructure<T>(uint Address)
        {
            var lpBytesWritten = 0;
            var buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)));
            UnsafeNativeMethods.ReadProcessMemory(Process.Handle, new IntPtr(Address), buffer, Marshal.SizeOf(typeof(T)), ref lpBytesWritten);
            var retValue = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeCoTaskMem(buffer);
            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        public void Reset(int val)
        {
            var data = BitConverter.GetBytes(val);
            Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetByte(byte val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetByteArray(byte[] val)
        {
            return Poke(Process, Address, val);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetDouble(double val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetFloat(float val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetInt16(short val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetInt32(int val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetInt64(long val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetUInt16(UInt16 val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public bool SetUInt32(UInt32 val)
        {
            var data = BitConverter.GetBytes(val);
            return Poke(Process, Address, data);
        }



        public void SetByte(uint Address, byte value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetByteArray(uint Address, byte[] value)
        {
            Poke(Process, Address, value);
        }

        public void SetDouble(uint Address, double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetFloat(uint Address, float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetShort(uint Address, short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetInt32(uint Address, int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }


        public void SetInt64(uint Address, long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetUInt16(uint Address, UInt16 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        public void SetUInt32(uint Address, UInt32 value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Poke(Process, Address, data);
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"> </param>
        /// <returns> </returns>
        public static string GetStringFromByteArray(byte[] bytes)
        {
            var u8 = new UTF8Encoding();
            var text = u8.GetString(bytes);
            var startIndex = text.IndexOf(Convert.ToChar(0));
            if ((startIndex != -1))
            {
                text = text.Remove(startIndex, (text.Length - startIndex));
            }
            return text;
        }

        /// <summary>
        /// </summary>
        /// <param name="byteArray"> </param>
        /// <returns> </returns>
        public static char[] ByteArrayToCharArray(byte[] byteArray)
        {
            var charArray = new char[byteArray.Length];
            for (var x = 0; x < byteArray.Length; x++)
            {
                charArray[x] = Convert.ToChar(byteArray[x]);
            }
            return charArray;
        }

        /// <summary>
        /// </summary>
        /// <param name="charArray"> </param>
        /// <returns> </returns>
        public static byte[] CharArrayToByteArray(char[] charArray)
        {
            var byteArray = new byte[charArray.Length];
            for (var x = 0; x < charArray.Length; x++)
            {
                byteArray[x] = Convert.ToByte(charArray[x]);
            }
            return byteArray;
        }

        #region Implementation of INotifyPropertyChanged

        //public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private void RaisePropertyChanged([CallerMemberName] string caller = "")
        //{
        //    PropertyChanged(this, new PropertyChangedEventArgs(caller));
        //}

        #endregion
    }
}