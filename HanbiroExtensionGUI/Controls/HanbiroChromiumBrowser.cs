using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI.Controls
{
    public class HanbiroChromiumBrowser : ChromiumWebBrowser
    {
        #region Fields
        int countLoaded = 0;
        private UserSettings userSettings;
        #endregion

        #region Constructors
        public HanbiroChromiumBrowser(string address, UserSettings userSettings) : base(address)
        {
            this.userSettings = userSettings;
            this.FrameLoadEnd += HanbiroChromiumBrowser_FrameLoadEnd;
        }
        #endregion

        #region Events

        private void HanbiroChromiumBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (countLoaded == 2)
            {
                var task = new Task(FillUserName);
                task.Start();
            }
            if (countLoaded == 3)
            {
                var task = new Task(ClickUserInfoPanel);
                task.Start();
            }

            countLoaded++;
        }
        #endregion

        #region Methods

        private void ClickUserInfoPanel()
        {
            Thread.Sleep(2000);
            this.EvaluateScriptAsync("document.getElementsByClassName('user-info')[0].click();").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    var task = new Task(CheckInCheckOut);
                    task.Start();
                }
                else
                {
                    Console.WriteLine("Click vao User Info Panel khong thanh cong");
                }
            });
        }

        private void CheckInCheckOut()
        {
            Thread.Sleep(5000);
            this.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[0].innerText;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    string clockInLabel = string.Empty;
                    clockInLabel = response.Result.ToString();

                    this.EvaluateScriptAsync("document.getElementsByClassName('tertiary-info text-center')[1].innerText;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            string clockOutLabel = string.Empty;
                            clockOutLabel = response.Result.ToString();

                            if ((string.IsNullOrEmpty(clockInLabel) && !string.IsNullOrEmpty(clockOutLabel))
                            || (string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel)))
                            {
                                this.EvaluateScriptAsync("document.getElementsByClassName('btn btn-primary btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x => {
                                    var response = x.Result;
                                    if (response.Success)
                                    {
                                        // clock in thành công
                                        countLoaded = 0;
                                    }
                                    else
                                    {
                                        // clock in không thành công
                                    }
                                });
                            }
                            else if (!string.IsNullOrEmpty(clockInLabel) && string.IsNullOrEmpty(clockOutLabel))
                            {
                                this.EvaluateScriptAsync("document.getElementsByClassName('btn btn-danger btn-round no-border width-100 btn-sm')[0].click();").ContinueWith(x => {
                                    var response = x.Result;
                                    if (response.Success)
                                    {
                                        // clock out thành công
                                        countLoaded = 0;
                                    }
                                    else
                                    {
                                        // clock out không thành công
                                    }
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine("Lay thoi gian clock out khong thanh cong");
                        }
                    });
                }
                else
                {
                    Console.WriteLine("Lay thoi gian clock in khong thanh cong");
                }
            });
        }

        private void FillUserName()
        {
            Thread.Sleep(2000);

            SendKeys(this, userSettings.UserName);

            Thread.Sleep(1000);

            var frame = GetFrame(this, 0);
            frame.EvaluateScriptAsync("document.getElementById('log-userid').value;").ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    if (response.Result.ToString() == userSettings.UserName)
                    {
                        var task = new Task(FillPassword);
                        task.Start();
                    }
                    else
                    {
                        // chưa nhập được tên người dùng
                        Console.WriteLine("Chua nhap dung ten nguoi dung");
                    }
                }
            });
        }

        private void FillPassword()
        {
            Thread.Sleep(200);

            var frame = GetFrame(this, 1);
            frame.EvaluateScriptAsync(string.Format("document.getElementsByClassName('form-control key')[0].value = '{0}';", userSettings.Password)).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    frame.EvaluateScriptAsync("document.getElementsByClassName('form-control key')[0].value;").ContinueWith(x =>
                    {
                        var response = x.Result;

                        if (response.Success && response.Result != null)
                        {
                            if (response.Result.ToString() == userSettings.Password)
                            {
                                var task = new Task(PressLogin);
                                task.Start();
                            }
                            else
                            {
                                // chưa nhập được mật khẩu
                                Console.WriteLine("Chua nhap dung mat khau");
                            }
                        }
                    });
                }
                else
                {
                    Console.WriteLine("Gan mat khau vao o text khong thanh cong");
                }
            });
        }

        private void PressLogin()
        {
            Thread.Sleep(200);

            var frame = GetFrame(this, 0);
            frame.EvaluateScriptAsync("document.getElementById('btn-log').click();").ContinueWith(x => {

                var response = x.Result;

                if (response.Success)
                {
                    // bấm nút đăng nhập thành công
                }
                else
                {
                    // bấm nút đăng nhập không thành công
                    Console.WriteLine("Bam nut dang nhap khong thanh cong");
                }
            });
        }

        #endregion

        #region Utils
        public static IFrame GetFrame(ChromiumWebBrowser browser, int indexs)
        {
            var identifiers = browser.GetBrowser().GetFrameIdentifiers();
            return browser.GetBrowser().GetFrame(identifiers[indexs]);
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        static string GetCharsFromKeys(Keys keys, bool shift, bool altGr)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;
            if (altGr)
            {
                keyboardState[(int)Keys.ControlKey] = 0xff;
                keyboardState[(int)Keys.Menu] = 0xff;
            }
            ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }

        static void SendKeys(ChromiumWebBrowser browser, string userId)
        {
            List<KeyEvent> events = new List<KeyEvent>();
            foreach (var c in userId)
            {
                var keyEvent = new KeyEvent()
                {
                    FocusOnEditableField = true,
                    WindowsKeyCode = GetCharsFromKeys((Keys)c, false, false)[0],
                    Modifiers = CefEventFlags.None,
                    Type = KeyEventType.Char,
                    IsSystemKey = false
                };
                events.Add(keyEvent);
            }

            foreach (KeyEvent ev in events)
            {
                Thread.Sleep(200);
                browser.GetBrowser().GetHost().SendKeyEvent(ev);
            }
        }
        #endregion
    }
}
