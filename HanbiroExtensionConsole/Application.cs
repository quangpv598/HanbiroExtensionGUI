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
    public class Application : IApplication
    {
        #region Fields
        private TelegramService telegramService;
        private HanbiroChromiumBrowser chromiumBrowser;
        private AppSettings appSettings;
        #endregion

        #region Properties
        public TelegramService TelegramService { get => telegramService; }
        public HanbiroChromiumBrowser ChromiumBrowser { get => chromiumBrowser;}
        #endregion

        #region Constructors
        public Application()
        {
            InitEvents();
            InitVariables();
        }
        #endregion

        #region Events
        private void TelegramService_OnAddingUser(object sender, User e)
        {
            SaveAppSettings();
        }
        private void TelegramService_OnEditingUser(object sender, User e)
        {
            SaveAppSettings();
        }
        #endregion

        #region Methods
        private void InitVariables()
        {
            appSettings = LoadAppSettings();
            telegramService = new TelegramService();
            chromiumBrowser = new HanbiroChromiumBrowser(appSettings.BaseUrl);
        }
        private void InitEvents()
        {
            telegramService.OnAddingUser += TelegramService_OnAddingUser;
            telegramService.OnEditingUser += TelegramService_OnEditingUser;
        }

        public AppSettings LoadAppSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveAppSettings()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
