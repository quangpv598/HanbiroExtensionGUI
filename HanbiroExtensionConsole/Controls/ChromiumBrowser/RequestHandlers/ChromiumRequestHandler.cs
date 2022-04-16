using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Controls.ChromiumBrowser
{
    class CustomResourceRequestHandler : ResourceRequestHandler
    {
        private readonly HanbiroRequestHanlders hanbiroRequestHanlders;
        public CustomResourceRequestHandler(HanbiroRequestHanlders hanbiroRequestHanlders)
        {
            this.hanbiroRequestHanlders = hanbiroRequestHanlders;
        }

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return hanbiroRequestHanlders.GetResourceResponseFilter(chromiumWebBrowser, browser, frame, request, response);
        }

        protected override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            hanbiroRequestHanlders.OnResourceLoadComplete(browserControl, browser, frame, request, response, status, receivedContentLength);
        }
    }

    public class ChromiumRequestHandler : RequestHandler
    {
        private readonly HanbiroRequestHanlders hanbiroRequestHanlders;
        public ChromiumRequestHandler(HanbiroRequestHanlders hanbiroRequestHanlders)
        {
            this.hanbiroRequestHanlders = hanbiroRequestHanlders;
        }
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new CustomResourceRequestHandler(hanbiroRequestHanlders);
        }

    }
}
