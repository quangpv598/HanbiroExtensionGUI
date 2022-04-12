using CefSharp;
using HanbiroExtensionGUI.Constants;
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
        public event EventHandler<HanbiroRequestHandlerArgs> OnLoginError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnSaveCookie;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnFrameLoadEnd;

        public HanbiroResourceHandler()
        {

        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            //if (request.Url.Equals($"{GlobalConstants.BaseUrl}ngw/app/template/sign/login.html"))
            //{
            //    OnBeforeLogin?.Invoke(this, new HanbiroRequestHandlerArgs(browserControl, browser, frame, request, response));
            //}

            if(request.Url == "http://infoplusvn.hanbiro.net/ngw/app/lib/css/input-password.css")
            {
                OnFrameLoadEnd?.Invoke(this, new HanbiroRequestHandlerArgs(browserControl, browser, frame, request, response));
            }

            return null;
        }
    }
}
