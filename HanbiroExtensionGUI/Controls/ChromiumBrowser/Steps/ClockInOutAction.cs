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
            var task = new Task(ClickUserInfoPanel);
            task.Start();
        }


        private async void ClickUserInfoPanel()
        {
            await Browser.WaitElement("document.getElementsByClassName('user-info')[0].value;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('user-info')[0].click();").ContinueWith(x =>
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
                    RaiseErrorEvent();
                }
            });
        }

        private void CheckInCheckOut()
        {
            GetClockInLabel();
        }

        private async void GetClockInLabel()
        {
            await Browser.WaitElement("document.getElementsByClassName('tertiary-info text-center')[0].innerText;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[0].innerText;").ContinueWith(x =>
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
                    RaiseErrorEvent();
                }
            });
        }

        private async void GetClockOutLabel(string clockInLabel)
        {
            await Browser.WaitElement("document.getElementsByClassName('tertiary-info text-center')[1].innerText;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[1].innerText;").ContinueWith(x =>
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
                    RaiseErrorEvent();
                }
            });
        }

        private void ClockInClockOut(string clockInLabel,string clockOutLabel)
        {
            if (Browser.IsCheckHealth)
            {
                CheckClockIn();
                CheckClockOut();
            }
            else
            {
                if ((string.IsNullOrEmpty(clockInLabel) && !string.IsNullOrEmpty(clockOutLabel))
                   || (string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel)))
                {
                    ClockIn();
                }
                else if (!string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel))
                {
                    ClockOut();
                }
            }
        }

        private async void CheckClockIn()
        {
            await Browser.WaitElement("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].innerText;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].innerText;").ContinueWith(x =>
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
            await Browser.WaitElement("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].value;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x =>
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
                    RaiseErrorEvent();
                }
            });
        }
        private async void CheckClockOut()
        {
            await Browser.WaitElement("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].innerText;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].innerText;").ContinueWith(x =>
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
            await Browser.WaitElement("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].value;",
                () => RaiseErrorEvent());

            await Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x =>
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
                    RaiseErrorEvent();
                }
            });
        }
    }
}
