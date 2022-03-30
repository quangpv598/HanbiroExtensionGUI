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
        private readonly HanbiroChromiumBrowser hanbiroChromiumBrowser;
        public HanbiroChromiumBrowser Browser => hanbiroChromiumBrowser;
        public UserSettings UserSettings => hanbiroChromiumBrowser.UserSettings;
        public AbstractAction(HanbiroChromiumBrowser hanbiroChromiumBrowser)
        {
            this.hanbiroChromiumBrowser = hanbiroChromiumBrowser;
        }

        public abstract void DoWork();
    }
}
