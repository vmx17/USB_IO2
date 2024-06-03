using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify
{
    public delegate void OnNotifyEventHandler(object sender, MessageEventArgs args);
    public class Notifier
    {
        [Description("Handle event with message for logging. If you want get message from this object, add code here.")]
        [Category("MesssageEvent")]
        [DisplayName("OnNotify")]
        public event OnNotifyEventHandler OnNotify;

        public Notifier()
        {
            ;
        }
        /// <summary>
        /// publish events with message
        /// </summary>
        /// <param name="message"></param>
        public void Notify(string message)
        {
            OnNotify?.Invoke(this, new MessageEventArgs(message));
        }
    }

    /// <summary>
    /// event args with message
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public MessageEventArgs()
        {
        }
        public MessageEventArgs(string mes)
        {
            Message = mes;
        }
    }
}
