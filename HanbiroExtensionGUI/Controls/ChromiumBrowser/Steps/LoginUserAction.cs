using CefSharp;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Extensions;
using HanbiroExtensionGUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var task = new Task(FillUserNameWithCookie);
            task.Start();
        }

        #region Fill Username Old Method

        //private async void FocusToUserNameTextbox()
        //{
        //    await Browser.EvaluateScriptAsync(@"
        //                    var play = document.getElementById('log-userid')
        //                    function findPos(obj)
        //                    {
        //                        var curleft = 0;
        //                        var curtop = 0;

        //                        if (obj.offsetParent)
        //                        {
        //                            do
        //                            {
        //                                curleft += obj.offsetLeft;
        //                                curtop += obj.offsetTop;
        //                            } while (obj = obj.offsetParent);

        //                            return { X: curleft,Y: curtop};
        //                        }
        //                    }
        //                    findPos(play)"
        //    )
        //    .ContinueWith(x =>
        //    {
        //        // 2. Continue with finding the coordinates and using MouseClick method 
        //        // for pressing left mouse button down and releasing it at desired end position.
        //        var responseForMouseClick = x.Result;

        //        if (responseForMouseClick.Success && responseForMouseClick.Result != null)
        //        {
        //            var xy = responseForMouseClick.Result;
        //            var json = JsonConvert.SerializeObject(xy).ToString();
        //            var coordx = json.Substring(json.IndexOf(':') + 1, 3);
        //            var coordy = json.Substring(json.LastIndexOf(':') + 1, 3);

        //            ChromiumBrowserUtils.MouseLeftDown(Browser, int.Parse(coordx) + 20, int.Parse(coordy) + 20);
        //            ChromiumBrowserUtils.MouseLeftUp(Browser, int.Parse(coordx) + 20, int.Parse(coordy) + 20);

        //            Task task = new Task(FillUserName);
        //            task.Start();
        //        }
        //    });
        //}

        //private async void FillUserName()
        //{
        //    string element = "document.getElementById('log-userid')";
        //    await Browser.WaitElement($"{element}.value;",
        //        () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

        //    Thread.Sleep(500);

        //    Utils.ChromiumBrowserUtils.SendKeys(Browser, CurrentUser.UserName);

        //    Thread.Sleep(500);

        //    await Browser.EvaluateScriptAsync($"{element}.value;").ContinueWith(x =>
        //    {
        //        var response = x.Result;

        //        if (response.Success && response.Result != null)
        //        {
        //            if (response.Result.ToString() == CurrentUser.UserName)
        //            {
        //                Browser.CheckHealthResult.AppendLineWithShortTime(
        //                    nameof(FillUserName),
        //                    true,
        //                    $"Fill Username Completed");
        //                var task = new Task(FillPasswordAsync);
        //                task.Start();
        //            }
        //            else
        //            {
        //                Browser.CheckHealthResult.AppendLineWithShortTime(
        //                    nameof(FillUserName),
        //                    false,
        //                    $"Username is not match with input");
        //                RaiseErrorEvent(new ErrorArgs(ErrorType.NotMatchWithInput, element));
        //            }
        //        }
        //        else
        //        {
        //            Browser.CheckHealthResult.AppendLineWithShortTime(
        //                nameof(FillUserName),
        //                false,
        //                $"Can't access element with id 'log-userid'");
        //            RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element));
        //        }
        //    });
        //}

        #endregion

        private async void FillUserNameWithCookie()
        {
            string script = $"document.cookie = 'auto_save_id={CurrentUser.UserName}; expires=2074-08-09T11:28:22.756Z; path=/'";

            await Browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(FillUserNameWithCookie),
                            true,
                            $"Fill Username Completed");

                    BeforeFillPassword();
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserNameWithCookie),
                        false,
                        $"Can't set cookie for username");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, script));
                }
            });
        }

        private async void BeforeFillPassword()
        {
            string element = "document.getElementById('log-userid')";
            await Browser.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            var task = new Task(FillPasswordAsync);
            task.Start();
        }

        private async void FillPasswordAsync()
        {
            string element = "document.getElementsByClassName('form-control key')[0]";
            await TaskWaiter.WaitUntil(() => Task.FromResult(ChromiumBrowserUtils.GetFramesCount(Browser) > 1), 1000, 5000);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 1);

            await frame.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await frame.EvaluateScriptAsync($"{element}.value = '{CurrentUser.Password}';").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    frame.EvaluateScriptAsync($"{element}.value;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            if (response.Result.ToString() == CurrentUser.Password)
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
                                RaiseErrorEvent(new ErrorArgs(ErrorType.NotMatchWithInput, element));

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
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }
        private async void PressLogin()
        {
            string element = "document.getElementById('btn-log')";
            await TaskWaiter.WaitUntil(() => Task.FromResult(ChromiumBrowserUtils.GetFramesCount(Browser) > 1), 1000, 5000);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 0);

            await frame.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await frame.EvaluateScriptAsync($"{element}.click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(PressLogin),
                            true,
                            $"Press Login button successfuly");

                    //CheckPassword();

                    CaptureScreenshotAsync();
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(PressLogin),
                        false,
                        $"Can't access element with id 'btn-log'");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }

        private async void CaptureScreenshotAsync()
        {
            var bytes = await Browser.CaptureScreenshotAsync();
            File.WriteAllBytes("afterclicklogin.jpg", bytes);
        }

        private async void CheckPassword()
        {
            string element = "document.getElementsByClassName('alert alert-warning')[0]";
            bool isAccountValid = false;
            await Browser.WaitElement($"{element}.length;",
                () => {

                    Browser.CheckHealthResult.AppendLineWithShortTime(
                            nameof(CheckPassword),
                            false,
                            $"Account is not valid.");

                    CurrentUser.IsLoginSuccess = false;
                    isAccountValid = true;
                }, 500, 2000);

            if (isAccountValid)
            {
                RaiseSuccessEvent();
            }
            else
            {
                RaiseErrorEvent(new ErrorArgs(ErrorType.WrongUsernameOrPassword, "Wrong User Name Or Password"));
            }
        }
    }
}
