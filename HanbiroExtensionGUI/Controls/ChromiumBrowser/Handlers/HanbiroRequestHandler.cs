using CefSharp;
using CefSharp.Handler;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Handlers
{
    class CustomResourceRequestHandler : ResourceRequestHandler
    {
        private readonly IResourceHandler resourceHandler;
        public CustomResourceRequestHandler(IResourceHandler resourceHandler)
        {
            this.resourceHandler = resourceHandler;
        }

        protected override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            resourceHandler.GetResourceResponseFilter(browserControl, browser, frame, request, response);
        }
    }


    public class HanbiroRequestHandler : RequestHandler
    {
        private readonly IResourceHandler resourceHandler;
        public HanbiroRequestHandler(IResourceHandler resourceHandler)
        {
            this.resourceHandler = resourceHandler;
        }
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new CustomResourceRequestHandler(resourceHandler);
        }

    }
}
