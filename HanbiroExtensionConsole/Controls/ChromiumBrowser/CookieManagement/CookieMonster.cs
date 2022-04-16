using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Controls.ChromiumBrowser.CookieManagement
{
    /// <summary>
    /// CefSharp's cookie operation class
    /// </summary>
    public class CookieMonster : ICookieVisitor
    {
        public List<CefSharp.Cookie> cookies = new List<CefSharp.Cookie>();
        readonly ManualResetEvent gotAllCookies = new ManualResetEvent(false);
        public ChromiumWebBrowser browser;

        /// <summary>
        /// Get the cookie
        /// </summary>
        /// <param name="DomainStr">According to the domain name, if the DomainStr is empty, get all the cookies</param>
        /// <returns></returns>
        public List<CefSharp.Cookie> GetCookieList(string DomainStr = "")
        {
            var visitor = new CookieMonster();
            if (DomainStr.Length > 0)
            {
                var cookieManager = browser.GetCookieManager();
                if (cookieManager.VisitAllCookies(visitor))
                {
                    visitor.WaitForAllCookies();
                    return visitor.cookies.ToList();
                }
                else
                {
                    return visitor.cookies;
                }
            }
            else
            {
                var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                if (cookieManager.VisitAllCookies(visitor))
                    visitor.WaitForAllCookies();
                return visitor.cookies;
            }
        }

        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            cookies.Add(cookie);
            if (count == total - 1)
                gotAllCookies.Set();
            return true;
        }

        public void WaitForAllCookies()
        {
            gotAllCookies.WaitOne();
        }

        public void Dispose()
        {
            //cookies.Clear();
        }
    }
}
