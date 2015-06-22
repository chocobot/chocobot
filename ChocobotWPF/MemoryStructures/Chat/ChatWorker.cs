// ChatWorker.cs


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Chocobot.Utilities.Memory;
using Timer = System.Timers.Timer;

namespace Chocobot.MemoryStructures.Chat
{
    internal class ChatWorker : INotifyPropertyChanged, IDisposable
    {
        #region Property Bindings

        #endregion

        #region Declarations

        private readonly SynchronizationContext _sync = SynchronizationContext.Current;
        private readonly BackgroundWorker _scanner = new BackgroundWorker();
        private readonly Timer _scanTimer;
        private readonly List<uint> _spots = new List<uint>();
        private int _lastCount;
        private uint _lastChatNum;
        private bool _isScanning;

        #endregion

        #region Events

        public event NewLineEventHandler OnNewline = delegate { };

        /// <summary>
        /// </summary>
        /// <param name="chatEntry"> </param>
        private void PostLineEvent(ChatEntry chatEntry)
        {
            _sync.Post(RaiseLineEvent, chatEntry);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"> </param>
        private void RaiseLineEvent(object state)
        {
            OnNewline((ChatEntry) state);
        }

        #endregion

        #region Delegates

        public delegate void NewLineEventHandler(ChatEntry chatEntry);

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="process"> </param>
        /// <param name="offsets"> </param>
        public ChatWorker(Process process)
        {

            _scanTimer = new Timer(100);
            _scanTimer.Elapsed += ScanTimerElapsed;
            _scanner.DoWork += ScannerDoWork;
            _scanner.RunWorkerCompleted += ScannerRunWorkerCompleted;
  
        }

        #region Timer Controls

        /// <summary>
        /// </summary>
        public void StartLogging()
        {
            _scanTimer.Enabled = true;
        }

        /// <summary>
        /// </summary>
        public void StopLogging()
        {
            _scanTimer.Enabled = false;
        }

        #endregion

        #region Threads

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isScanning)
            {
                return;
            }
            if (_scanner.IsBusy != true)
            {
                _scanner.RunWorkerAsync();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ScannerDoWork(object sender, DoWorkEventArgs e)
        {

 
            _isScanning = true;

            long address = MemoryLocations.Database["chatlog"];

            var chatPointers = MemoryHandler.Instance.GetStructure<ChatPointers>(address);
            if (_lastCount == 0)
            {
                _lastCount = (int)chatPointers.LineCount1;
            }
            if (_lastCount == chatPointers.LineCount1)
            {
                return;
            }
            _spots.Clear();
            var index = (int)(chatPointers.OffsetArrayPos - chatPointers.OffsetArrayStart) / 4;
            var lengths = new List<int>();
            try
            {
                for (var i = chatPointers.LineCount1 - _lastCount; i > 0; i--)
                {
                    var getline = ((index - i) < 0) ? (index - i) + 256 : index - i;
                    int lineLen;
                    if (getline == 0)
                    {
                        lineLen = MemoryHandler.Instance.GetInt32(chatPointers.OffsetArrayStart);
                    }
                    else
                    {
                        var previous = MemoryHandler.Instance.GetInt32(chatPointers.OffsetArrayStart + (uint)((getline - 1) * 4));
                        var current = MemoryHandler.Instance.GetInt32(chatPointers.OffsetArrayStart + (uint)(getline * 4));
                        lineLen = current - previous;
                    }
                    lengths.Add(lineLen);

                    _spots.Add(chatPointers.LogStart + (uint)MemoryHandler.Instance.GetInt32(chatPointers.OffsetArrayStart + (uint)((getline - 1) * 4)));
                }
                var limit = _spots.Count;
                for (var i = 0; i < limit; i++)
                {
                    _spots[i] = (_spots[i] > _lastChatNum) ? _spots[i] : chatPointers.LogStart;

                    var text = MemoryHandler.Instance.GetByteArray(_spots[i], lengths[i]);
                    var chatEntry = new ChatEntry(text.ToArray());
                    if (Regex.IsMatch(chatEntry.Combined, @"[\w\d]{4}::?.+"))
                    {
                        PostLineEvent(chatEntry);
                    }
                    else
                    {
                        Debug.Print("DebugLineEvent: {0}", text.ToArray());
                    }
                    _lastChatNum = _spots[i];
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.Print(LogManager.GetCurrentClassLogger(), "", ex);
            }
            _lastCount = (int)chatPointers.LineCount1;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ScannerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _isScanning = false;
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _scanTimer.Elapsed -= ScanTimerElapsed;
            _scanner.DoWork -= ScannerDoWork;
            _scanner.RunWorkerCompleted -= ScannerRunWorkerCompleted;
        }

        #endregion
    }
}