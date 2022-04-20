using CefSharp;
using HanbiroExtensionGUI.Controls.ChromiumBrowser;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.CookieManagement;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services;
using HanbiroExtensionGUI.Services.JobSchedulerServices;
using HanbiroExtensionGUI.Services.Telegram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace HanbiroExtensionGUI
{
    public class App : IApp
    {
        #region Fields
        private string appSettingsPath = @"AppSettings.json";
        private TelegramService telegramService;
        private HanbiroChromiumBrowser chromiumBrowser;
        private HanbiroChromiumBrowser chromiumBrowserCookie;
        private TelegramHandlers telegramHandlers;
        private JobSchedulerService jobSchedulerService;
        private AppSettings appSettings;
        private Queue<(User, string, ActionStatus, ErrorType)> MessageQueue = new Queue<(User, string, ActionStatus, ErrorType)>();
        private Queue<User> UsersCookie = new Queue<User>();
        private System.Timers.Timer timer = new System.Timers.Timer();
        private frmMain frmMain;
        #endregion

        #region Properties
        public TelegramService TelegramService { get => telegramService; }
        public HanbiroChromiumBrowser ChromiumBrowser { get => chromiumBrowser; }
        private List<User> allUsers => appSettings.Users;
        #endregion

        #region Constructors
        public App(frmMain frmMain)
        {
            this.frmMain = frmMain;
            InitVariables();
            InitEvents();
        }
        #endregion

        #region Events
        private void ChromiumBrowser_OnSavedCookie(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            SaveAppSettings();
            Console.WriteLine(DateTime.Now.ToString() + $"-Stop-{e.ClockType}-{e.User.UserName}");
        }

        private void ChromiumBrowser_OnSuccess(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            StringBuilder message = new StringBuilder();
            string clockTypeString = string.Empty;
            switch (e.ClockType)
            {
                case Enums.ClockType.In:
                    clockTypeString = "Clock In";
                    break;
                case Enums.ClockType.Out:
                    clockTypeString = "Clock Out";
                    break;
            }
            message.AppendLine($"{clockTypeString} at {DateTime.Now.ToString()}");
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Success, ErrorType.None));
            telegramService.SendMessageToAdminitrators(message.ToString(), e.User);

            Console.WriteLine($"Success : {e.User.UserName} - {e.Message.ToString()}");

            jobSchedulerService.ClockInOut();
        }

        private void ChromiumBrowser_OnError(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Some thing went wrong!!!");
            message.AppendLine("Please contact me at https://t.me/quangpv598");
            message.AppendLine("You need Check In/Out yourself.");
            message.AppendLine($"Visit {appSettings.BaseUrl}");
            message.AppendLine($"[Message : {e.Message}]");
            
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Error, e.Type));

            Console.WriteLine($"Error : {e.User.UserName} {e.Message.ToString()}");

            telegramService.SendMessageToAdminitrators(message.ToString(), e.User);

            jobSchedulerService.ClockInOut();
        }
        private void ChromiumBrowser_OnBrowserReady(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            jobSchedulerService.InitSchedulerAsync();
        }

        private void TelegramHandlers_OnUpdatingUser(object sender, User e)
        {
            SaveAppSettings();
        }

        private void TelegramHandlers_OnAddingUser(object sender, User e)
        {
            appSettings.Users.Add(e);
            SaveAppSettings();
        }

        private void ChromiumBrowserCookie_OnSavedCookie(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            SaveAppSettings();
            chromiumBrowserCookie.IsFree = true;
            telegramService.SendLoginSuccess(e.User);
            SaveAppSettings();
        }
        private void ChromiumBrowserCookie_OnError(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            chromiumBrowserCookie.IsFree = true;
            telegramService.SendTryAgain(e.User);
            SaveAppSettings();
        }

        private void ChromiumBrowserCookie_OnBrowserReady(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += (s, e) =>
            {
                var usersCookie = appSettings.Users.Where(u => u.IsActive
                && !string.IsNullOrEmpty(u.UserName) && !string.IsNullOrEmpty(u.Password)
                && string.IsNullOrEmpty(u.Cookie));
                foreach (var user in usersCookie)
                {
                    if (!UsersCookie.Contains(user))
                    {
                        UsersCookie.Enqueue(user);
                    }
                }
            };
            timer.Start();

            var timer2 = new System.Timers.Timer();
            timer2.Interval = 5000;
            timer2.Elapsed += (s, e) =>
            {
                if (UsersCookie.Count > 0)
                {
                    var user = UsersCookie.Dequeue();
                    if (chromiumBrowserCookie.IsFree)
                    {
                        chromiumBrowserCookie.IsFree = false;
                        chromiumBrowserCookie.LoadUserCookie(user);
                    }
                }
            };
            timer2.Start();
        }

        private void JobSchedulerService_OnLogMessage(object sender, string e)
        {
            telegramService.SendMessageToAdminitrators(e);
        }

        #endregion

        #region Methods
        private void InitComponents()
        {
            chromiumBrowser.Dock = DockStyle.Fill;
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(chromiumBrowser);

            chromiumBrowserCookie.Dock = DockStyle.Fill;
            var panel2 = new Panel();
            panel2.Dock = DockStyle.Fill;
            panel2.Controls.Add(chromiumBrowserCookie);

            var tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            var tab1 = new TabPage();
            tab1.Text = "Job";
            tab1.Controls.Add(panel);

            var tab2 = new TabPage();
            tab2.Text = "Cookie";
            tab2.Controls.Add(panel2);

            tabControl.TabPages.Add(tab1);
            tabControl.TabPages.Add(tab2);

            frmMain.Controls.Add(tabControl);
        }
        private void InitVariables()
        {
            appSettings = LoadAppSettings();
            telegramHandlers = new TelegramHandlers(allUsers);
            telegramService = new TelegramService(appSettings.TelegramToken, telegramHandlers, appSettings.Adminítrators);
            chromiumBrowser = new HanbiroChromiumBrowser(appSettings.BaseUrl, isGetCookie: false);
            chromiumBrowserCookie = new HanbiroChromiumBrowser(appSettings.BaseUrl, isGetCookie: true);
            jobSchedulerService = new JobSchedulerService(appSettings.TimeWork, appSettings.Users, chromiumBrowser);

            timer.Interval = 10000;
            timer.Elapsed += (s, e) =>
            {
                while (MessageQueue.Count > 0)
                {
                    var message = MessageQueue.Dequeue();

                    if (message.Item3 == ActionStatus.Error)
                    {
                        if (message.Item4 == ErrorType.WrongUsernameOrPassword)
                        {
                            telegramService.LogoutUser(message.Item1, message.Item2);

                            var user = message.Item1;

                            user.LastCommand = string.Empty;
                            user.IsActive = false;

                            SaveAppSettings();
                        }
                        else
                        {
                            telegramService.SendMessageToUser(message.Item1, message.Item2);
                        }
                    }
                    else
                    {
                        telegramService.SendMessageToUser(message.Item1, message.Item2);
                    }
                }
            };
            timer.Start();

            InitComponents();
        }
        private void InitEvents()
        {
            chromiumBrowser.OnSuccess += ChromiumBrowser_OnSuccess;
            chromiumBrowser.OnError += ChromiumBrowser_OnError;
            chromiumBrowser.OnSavedCookie += ChromiumBrowser_OnSavedCookie;
            chromiumBrowser.OnBrowserReady += ChromiumBrowser_OnBrowserReady;

            chromiumBrowserCookie.OnBrowserReady += ChromiumBrowserCookie_OnBrowserReady;
            chromiumBrowserCookie.OnSavedCookie += ChromiumBrowserCookie_OnSavedCookie;
            chromiumBrowserCookie.OnError += ChromiumBrowserCookie_OnError;

            telegramHandlers.OnAddingUser += TelegramHandlers_OnAddingUser;
            telegramHandlers.OnUpdatingUser += TelegramHandlers_OnUpdatingUser;

            jobSchedulerService.OnLogMessage += JobSchedulerService_OnLogMessage;     
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
