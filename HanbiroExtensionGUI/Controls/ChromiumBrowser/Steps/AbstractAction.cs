using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Steps
{
    public abstract class AbstractAction
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void SuccessErrorEventHandler(object sender);

        // Declare the event.
        public event SuccessErrorEventHandler OnSuccessEvent;
        public event SuccessErrorEventHandler OnErrorEvent;

        private readonly HanbiroChromiumBrowser hanbiroChromiumBrowser;
        public HanbiroChromiumBrowser Browser => hanbiroChromiumBrowser;
        public UserSettings UserSettings => hanbiroChromiumBrowser.UserSettings;
        public AbstractAction(HanbiroChromiumBrowser hanbiroChromiumBrowser)
        {
            this.hanbiroChromiumBrowser = hanbiroChromiumBrowser;
        }

        public abstract void DoWork();

        // Wrap the event in a protected virtual method
        // to enable derived classes to raise the event.
        protected virtual void RaiseSuccessEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            OnSuccessEvent?.Invoke(this);
        }

        protected virtual void RaiseErrorEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            OnErrorEvent?.Invoke(this);
        }
    }
}
