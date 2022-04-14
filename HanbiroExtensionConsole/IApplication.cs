using HanbiroExtensionConsole.Controls.ChromiumBrowser;
using HanbiroExtensionConsole.Models;
using HanbiroExtensionConsole.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole
{
    public interface IApplication
    {
        TelegramService TelegramService { get;}
        HanbiroChromiumBrowser ChromiumBrowser { get;}
        AppSettings LoadAppSettings();
        void SaveAppSettings();
        void Start();
    }
}
