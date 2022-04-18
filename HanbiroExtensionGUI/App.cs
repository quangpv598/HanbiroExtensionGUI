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
        private TelegramHandlers telegramHandlers;
        private JobSchedulerService jobSchedulerService;
        private AppSettings appSettings;
        private Queue<(User, string, ActionStatus)> MessageQueue = new Queue<(User, string, ActionStatus)>();
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
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Success));

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
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Error));

            Console.WriteLine($"Error : {e.User.UserName} {e.Message.ToString()}");

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

        #endregion

        #region Methods
        private void InitComponents()
        {
            chromiumBrowser.Dock = DockStyle.Fill;
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(chromiumBrowser);
            frmMain.Controls.Add(panel);
        }
        private void InitVariables()
        {
            appSettings = LoadAppSettings();
            telegramHandlers = new TelegramHandlers(allUsers);
            telegramService = new TelegramService(appSettings.TelegramToken, telegramHandlers);
            chromiumBrowser = new HanbiroChromiumBrowser(appSettings.BaseUrl);
            jobSchedulerService = new JobSchedulerService(appSettings.TimeWork, appSettings.Users, chromiumBrowser);

            timer.Interval = 10000;
            timer.Elapsed += (s, e) =>
            {
                while (MessageQueue.Count > 0)
                {
                    var message = MessageQueue.Dequeue();
                    
                    if(message.Item3 == ActionStatus.Error)
                    {
                        telegramService.LogoutUser(message.Item1, message.Item2);
                    }
                    else
                    {
                        telegramService.SendMessageToUser(message.Item1, message.Item2);

                        var user = message.Item1;

                        user.LastCommand = string.Empty;
                        user.IsActive = false;
                        user.UserName = string.Empty;
                        user.Password = string.Empty;
                        user.Cookie = string.Empty;

                        SaveAppSettings();
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

            telegramHandlers.OnAddingUser += TelegramHandlers_OnAddingUser;
            telegramHandlers.OnUpdatingUser += TelegramHandlers_OnUpdatingUser;
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
