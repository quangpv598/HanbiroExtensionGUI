using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Controls;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public abstract class HanbiroServiceBase : ServicesBase
    {
        #region Fields
        private HanbiroChromiumBrowser hanbiroChromiumBrowser;
        private bool readyToUse = false;
        private int countFrameLoaded = 0;
        public event EventHandler OnSuccess;
        public event EventHandler OnError;
        #endregion

        #region Properties
        public bool ReadyToUse => readyToUse;
        public HanbiroChromiumBrowser Browser => hanbiroChromiumBrowser;
        #endregion

        #region Constructors
        public HanbiroServiceBase(CurrentUserSettings currentUserSettings) : base(currentUserSettings)
        {
            hanbiroChromiumBrowser = new HanbiroChromiumBrowser(GlobalConstants.BaseUrl);
            hanbiroChromiumBrowser.OnClockInOutSuccess += HanbiroChromiumBrowser_OnClockInOutSuccess;
            hanbiroChromiumBrowser.OnClockInOutError += HanbiroChromiumBrowser_OnClockInOutError;
            hanbiroChromiumBrowser.FrameLoaded += HanbiroChromiumBrowser_FrameLoaded;
        }
        #endregion

        #region Events
        private void HanbiroChromiumBrowser_FrameLoaded(object sender, EventArgs e)
        {
            countFrameLoaded++;
            if (countFrameLoaded == 1)
            {
                readyToUse = true;
            }
        }

        private void HanbiroChromiumBrowser_OnClockInOutError(object sender, EventArgs e)
        {
            OnError?.Invoke(this, new BrowserEventArgs((HanbiroChromiumBrowser)sender));
        }

        private void HanbiroChromiumBrowser_OnClockInOutSuccess(object sender, EventArgs e)
        {
            OnSuccess?.Invoke(this, new BrowserEventArgs((HanbiroChromiumBrowser)sender));
        }
        #endregion

        #region Methods
        #endregion
    }
}
