using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Steps;
using HanbiroExtensionGUI.Extensions;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Resources;
using HanbiroExtensionGUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI.Controls
{
    public class HanbiroChromiumBrowser : ChromiumWebBrowser
    {
        #region Fields
        int countLoaded = 0;
        private UserSettings userSettings;
        #endregion

        #region Properties
        public bool IsCheckHealth { get; set; }
        public UserSettings UserSettings => userSettings;
        public StringBuilder CheckHealthResult { get; protected set; } = new StringBuilder();
        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string address, UserSettings userSettings) : base(address)
        {
            this.userSettings = userSettings;
            this.FrameLoadEnd += HanbiroChromiumBrowser_FrameLoadEnd;
        }
        #endregion

        #region Events

        private void HanbiroChromiumBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            CheckHealthResult.AppendLineWithShortTime(nameof(HanbiroChromiumBrowser_FrameLoadEnd), 
                true,
                $"Frame Loaded With Count = {countLoaded}");
            if (countLoaded == 2)
            {
                var loginAction = new LoginUserAction(this);
                loginAction.DoWork();
            }
            if (countLoaded == 3)
            {
                var clockInOutAction = new ClockInOutAction(this);
                clockInOutAction.DoWork();
            }

            countLoaded++;
        }
        #endregion


    }
}
