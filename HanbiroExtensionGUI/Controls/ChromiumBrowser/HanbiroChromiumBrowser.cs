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
        private readonly LoginUserAction loginAction;
        private readonly ClockInOutAction clockInOutAction;
        public event EventHandler Disposed;
        #endregion

        #region Properties
        public bool IsCheckHealth { get; set; }
        public UserSettings UserSettings => userSettings;
        public StringBuilder CheckHealthResult { get; protected set; }
        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string address, UserSettings userSettings) : base(address)
        {
            this.userSettings = userSettings;
            this.loginAction = new LoginUserAction(this);
            this.clockInOutAction = new ClockInOutAction(this);
            this.CheckHealthResult = new StringBuilder();

            #region Init Events
            //this.loginAction.OnSuccessEvent += LoginAction_OnSuccessEvent;
            //this.loginAction.OnErrorEvent += LoginAction_OnErrorEvent;

            this.clockInOutAction.OnSuccessEvent += LoginAction_OnSuccessEvent;
            this.clockInOutAction.OnErrorEvent += LoginAction_OnErrorEvent;

            this.FrameLoadEnd += HanbiroChromiumBrowser_FrameLoadEnd;
            #endregion
        }

        private void LoginAction_OnErrorEvent(object sender)
        {
            this.CheckHealthResult.AppendLine("Error!!!");
            Disposed?.Invoke(this, new EventArgs());
        }

        private void LoginAction_OnSuccessEvent(object sender)
        {
            this.CheckHealthResult.AppendLine("Success!!!");
            Disposed?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Events

        private void HanbiroChromiumBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            CheckHealthResult.AppendLineWithShortTime(
                nameof(HanbiroChromiumBrowser_FrameLoadEnd), 
                true,
                $"Frame Loaded With Count = {countLoaded}");
            if (countLoaded == 2)
            {
                CheckHealthResult.Clear();
                loginAction.DoWork();
            }
            if (countLoaded == 3)
            {
                clockInOutAction.DoWork();
            }

            countLoaded++;
        }
        #endregion


    }
}
