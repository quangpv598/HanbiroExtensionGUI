using CefSharp;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Extensions;
using HanbiroExtensionGUI.Models;
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
            string element = "document.getElementById('log-userid')";
            await Browser.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));
            
            Thread.Sleep(100);

            Utils.ChromiumBrowserUtils.SendKeys(Browser, CurrentUser.UserName);

            Thread.Sleep(100);

            await Browser.EvaluateScriptAsync($"{element}.value;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    if (response.Result.ToString() == CurrentUser.UserName)
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
                        RaiseErrorEvent(new ErrorArgs(ErrorType.NotMatchWithInput, element));
                    }
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(FillUserName),
                        false,
                        $"Can't access element with id 'log-userid'");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element));
                }
            });
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
