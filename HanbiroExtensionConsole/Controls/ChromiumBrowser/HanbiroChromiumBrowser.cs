using CefSharp;
using CefSharp.OffScreen;
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
        private HanbiroRequestHanlders hanbiroRequestHanlders;
        private LoginExcutor loginExcutor;

        public event EventHandler<LoginExcutorArgs> OnSuccess;
        public event EventHandler<LoginExcutorArgs> OnError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string baseUrl)
        {
            this.baseUrl = baseUrl;
            hanbiroRequestHanlders = new HanbiroRequestHanlders(baseUrl);
            this.RequestHandler = new ChromiumRequestHandler(hanbiroRequestHanlders);
            loginExcutor = new LoginExcutor(this);

            hanbiroRequestHanlders.OnBeforeLoginManually += HanbiroRequestHanlders_OnBeforeLoginManually;

            loginExcutor.OnLoginError += LoginExcutor_OnLoginError;
        }

        #endregion

        #region Events
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

            OnError?.Invoke(this, new LoginExcutorArgs(currentUser));
        }
        #endregion

        #region Methods
        public void ClockIn(User user)
        {
            hanbiroRequestHanlders.SetCurrentUser(user, ClockType.In);
            LoadUserToBrowser(user);
        }
        public void ClockOut(User user)
        {
            hanbiroRequestHanlders.SetCurrentUser(user, ClockType.Out);
            LoadUserToBrowser(user);
        }

        private void LoadUserToBrowser(User user)
        {
            TRIED_TIMES = 0;

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
        }

        private void LoadCookie(User user)
        {
            string cookieStringJson = Encoding.UTF8.GetString(Convert.FromBase64String(user.Cookie));
            var cookie = "{\"cookies\" " + ":" + "" + cookieStringJson + "}";
            dynamic cookieJson = JObject.Parse(cookie);
            var cookies = this.GetCookieManager();
            foreach (var item in cookieJson["cookies"])
            {
                cookies.SetCookie(baseUrl, new CefSharp.Cookie()
                {
                    Name = item["name"],
                    Value = item["value"],
                    Domain = item["domain"],
                    Path = item["path"],
                    HttpOnly = item["httpOnly"],
                    Secure = item["secure"],
                    Expires = item["expires"]
                });
            }
            this.Load(baseUrl);
        }
        #endregion

        #endregion
    }
}
