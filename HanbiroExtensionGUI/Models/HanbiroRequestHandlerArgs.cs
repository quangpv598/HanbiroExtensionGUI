using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Models
{
    public class HanbiroRequestHandlerArgs : EventArgs
    {
        public IWebBrowser BrowserControl { get; set; }
        public IBrowser Browser { get; set; }
        public IFrame Frame { get; set; }
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }
        
        public HanbiroRequestHandlerArgs(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            this.BrowserControl = browserControl;
            this.Browser = browser;
            this.Frame = frame;
            this.Request = request;
            this.Response = response;
        }
    }
}
