using CefSharp;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Handlers
{
    public class HanbiroResourceHandler : IResourceHandler
    {
        public event EventHandler<HanbiroRequestHandlerArgs> OnBeforeLogin;
        public event EventHandler<HanbiroRequestHandlerArgs> OnLogin;
        public event EventHandler<HanbiroRequestHandlerArgs> OnLoginSuccess;

        public HanbiroResourceHandler()
        {

        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {

        }
    }
}
