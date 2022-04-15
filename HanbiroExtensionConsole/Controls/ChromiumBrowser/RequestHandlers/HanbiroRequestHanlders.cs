﻿using CefSharp;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionConsole.Enums;
using HanbiroExtensionConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Controls.ChromiumBrowser
{
    public class HanbiroRequestHanlders
    {
        #region Fields
        private User currentUser;
        private ClockType clockType;
        private readonly string baseUrl;

        public event EventHandler<HanbiroRequestHandlerArgs> OnBeforeLoginManually;
        public event EventHandler<HanbiroRequestHandlerArgs> OnAuthenticateError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnSaveCookie;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockIn;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOut;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public HanbiroRequestHanlders(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        #endregion

        #region Events

        #endregion

        #region Methods

        public void SetCurrentUser(User user, ClockType clockType)
        {
            this.currentUser = user;
            this.clockType = clockType;
        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            var args = new HanbiroRequestHandlerArgs(currentUser, browserControl, browser, frame, request, response);
            if (request.Url == $"{baseUrl}/ngw/app/lib/css/input-password.css")
            {
                OnBeforeLoginManually?.Invoke(this, args);
            }
            else if (request.Url == $"{baseUrl}/ngw/sign/auth"
                || request.Url == $"{baseUrl}/ngw/app/template/main/dashboard.html")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    if (clockType == ClockType.In)
                    {
                        OnClockIn?.Invoke(this, args);
                    }
                    else if (clockType == ClockType.Out)
                    {
                        OnClockOut?.Invoke(this, args);
                    }
                    else
                    {
                        throw new Exception("Dont support ClockType");
                    }
                }
                else
                {
                    OnAuthenticateError?.Invoke(this, args);
                }
            }
            else if (request.Url == $"{baseUrl}/ngw/timecard/punch_v2/in")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    OnClockInSuccess?.Invoke(this, args);
                }
                else
                {
                    OnClockInError?.Invoke(this, args);
                }

                OnSaveCookie?.Invoke(this, args);
            }
            else if (request.Url == $"{baseUrl}/ngw/timecard/punch_v2/out")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    OnClockOutSuccess?.Invoke(this, args);
                }
                else
                {
                    OnClockOutError?.Invoke(this, args);
                }

                OnSaveCookie?.Invoke(this, args);
            }
        }
        #endregion
    }
}
