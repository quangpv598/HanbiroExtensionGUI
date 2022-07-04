using CefSharp;
using CefSharp.WinForms;
using HanbiroExtensionGUI.Constants;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.CookieManagement;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser
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
        private readonly TimeWork timeWork;

        public event EventHandler<HanbiroArgs> OnError;
        public event EventHandler<HanbiroArgs> OnSuccess;
        public event EventHandler<HanbiroArgs> OnSavedCookie;
        public event EventHandler<HanbiroArgs> OnBrowserReady;
        #endregion

        #region Properties
        public bool IsFree { get; set; } = true;
        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string baseUrl, TimeWork timeWork) : base(baseUrl)
        {
            this.baseUrl = baseUrl;
            this.timeWork = timeWork;
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
            hanbiroRequestHanlders.OnCallApiError += HanbiroRequestHanlders_OnCallApiError;
            hanbiroRequestHanlders.OnGetCookieDone += HanbiroRequestHanlders_OnGetCookieDone;
            hanbiroRequestHanlders.OnLoginManuallyGetCookie += HanbiroRequestHanlders_OnLoginManuallyGetCookie;

            loginExcutor.OnLoginError += LoginExcutor_OnLoginError;
        }

        #endregion

        #region Events

        private void HanbiroRequestHanlders_OnGetCookieDone(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User, null);
        }

        private void HanbiroRequestHanlders_OnCallApiError(object sender, HanbiroRequestHandlerArgs e)
        {
            OnError?.Invoke(this, new HanbiroArgs(e.User,
                e.ErrorMessage,
                ErrorType.CallApi,
                clockType,
                ActionStatus.Error));
        }

        private void HanbiroRequestHanlders_OnBrowserReady(object sender, HanbiroRequestHandlerArgs e)
        {
            Console.WriteLine("Browser ready!!!");
            hanbiroRequestHanlders.HasInit = true;
            OnBrowserReady?.Invoke(this, null);
        }

        private void HanbiroRequestHanlders_OnClockOutError(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User
                , () => OnError?.Invoke(this, new HanbiroArgs(e.User,
                 e.ErrorMessage,
                 ErrorType.FailToClockOut,
                 clockType,
                 ActionStatus.Error)));
        }

        private void HanbiroRequestHanlders_OnClockOutSuccess(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User,
                () => OnSuccess?.Invoke(this, new HanbiroArgs(e.User,
                "Clock In Success",
                ErrorType.None,
                clockType,
                ActionStatus.Success)));


        }

        private void HanbiroRequestHanlders_OnClockOut(object sender, HanbiroRequestHandlerArgs e)
        {
            if (DateTime.Now.TimeOfDay >= timeWork.EndTime.TimeOfDay)
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
            else
            {
                OnError?.Invoke(this, new HanbiroArgs(e.User,
                "You can not clock out at now.",
                ErrorType.FailToClockOut,
                clockType,
                ActionStatus.Error));
            }
        }

        private void HanbiroRequestHanlders_OnClockInError(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User,
                () => OnError?.Invoke(this, new HanbiroArgs(e.User,
                e.ErrorMessage,
                ErrorType.FailToClockIn,
                clockType,
                ActionStatus.Error)));
        }

        private void HanbiroRequestHanlders_OnClockInSuccess(object sender, HanbiroRequestHandlerArgs e)
        {
            SaveCookie(e.User,
                () => OnSuccess?.Invoke(this, new HanbiroArgs(e.User,
                "Clock Out Success",
                ErrorType.None,
                clockType,
                ActionStatus.Success)));
        }

        private void HanbiroRequestHanlders_OnClockIn(object sender, HanbiroRequestHandlerArgs e)
        {
            if (DateTime.Now.TimeOfDay >= timeWork.StartTime.TimeOfDay)
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
            else
            {
                OnError?.Invoke(this, new HanbiroArgs(e.User,
                "You can not clock in at now.",
                ErrorType.FailToClockOut,
                clockType,
                ActionStatus.Error));
            }
        }

        private void HanbiroRequestHanlders_OnAuthenticateError(object sender, HanbiroRequestHandlerArgs e)
        {
            OnError?.Invoke(this, new HanbiroArgs(e.User,
                e.ErrorMessage,
                ErrorType.WrongUsernameOrPassword,
                clockType,
                ActionStatus.Error));
        }

        private void HanbiroRequestHanlders_OnBeforeLoginManually(object sender, HanbiroRequestHandlerArgs e)
        {
            var currentUser = e.User;
            hanbiroRequestHanlders.IsGetCookie = false;
            loginExcutor.Excute(currentUser);
        }

        private void HanbiroRequestHanlders_OnLoginManuallyGetCookie(object sender, HanbiroRequestHandlerArgs e)
        {
            var currentUser = e.User;
            hanbiroRequestHanlders.IsGetCookie = true;
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

        public void LoadUserCookie(User user)
        {
            var cookies = this.GetCookieManager();
            cookies.DeleteCookies();

            hanbiroRequestHanlders.IsGetCookie = true;
            hanbiroRequestHanlders.GetCookie(user);
            loginExcutor.FillUserNameWithCookie(user);
        }

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
            Task task = new Task(() =>
            {
                Task.Delay(1000);

                TRIED_TIMES = 0;
                hanbiroRequestHanlders.IsGetCookie = false;
                hanbiroRequestHanlders.SetCurrentUser(user, clockType);

                if (string.IsNullOrEmpty(user.Cookie))
                {
                    loginExcutor.FillUserNameWithCookie(user);
                }
                else
                {
                    LoadCookie(user);
                }
            });
            task.Start();
        }

        #region Cookies
        private void SaveCookie(User user, Action action)
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

                    action?.Invoke();

                });
            task.Start();
        }

        private void LoadCookie(User user)
        {

            //delete all cookie before
            var cookies = this.GetCookieManager();
            cookies.DeleteCookies();

            //load cookie
            string cookieStringJson = Encoding.UTF8.GetString(Convert.FromBase64String(user.Cookie));
            var cookie = "{\"cookies\" " + ":" + "" + cookieStringJson + "}";
            dynamic cookieJson = JObject.Parse(cookie);

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
