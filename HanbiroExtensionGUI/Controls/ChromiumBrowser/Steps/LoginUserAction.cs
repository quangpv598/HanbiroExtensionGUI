using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
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
                        var task = new Task(FillPassword);
                        task.Start();
                    }
                    else
                    {
                        // chưa nhập được tên người dùng
                        Debug.WriteLine("Chua nhap dung ten nguoi dung");
                    }
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
                                var task = new Task(PressLogin);
                                task.Start();
                            }
                            else
                            {
                                // chưa nhập được mật khẩu
                                Debug.WriteLine("Chua nhap dung mat khau");
                            }
                        }
                    });
                }
                else
                {
                    Debug.WriteLine("Gan mat khau vao o text khong thanh cong");
                }
            });
        }

        private void PressLogin()
        {
            Thread.Sleep(200);

            var frame = Utils.ChromiumBrowserUtils.GetFrame(Browser, 0);
            frame.EvaluateScriptAsync("document.getElementById('btn-log').click();").ContinueWith(x => {

                var response = x.Result;

                if (response.Success)
                {
                    // bấm nút đăng nhập thành công
                }
                else
                {
                    // bấm nút đăng nhập không thành công
                    Debug.WriteLine("Bam nut dang nhap khong thanh cong");
                }
            });
        }
    }
}
