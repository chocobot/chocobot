using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Chocobot.MemoryStructures.Chat;
using Chocobot.Utilities.Memory;

namespace Chocobot.Controls
{
    /// <summary>
    /// Interaction logic for ChatlogControl.xaml
    /// </summary>
    public partial class ChatlogControl : UserControl
    {
        private readonly ChatWorker _chatworker;
        private readonly ObservableCollection<ChatEntry> _chatlog = new ObservableCollection<ChatEntry>();


        public ChatlogControl()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            _chatworker = new ChatWorker(MemoryHandler.Instance.Process);
            _chatworker.OnNewline += _chatworker_Delegate;
            lst_ChatLog.ItemsSource = _chatlog;

            _chatworker.StartLogging();

        }

        private void _chatworker_Delegate(ChatEntry chatentry)
        {
            if (_chatlog.Count > 50)
                _chatlog.RemoveAt(0);

            _chatlog.Add(chatentry);
        }
    }
}
