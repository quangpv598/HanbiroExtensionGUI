﻿using CefSharp;
using HanbiroExtensionConsole.Constants;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.RequestHandlers;
using HanbiroExtensionConsole.Enums;
using HanbiroExtensionConsole.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public event EventHandler<HanbiroRequestHandlerArgs> OnCallApiError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockIn;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockInError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOut;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutSuccess;
        public event EventHandler<HanbiroRequestHandlerArgs> OnClockOutError;
        public event EventHandler<HanbiroRequestHandlerArgs> OnBrowserReady;
        #endregion

        #region Properties
        public bool HasInit { get; set; } = false;
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

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (request.Url == $"{baseUrl}{ApiResources.Auth}"
                || request.Url == $"{baseUrl}{ApiResources.ClockIn}"
                || request.Url == $"{baseUrl}{ApiResources.ClockOut}")
            {
                var filter = FilterManager.CreateFilter(request.Identifier.ToString());
                return filter;
            }

            return null;
        }
        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            var args = new HanbiroRequestHandlerArgs(currentUser, browserControl, browser, frame, request, response);

            if (HasInit is false)
            {
                if (request.Url == $"{baseUrl}{ApiResources.LoginSignal}")
                {
                    OnBrowserReady?.Invoke(this, args);
                }
                return;
            }

            if (request.Url == $"{baseUrl}{ApiResources.LoginSignal}")
            {
                OnBeforeLoginManually?.Invoke(this, args);
            }
            else if (request.Url == $"{baseUrl}{ApiResources.Auth}")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestJsonFilter;

                    if (filter != null)
                    {
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        string data = encoding.GetString(filter.DataAll.ToArray());

                        dynamic d = JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
                        if (d.success == false)
                        {
                            args.ErrorMessage = d.msg;
                            OnAuthenticateError?.Invoke(this, args);
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Call Api Error : {request.Url}";
                        OnCallApiError?.Invoke(this, args);
                    }
                }
                else
                {
                    args.ErrorMessage = $"Call Api Error : {request.Url}";
                    OnCallApiError?.Invoke(this, args);
                }
            }
            else if (request.Url == $"{baseUrl}{ApiResources.AuthSuccessSignal}")
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
                    args.ErrorMessage = $"Call Api Error : {request.Url}";
                    OnCallApiError?.Invoke(this, args);
                }
            }
            else if (request.Url == $"{baseUrl}{ApiResources.ClockIn}")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestJsonFilter;

                    if (filter != null)
                    {
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        string data = encoding.GetString(filter.DataAll.ToArray());

                        dynamic d = JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
                        if (d.success == true)
                        {
                            OnClockInSuccess?.Invoke(this, args);
                        }
                        else
                        {
                            args.ErrorMessage = d.msg;
                            OnClockInError?.Invoke(this, args);
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Call Api Error : {request.Url}";
                        OnCallApiError?.Invoke(this, args);
                    }
                    
                }
                else
                {
                    args.ErrorMessage = $"Call Api Error : {request.Url}";
                    OnCallApiError?.Invoke(this, args);
                }
            }
            else if (request.Url == $"{baseUrl}{ApiResources.ClockOut}")
            {
                if (response.StatusCode == 200 && response.StatusText == "OK" && response.ErrorCode == CefErrorCode.None)
                {
                    var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestJsonFilter;

                    if (filter != null)
                    {
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        string data = encoding.GetString(filter.DataAll.ToArray());

                        dynamic d = JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter());
                        if (d.success == true)
                        {
                            OnClockOutSuccess?.Invoke(this, args);
                        }
                        else
                        {
                            args.ErrorMessage = d.msg;
                            OnClockOutError?.Invoke(this, args);
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Call Api Error : {request.Url}";
                        OnCallApiError?.Invoke(this, args);
                    }
                }
                else
                {
                    args.ErrorMessage = $"Call Api Error : {request.Url}";
                    OnCallApiError?.Invoke(this, args);
                }
            }
        }
        #endregion
    }
}
