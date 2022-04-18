using HanbiroExtensionGUI.Controls.ChromiumBrowser;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI
{
    public interface IApp
    {
        TelegramService TelegramService { get;}
        HanbiroChromiumBrowser ChromiumBrowser { get;}
        AppSettings LoadAppSettings();
        void SaveAppSettings();
        void Start();
    }
}
