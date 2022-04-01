using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils
{
    public static class ChromiumBrowserUtils
    {
        public async static Task WaitElement(this ChromiumWebBrowser browser, 
            string element,
            Action timeOutAction, 
            int sequence = 1000,
            int timeout = 120000)
        {
            try
            {
                await TaskWaiter.WaitUntil(async () =>
                {
                    bool isSuccess = false;
                    await browser.EvaluateScriptAsync(element).ContinueWith(x =>
                    {
                        isSuccess = x.Result.Success;
                    });
                    return isSuccess;
                }, sequence, timeout);
            }
            catch (Exception ex)
            {
                timeOutAction?.Invoke();
            }
        }

        public async static Task WaitElement(this IFrame frame,
            string element,
            Action timeOutAction,
            int sequence = 1000,
            int timeout = 120000)
        {
            try
            {
                await TaskWaiter.WaitUntil(async () =>
                {
                    bool isSuccess = false;
                    await frame.EvaluateScriptAsync(element).ContinueWith(x =>
                    {
                        isSuccess = x.Result.Success;
                    });
                    return isSuccess;
                }, sequence, timeout);
            }
            catch (Exception ex)
            {
                timeOutAction?.Invoke();
            }
        }

        public static IFrame GetFrame(ChromiumWebBrowser browser, int indexs)
        {
            var identifiers = browser.GetBrowser().GetFrameIdentifiers().OrderBy(id => id).ToList();
            return browser.GetBrowser().GetFrame(identifiers[indexs]);
        }

        public static int GetFramesCount(ChromiumWebBrowser browser)
        {
            var identifiers = browser.GetBrowser().GetFrameIdentifiers();
            return browser.GetBrowser().GetFrameCount();
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        public static string GetCharsFromKeys(Keys keys, bool shift, bool altGr)
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

        public static void SendKeys(ChromiumWebBrowser browser, string userId)
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
    }
}
