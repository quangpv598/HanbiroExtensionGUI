using CefSharp;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.EventsArgs
{
    public class HanbiroRequestHandlerArgs : EventArgs
    {
        public IWebBrowser BrowserControl { get; set; }
        public IBrowser Browser { get; set; }
        public IFrame Frame { get; set; }
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
        public string Time { get; set; }

        public HanbiroRequestHandlerArgs(User user, IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            this.User = user;
            this.BrowserControl = browserControl;
            this.Browser = browser;
            this.Frame = frame;
            this.Request = request;
            this.Response = response;
        }
    }
}
