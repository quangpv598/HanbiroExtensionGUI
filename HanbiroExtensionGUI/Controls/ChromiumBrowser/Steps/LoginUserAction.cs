using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Steps
{
    public class LoginUserAction : AbstractAction
    {
        public LoginUserAction(HanbiroChromiumBrowser hanbiroChromiumBrowser) : base(hanbiroChromiumBrowser)
        {

        }
        public override void DoWork()
        {
            Browser.CheckHealthResult.AppendLineWithShortTime(
                nameof(DoWork),
                true,
                $"Enter to Login Action");
            var task = new Task(FillUserName);
            task.Start();
        }

        private void FillUserName()
        {
            Thread.Sleep(2000);

            Utils.ChromiumBrowserUtils.SendKeys(Browser, UserSettings.UserName);

            Thread.Sleep(1000);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 0);
            frame.EvaluateScriptAsync("document.getElementById('log-userid').value;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    if (response.Result.ToString() == UserSettings.UserName)
                    {
                        Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(FillUserName),
                            true,
                            $"Fill Username Completed");
                        var task = new Task(FillPassword);
                        task.Start();
                    }
                    else
                    {
                        Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(FillUserName),
                            false,
                            $"Username is not match with input");
                    }
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserName),
                        false,
                        $"Can't access element with id 'log-userid'");
                }
            });
        }

        private void FillPassword()
        {
            Thread.Sleep(200);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 1);
            frame.EvaluateScriptAsync(string.Format("document.getElementsByClassName('form-control key')[0].value = '{0}';", UserSettings.Password)).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    frame.EvaluateScriptAsync("document.getElementsByClassName('form-control key')[0].value;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            if (response.Result.ToString() == UserSettings.Password)
                            {
                                Browser.CheckHealthResult.AppendLineWithShortTime(
                                        nameof(FillUserName),
                                        true,
                                        $"Fill Password Completed");
                                var task = new Task(PressLogin);
                                task.Start();
                            }
                            else
                            {
                                Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(FillUserName),
                            false,
                            $"Password is not match with input");
                            }
                        }
                    });
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserName),
                        false,
                        $"Can't access Password element");
                }
            });
        }

        private void PressLogin()
        {
            Thread.Sleep(200);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 0);
            frame.EvaluateScriptAsync("document.getElementById('btn-log').click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(PressLogin),
                            true,
                            $"Press Login button successfuly");
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserName),
                        false,
                        $"Can't access element with id 'btn-log'");
                }
            });
        }
    }
}
