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
    public class ClockInOutAction : AbstractAction
    {
        public ClockInOutAction(HanbiroChromiumBrowser hanbiroChromiumBrowser) : base(hanbiroChromiumBrowser)
        {
            
        }

        public override void DoWork()
        {
            Browser.CheckHealthResult.AppendLineWithShortTime(
                nameof(DoWork),
                true,
                $"Enter to Clock In Clock Out Action");

            CurrentUser.IsLoginSuccess = true;

            if (Browser.IsCheckAccountValid)
            {
                RaiseSuccessEvent();
            }
            else
            {
                var task = new Task(ClickUserInfoPanel);
                task.Start();
            }
        }


        private async void ClickUserInfoPanel()
        {
            string element = "document.getElementsByClassName('user-info')[0]";
            await Browser.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClickUserInfoPanel),
                                    true,
                                    $"Press User Info Panel Successfully");
                    var task = new Task(CheckInCheckOut);
                    task.Start();
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                        nameof(ClickUserInfoPanel),
                        false,
                        $"Can't access element 'user-info'");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }

        private void CheckInCheckOut()
        {
            GetClockInLabel();
        }

        private async void GetClockInLabel()
        {
            string element = "document.getElementsByClassName('tertiary-info text-center')[0]";
            await Browser.WaitElement($"{element}.innerText;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.innerText;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    string clockInLabel = string.Empty;
                    clockInLabel = response.Result.ToString();

                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(GetClockInLabel),
                                    true,
                                    $"Get Clock In Label Successfully");

                    GetClockOutLabel(clockInLabel);
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(GetClockInLabel),
                                    false,
                                    $"Can Not Get Clock In Label");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }

        private async void GetClockOutLabel(string clockInLabel)
        {
            string element = "document.getElementsByClassName('tertiary-info text-center')[1]";
            await Browser.WaitElement($"{element}.innerText;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.innerText;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    string clockOutLabel = string.Empty;
                    clockOutLabel = response.Result.ToString();

                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(GetClockOutLabel),
                                    true,
                                    $"Get Clock Out Label Successfully");

                    ClockInClockOut(clockInLabel, clockOutLabel);
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(GetClockOutLabel),
                                    false,
                                    $"Can Not Get Clock Out Label");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }

        private void ClockInClockOut(string clockInLabel,string clockOutLabel)
        {
            if ((string.IsNullOrEmpty(clockInLabel) && !string.IsNullOrEmpty(clockOutLabel))
                   || (string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel)))
            {
                if (Browser.IsCheckHealth)
                {
                    CheckClockIn();
                }
                else
                {
                    ClockIn();
                }
            }
            else if (!string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel))
            {
                if (Browser.IsCheckHealth)
                {
                    CheckClockOut();
                }
                else
                {
                    ClockOut();
                }
            }
        }

        private async void CheckClockIn()
        {
            string element = "document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0]";
            await Browser.WaitElement($"{element}.innerText;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.innerText;").ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success && !string.IsNullOrEmpty(response.Result.ToString()))
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClockIn),
                                    true,
                                    $"Clock In Successfully");
                    RaiseSuccessEvent();
                }
            });
        }

        private async void ClockIn()
        {
            string element = "document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0]";
            await Browser.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.click();").ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClockIn),
                                    true,
                                    $"Clock In Successfully");
                    RaiseSuccessEvent();
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClockIn),
                                    false,
                                    $"Can not access element to Clock In");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }
        private async void CheckClockOut()
        {
            string element = "document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0]";
            await Browser.WaitElement($"{element}.innerText;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.innerText;").ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success && !string.IsNullOrEmpty(response.Result.ToString()))
                {
                    RaiseSuccessEvent();
                }
            });
        }
        private async void ClockOut()
        {
            string element = "document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0]";
            await Browser.WaitElement($"{element}.value;",
                () => RaiseErrorEvent(new ErrorArgs(ErrorType.CannotFindElement, element)));

            await Browser.EvaluateScriptAsync($"{element}.click();").ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success)
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClockIn),
                                    true,
                                    $"Clock Out Successfully");
                    RaiseSuccessEvent();
                }
                else
                {
                    Browser.CheckHealthResult.AppendLineWithShortTime(
                                    nameof(ClockIn),
                                    false,
                                    $"Can not access element to Clock Out");
                    RaiseErrorEvent(new ErrorArgs(ErrorType.CannotAccessElement, element));
                }
            });
        }
    }
}
