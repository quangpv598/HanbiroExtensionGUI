using HanbiroExtensionGUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Models
{
    public class BrowserEventArgs : EventArgs
    {
        public HanbiroChromiumBrowser Browser;
        public ErrorArgs ErrorArgs;
        public BrowserEventArgs(HanbiroChromiumBrowser browser, ErrorArgs errorArgs = null)
        {
            this.Browser = browser;
            this.ErrorArgs = errorArgs;
        }
    }
}
