using CefSharp;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.EventsArgs;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils
{
    public class LoginExcutor
    {
        #region Fields
        private User currentUser;
        private HanbiroChromiumBrowser chromiumBrowser;
        public event EventHandler<LoginExcutorArgs> OnLoginError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public LoginExcutor(HanbiroChromiumBrowser chromiumBrowser)
        {
            this.chromiumBrowser = chromiumBrowser;
        }
        #endregion

        #region Events

        #endregion

        #region Methods

        public async void FillUserNameWithCookie(User user)
        {
            string script = $"document.cookie = 'auto_save_id={user.UserName}; expires=2074-08-09T11:28:22.756Z; path=/'";

            await chromiumBrowser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    chromiumBrowser.Reload();
                }
                else
                {
                    OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser));
                }
            });
        }

        public void Excute(User user)
        {
            this.currentUser = user;
            Task task = new Task(BeforeFillPassword);
            task.Start();
        }

        private async void BeforeFillPassword()
        {
            string element = "document.getElementById('log-userid')";
            await chromiumBrowser.WaitElement($"{element}.value;",
                () => OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser)));

            var task = new Task(FillPasswordAsync);
            task.Start();
        }

        private async void FillPasswordAsync()
        {
            string element = "document.getElementsByClassName('form-control key')[0]";
            await TaskWaiter.WaitUntil(() => Task.FromResult(ChromiumBrowserUtils.GetFramesCount(chromiumBrowser) > 1), 1000, 5000);

            var frame = ChromiumBrowserUtils.GetFrame(chromiumBrowser, 1);

            await frame.WaitElement($"{element}.value;",
                () => OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser)));

            string decryptedPassword = EncryptionUtils.Decrypt(currentUser.Password);

            await frame.EvaluateScriptAsync($"{element}.value = '{decryptedPassword}';").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    frame.EvaluateScriptAsync($"{element}.value;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            if (response.Result.ToString() == decryptedPassword)
                            {
                                var task = new Task(PressLogin);
                                task.Start();
                            }
                            else
                            {
                                OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser));
                            }
                        }
                    });
                }
                else
                {
                    OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser));
                }
            });
        }
        private async void PressLogin()
        {
            string element = "document.getElementById('btn-log')";
            await TaskWaiter.WaitUntil(() => Task.FromResult(ChromiumBrowserUtils.GetFramesCount(chromiumBrowser) > 1), 1000, 5000);

            var frame = ChromiumBrowserUtils.GetFrame(chromiumBrowser, 0);

            await frame.WaitElement($"{element}.value;",
                () => OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser)));

            await frame.EvaluateScriptAsync($"{element}.click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {

                }
                else
                {
                    OnLoginError?.Invoke(this, new LoginExcutorArgs(currentUser));
                }
            });
        }
        #endregion

    }
}
