using CefSharp;
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

        private async void FillUserName()
        {
            try
            {
                await TaskWaiter.WaitUntil(async () =>
                {
                    bool isSuccess = false;
                    await Browser.EvaluateScriptAsync("document.getElementById('log-userid').value;").ContinueWith(x =>
                    {
                        isSuccess = x.Result.Success;
                    });
                    return isSuccess;
                }, 1000, 5000);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent();
            }
            
            Thread.Sleep(1000);

            Utils.ChromiumBrowserUtils.SendKeys(Browser, UserSettings.UserName);

            Thread.Sleep(1000);

            await Browser.EvaluateScriptAsync("document.getElementById('log-userid').value;").ContinueWith(x =>
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
                        var task = new Task(FillPasswordAsync);
                        task.Start();
                    }
                    else
                    {
                        Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(FillUserName),
                            false,
                            $"Username is not match with input");
                        RaiseErrorEvent();
                    }
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserName),
                        false,
                        $"Can't access element with id 'log-userid'");
                    RaiseErrorEvent();
                }
            });
        }

        private async void FillPasswordAsync()
        {
            await TaskWaiter.WaitUntil(() => Task.FromResult(ChromiumBrowserUtils.GetFramesCount(Browser) > 1), 1000, 5000);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 1);
            await frame.EvaluateScriptAsync(string.Format("document.getElementsByClassName('form-control key')[0].value = '{0}';", UserSettings.Password)).ContinueWith(x =>
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
                                        nameof(FillPasswordAsync),
                                        true,
                                        $"Fill Password Completed");
                                var task = new Task(PressLogin);
                                task.Start();
                            }
                            else
                            {
                                Browser.CheckHealthResult.AppendLineWithShortTime(
                                        nameof(FillPasswordAsync),
                                        false,
                                        $"Password is not match with input");
                                RaiseErrorEvent();
                            }
                        }
                    });
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillPasswordAsync),
                        false,
                        $"Can't access Password element");
                    RaiseErrorEvent();
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
                        nameof(PressLogin),
                        false,
                        $"Can't access element with id 'btn-log'");
                    RaiseErrorEvent();
                }
            });
        }
    }
}
