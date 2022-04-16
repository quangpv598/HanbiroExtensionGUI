using CefSharp;
using HanbiroExtensionConsole.Controls.ChromiumBrowser;
using HanbiroExtensionConsole.Controls.ChromiumBrowser.CookieManagement;
using HanbiroExtensionConsole.Models;
using HanbiroExtensionConsole.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace HanbiroExtensionConsole
{
    public class Application : IApplication
    {
        #region Fields
        private string appSettingsPath = @"AppSettings.json";
        private TelegramService telegramService;
        private HanbiroChromiumBrowser chromiumBrowser;
        private AppSettings appSettings;
        private Queue<User> Users = new Queue<User>();
        #endregion

        #region Properties
        public TelegramService TelegramService { get => telegramService; }
        public HanbiroChromiumBrowser ChromiumBrowser { get => chromiumBrowser;}
        #endregion

        #region Constructors
        public Application()
        {
            InitVariables();
            InitEvents();
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

        private void ChromiumBrowser_OnSavedCookie(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            SaveAppSettings();
            //Console.WriteLine(DateTime.Now.ToString() + $"-Stop-{e.User.UserName}");

            ClockOut();
        }

        private void ChromiumBrowser_OnSuccess(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            
        }

        private void ChromiumBrowser_OnError(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            
        }
        private void ChromiumBrowser_OnBrowserReady(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            DoWork();

            Timer timer = new Timer();
            timer.Interval = 900000; // 15m
            timer.Elapsed += (s, e) => {

                DoWork();
            };
            timer.Start();
        }

        #endregion

        #region Methods

        private void DoWork()
        {
            Console.WriteLine("===========================");
            foreach (var user in appSettings.Users)
            {
                Users.Enqueue(user);
            }

            ClockOut();
        }

        private void ClockOut()
        {
            if (Users.Count == 0) return;
            var firstUser = Users.Dequeue();
            Console.Write(appSettings.Users.IndexOf(firstUser) + "  ");
            chromiumBrowser.ClockOut(firstUser);
        }

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

            chromiumBrowser.OnSuccess += ChromiumBrowser_OnSuccess;
            chromiumBrowser.OnError += ChromiumBrowser_OnError;
            chromiumBrowser.OnSavedCookie += ChromiumBrowser_OnSavedCookie;
            chromiumBrowser.OnBrowserReady += ChromiumBrowser_OnBrowserReady;
        }

        public AppSettings LoadAppSettings()
        {
            var appSettings = new AppSettings();
            if (File.Exists(appSettingsPath))
            {
                var json = File.ReadAllText(appSettingsPath);
                appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            }
            return appSettings;
        }

        public void SaveAppSettings()
        {
            if (appSettings is null)
            {
                appSettings = new AppSettings();
            }

            string json = JsonSerializer.Serialize(appSettings);
            File.WriteAllText(appSettingsPath, json);
        }

        public void Start()
        {

        }
        
        #endregion
    }
}
