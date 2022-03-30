using CefSharp;
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
            var task = new Task(ClickUserInfoPanel);
            task.Start();
        }


        private void ClickUserInfoPanel()
        {
            Thread.Sleep(2000);
            Browser.EvaluateScriptAsync("document.getElementsByClassName('user-info')[0].click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    var task = new Task(CheckInCheckOut);
                    task.Start();
                }
                else
                {
                    Debug.WriteLine("Click vao User Info Panel khong thanh cong");
                }
            });
        }

        private void CheckInCheckOut()
        {
            Thread.Sleep(5000);
            Browser.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[0].innerText;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    string clockInLabel = string.Empty;
                    clockInLabel = response.Result.ToString();

                    Browser.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[1].innerText;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            string clockOutLabel = string.Empty;
                            clockOutLabel = response.Result.ToString();

                            if ((string.IsNullOrEmpty(clockInLabel) && !string.IsNullOrEmpty(clockOutLabel))
                            || (string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel)))
                            {
                                //Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x =>
                                //{
                                //    var response = x.Result;
                                //    if (response.Success)
                                //    {
                                //        // clock in thành công
                                        
                                //    }
                                //    else
                                //    {
                                //        // clock in không thành công
                                //    }
                                //});
                            }
                            else if (!string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel))
                            {
                                //Browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x =>
                                //{
                                //    var response = x.Result;
                                //    if (response.Success)
                                //    {
                                //        // clock out thành công
                                        
                                //    }
                                //    else
                                //    {
                                //        // clock out không thành công
                                //    }
                                //});
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Lay thoi gian clock out khong thanh cong");
                        }
                    });
                }
                else
                {
                    Debug.WriteLine("Lay thoi gian clock in khong thanh cong");
                }
            });
        }
    }
}
