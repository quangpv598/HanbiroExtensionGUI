using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Handlers;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Steps;
using HanbiroExtensionGUI.Enums;
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
        int countLoaded = 0, countFrameLoaded = 0;
        private User currentUser;
        private readonly LoginUserAction loginAction;
        private readonly ClockInOutAction clockInOutAction;
        public event EventHandler OnClockInOutSuccess;
        public event EventHandler OnClockInOutError;
        public event EventHandler FrameLoaded;
        private bool isStartWork = false;
        private bool isBusy = false;
        #endregion

        #region Properties
        public bool IsCheckHealth { get; set; }
        public bool IsCheckAccountValid { get; set; }
        public User CurrentUser => currentUser;
        public StringBuilder CheckHealthResult { get; protected set; }
        public bool IsBusy => isBusy;
        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string address) : base(address)
        {
            this.loginAction = new LoginUserAction(this);
            this.clockInOutAction = new ClockInOutAction(this);
            this.CheckHealthResult = new StringBuilder();

            var resourceHandler = new HanbiroResourceHandler();

            var hanbiroRequestHandler = new HanbiroRequestHandler(resourceHandler);

            base.RequestHandler = hanbiroRequestHandler;

            #region Init Events
            this.loginAction.OnSuccessEvent += LoginAction_OnSuccessEvent;
            this.loginAction.OnErrorEvent += LoginAction_OnErrorEvent;

            this.clockInOutAction.OnSuccessEvent += LoginAction_OnSuccessEvent;
            this.clockInOutAction.OnErrorEvent += LoginAction_OnErrorEvent;

            this.FrameLoadEnd += HanbiroChromiumBrowser_FrameLoadEnd;
            #endregion
        }

        private void LoginAction_OnErrorEvent(object sender, ErrorArgs errorCoce)
        {
            this.CheckHealthResult.AppendLine("Error!!!");
            OnClockInOutError?.Invoke(this, new BrowserEventArgs(this, errorCoce));

            isBusy = false;
        }

        private void LoginAction_OnSuccessEvent(object sender)
        {
            this.CheckHealthResult.AppendLine("Success!!!");
            OnClockInOutSuccess?.Invoke(this, new BrowserEventArgs(this));
            isBusy = false;
        }
        #endregion

        #region Events

        private void HanbiroChromiumBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (isStartWork)
            {
                CheckHealthResult.AppendLineWithShortTime(
                nameof(HanbiroChromiumBrowser_FrameLoadEnd),
                true,
                $"Frame Loaded With Count = {countLoaded}");
                if (countLoaded == 1)
                {
                    CheckHealthResult.Clear();
                    loginAction.DoWork();
                }
                if (countLoaded == 2)
                {
                    clockInOutAction.DoWork();
                }

                countLoaded++;
            }
            else
            {
                if (countFrameLoaded == 2)
                {
                    FrameLoaded?.Invoke(this, new EventArgs());
                }
                countFrameLoaded++;
            }
        }
        #endregion

        #region Methods

        public async void DoWork(User user)
        {
            isBusy = true;
            this.currentUser = user;
            countLoaded = 0;
            isStartWork = true;
            await this.GetCookieManager().DeleteCookiesAsync(string.Empty, string.Empty);
            this.Reload();
        }

        #endregion
    }
}
