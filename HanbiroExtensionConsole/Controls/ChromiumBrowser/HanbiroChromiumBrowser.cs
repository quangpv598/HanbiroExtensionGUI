using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionConsole.Constants;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.CookieManagement;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionConsole.Enums;
using HanbiroExtensionConsole.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Controls.ChromiumBrowser
{
    public class HanbiroChromiumBrowser : ChromiumWebBrowser
    {

        #region Constants
        private readonly int TRY_TIMES = 3;
        private int TRIED_TIMES = 0;
        #endregion

        #region Fields
        private readonly string baseUrl;
        private ClockType clockType;
        private HanbiroRequestHanlders hanbiroRequestHanlders;
        private LoginExcutor loginExcutor;

        public event EventHandler<HanbiroArgs> OnError;
        public event EventHandler<HanbiroArgs> OnSuccess;
        public event EventHandler<HanbiroArgs> OnSavedCookie;
        public event EventHandler<HanbiroArgs> OnBrowserReady;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string baseUrl) : base(baseUrl)
        {
            this.baseUrl = baseUrl;
            hanbiroRequestHanlders = new HanbiroRequestHanlders(baseUrl);
            this.RequestHandler = new ChromiumRequestHandler(hanbiroRequestHanlders);
            loginExcutor = new LoginExcutor(this);

            hanbiroRequestHanlders.OnBeforeLoginManually += HanbiroRequestHanlders_OnBeforeLoginManually;
            hanbiroRequestHanlders.OnAuthenticateError += HanbiroRequestHanlders_OnAuthenticateError;
            hanbiroRequestHanlders.OnClockIn += HanbiroRequestHanlders_OnClockIn;
            hanbiroRequestHanlders.OnClockInSuccess += HanbiroRequestHanlders_OnClockInSuccess;
            hanbiroRequestHanlders.OnClockInError += HanbiroRequestHanlders_OnClockInError;
            hanbiroRequestHanlders.OnClockOut += HanbiroRequestHanlders_OnClockOut;
            hanbiroRequestHanlders.OnClockOutSuccess += HanbiroRequestHanlders_OnClockOutSuccess;
            hanbiroRequestHanlders.OnClockOutError += HanbiroRequestHanlders_OnClockOutError;
            hanbiroRequestHanlders.OnBrowserReady += HanbiroRequestHanlders_OnBrowserReady;

            loginExcutor.OnLoginError += LoginExcutor_OnLoginError;
        }

        private void HanbiroRequestHanlders_OnBrowserReady(object sender, HanbiroRequestHandlerArgs e)
        {
            hanbiroRequestHanlders.HasInit = true;
            OnBrowserReady?.Invoke(this, null);
        }
        #endregion

        #region Events

        private void HanbiroRequestHanlders_OnClockOutError(object sender, HanbiroRequestHandlerArgs e)
        {
            OnError?.Invoke(this, new HanbiroArgs(e.User,
                "ClockOut Fail",
                ErrorType.FailToClockOut,
                clockType,
                ActionStatus.Error));
        }

        private void HanbiroRequestHanlders_OnClockOutSuccess(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User);

            OnSuccess?.Invoke(this, new HanbiroArgs(e.User,
                "Clock In Success",
                ErrorType.None,
                clockType,
                ActionStatus.Success));
        }

        private void HanbiroRequestHanlders_OnClockOut(object sender, HanbiroRequestHandlerArgs e)
        {
            var frame = e.Frame;
            IRequest request = frame.CreateRequest();

            request.Url = $"{baseUrl}{ApiResources.ClockOut}";
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            element.Bytes = Encoding.UTF8.GetBytes(ApiResources.ClockInOutPostPayload);
            request.PostData.AddElement(element);

            frame.LoadRequest(request);
        }

        private void HanbiroRequestHanlders_OnClockInError(object sender, HanbiroRequestHandlerArgs e)
        {
            OnError?.Invoke(this, new HanbiroArgs(e.User,
                "ClockIn Fail",
                ErrorType.FailToClockIn,
                clockType,
                ActionStatus.Error));
        }

        private void HanbiroRequestHanlders_OnClockInSuccess(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User);

            OnSuccess?.Invoke(this, new HanbiroArgs(e.User,
                "Clock Out Success",
                ErrorType.None,
                clockType,
                ActionStatus.Success));
        }

        private void HanbiroRequestHanlders_OnClockIn(object sender, HanbiroRequestHandlerArgs e)
        {
            var frame = e.Frame;
            IRequest request = frame.CreateRequest();

            request.Url = $"{baseUrl}{ApiResources.ClockIn}";
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            element.Bytes = Encoding.UTF8.GetBytes(ApiResources.ClockInOutPostPayload);
            request.PostData.AddElement(element);

            frame.LoadRequest(request);
        }

        private void HanbiroRequestHanlders_OnAuthenticateError(object sender, HanbiroRequestHandlerArgs e)
        {
            OnError?.Invoke(this, new HanbiroArgs(e.User,
                "Wrong username or password",
                ErrorType.WrongUsernameOrPassword,
                clockType,
                ActionStatus.Error));
        }

        private void HanbiroRequestHanlders_OnBeforeLoginManually(object sender, HanbiroRequestHandlerArgs e)
        {
            var currentUser = e.User;
            loginExcutor.Excute(currentUser);
        }

        private void LoginExcutor_OnLoginError(object sender, EventsArgs.LoginExcutorArgs e)
        {
            var currentUser = e.User;

            if (TRIED_TIMES < TRY_TIMES)
            {
                loginExcutor.FillUserNameWithCookie(currentUser);
                TRIED_TIMES++;
                return;
            }

            OnError?.Invoke(this, new HanbiroArgs(currentUser,
                "Fail to login",
                ErrorType.FailToLogin,
                clockType,
                ActionStatus.Error));
        }
        #endregion

        #region Methods
        public void ClockIn(User user)
        {
            clockType = ClockType.In;
            LoadUserToBrowser(user);
        }
        public void ClockOut(User user)
        {
            clockType = ClockType.Out;
            LoadUserToBrowser(user);
        }

        private void LoadUserToBrowser(User user)
        {
            TRIED_TIMES = 0;
            hanbiroRequestHanlders.SetCurrentUser(user, clockType);

            if (string.IsNullOrEmpty(user.Cookie))
            {
                loginExcutor.FillUserNameWithCookie(user);
            }
            else
            {
                LoadCookie(user);
            }
        }

        #region Cookies
        private void SaveCookie(User user)
        {
            Task task = new Task(
                () =>
                {

                    CookieMonster visitor = new CookieMonster();
                    visitor.browser = this;
                    var list = visitor.GetCookieList(baseUrl);

                    if (list.Count(c => c.Name == "auto_save_id") == 0)
                    {
                        list.Add(new Cookie
                        {
                            Name = "auto_save_id",
                            Value = user.UserName,
                            Domain = baseUrl,
                            Path = "/",
                            HttpOnly = false,
                            Secure = false,
                            Expires = new DateTime(2074, 08, 26, 08, 47, 49)
                        });
                    }

                    var json = JsonConvert.SerializeObject(list);
                    string cookieBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                    user.Cookie = cookieBase64String;

                    OnSavedCookie?.Invoke(this, new HanbiroArgs(user,
                        "Cookie saved",
                        ErrorType.None,
                        clockType,
                        ActionStatus.Success));

                });
            task.Start();
        }

        private void LoadCookie(User user)
        {
            string cookieStringJson = Encoding.UTF8.GetString(Convert.FromBase64String(user.Cookie));
            var cookie = "{\"cookies\" " + ":" + "" + cookieStringJson + "}";
            dynamic cookieJson = JObject.Parse(cookie);
            var cookies = this.GetCookieManager();
            foreach (var item in cookieJson["cookies"])
            {
                var cookieObject = new CefSharp.Cookie()
                {
                    Name = item["Name"],
                    Value = item["Value"],
                    Domain = item["Domain"],
                    Path = item["Path"],
                    HttpOnly = item["HttpOnly"],
                    Secure = item["Secure"],
                    Expires = item["Expires"]
                };

                cookies.SetCookie(baseUrl, cookieObject);
            }
            this.Load(baseUrl);
        }
        #endregion

        #endregion
    }
}
