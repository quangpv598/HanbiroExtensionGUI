using CefSharp;
using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.RequestHandlers;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser
{
    public class HanbiroRequestHanlders
    {
        #region Fields

        private User currentUser;
        private ClockType clockType;
        private readonly string baseUrl;
        private bool hasClock;

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
        public event EventHandler<HanbiroRequestHandlerArgs> OnGetCookieDone;
        #endregion

        #region Properties
        public bool HasInit { get; set; } = false;
        public bool IsGetCookie { get; set; }
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

        public void GetCookie(User user)
        {
            this.currentUser = user;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (request.Url == $"{baseUrl}{ApiResources.Auth}"
                || request.Url == $"{baseUrl}{ApiResources.ClockIn}"
                || request.Url == $"{baseUrl}{ApiResources.ClockOut}"
                || request.Url == $"{baseUrl}{ApiResources.GetUserInfoSignal}"
                || request.Url == $"{baseUrl}{ApiResources.GetTimeCard}")
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
                        args.ErrorMessage = $"Can not get data from filter : {request.Url}";
                        OnCallApiError?.Invoke(this, args);
                    }
                }
                else
                {
                    args.ErrorMessage = $"Call Api Error : {request.Url}";
                    OnCallApiError?.Invoke(this, args);
                }
            }
            else if (request.Url == $"{baseUrl}{ApiResources.GetUserInfoSignal}")
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
                            if(currentUser.UserName == d.rows?.user_config?.user_data?.id)
                            {
                                currentUser.FullName = d.rows?.user_config?.user_data?.name;
                                currentUser.Email = d.rows?.user_config?.user_data?.email;
                                currentUser.PhoneNumber = d.rows?.user_config?.user_data?.telephone;
                            }
                            else
                            {
                                args.ErrorMessage = "Username is not match with receive data";
                                OnAuthenticateError?.Invoke(this, args);
                            }
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Can not get data from filter : {request.Url}";
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
                    if (IsGetCookie)
                    {
                        OnGetCookieDone?.Invoke(this, args);
                    }
                    else
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
                        if (d.success == true && !string.IsNullOrEmpty(d.rows?.date) && !string.IsNullOrEmpty(d.rows?.time))
                        {
                            currentUser.ClockInTime = DateTime.Now;
                            OnClockInSuccess?.Invoke(this, args);
                        }
                        else
                        {
                            args.ErrorMessage = d.msg;
                            OnClockInError?.Invoke(this, args);
                            //OnClockInSuccess?.Invoke(this, args);
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Can not get data from filter : {request.Url}";
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
                        if (d.success == true && !string.IsNullOrEmpty(d.rows?.date) && !string.IsNullOrEmpty(d.rows?.time))
                        {
                            currentUser.ClockOutTime = DateTime.Now;
                            OnClockOutSuccess?.Invoke(this, args);
                        }
                        else
                        {
                            args.ErrorMessage = d.msg;
                            OnClockOutError?.Invoke(this, args);
                            //OnClockOutSuccess?.Invoke(this, args);
                        }
                    }
                    else
                    {
                        args.ErrorMessage = $"Can not get data from filter : {request.Url}";
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
