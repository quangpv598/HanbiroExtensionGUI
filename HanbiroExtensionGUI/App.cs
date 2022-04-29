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
        private string appSettingsPath = $"{Application.StartupPath}/AppSettings.json";
        private TelegramService telegramService;
        private HanbiroChromiumBrowser chromiumBrowser;
        private TelegramHandlers telegramHandlers;
        private JobSchedulerService jobSchedulerService;
        private AppSettings appSettings;
        private Queue<(User, string, ActionStatus, ErrorType)> MessageQueue = new Queue<(User, string, ActionStatus, ErrorType)>();
        private List<(User, ActionStatus)> results = new List<(User, ActionStatus)>();
        private Queue<User> UsersCookie = new Queue<User>();
        private System.Timers.Timer timer = new System.Timers.Timer();
        private frmMain frmMain;
        #endregion

        #region Properties
        public TelegramService TelegramService { get => telegramService; }
        public HanbiroChromiumBrowser ChromiumBrowser { get => chromiumBrowser; }
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
                    clockTypeString = $"Clock In at {e.User.ClockInTime.ToString()}";
                    break;
                case Enums.ClockType.Out:
                    clockTypeString = $"Clock Out at {e.User.ClockOutTime.ToString()}";
                    break;
            }
            message.AppendLine(clockTypeString);
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Success, ErrorType.None));
            results.Add((e.User, ActionStatus.Success));
            //telegramService.SendMessageToAdminitrators(message.ToString(), e.User);

            Console.WriteLine($"Success : {e.User.UserName} - {e.Message.ToString()}");

            jobSchedulerService.ClockInOut();
        }

        private void ChromiumBrowser_OnError(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Some thing went wrong!!!");
            //message.AppendLine("Please contact me at https://t.me/quangpv598");
            message.AppendLine("You need Check In/Out yourself.");
            message.AppendLine($"Visit {appSettings.BaseUrl}");
            message.AppendLine($"[Message : {e.Message}]");
            
            MessageQueue.Enqueue((e.User, message.ToString(), ActionStatus.Error, e.Type));
            results.Add((e.User, ActionStatus.Error));

            Console.WriteLine($"Error : {e.User.UserName} {e.Message.ToString()}");

            telegramService.SendMessageToAdminitrators(message.ToString(), e.User);

            jobSchedulerService.ClockInOut();
        }
        private void ChromiumBrowser_OnBrowserReady(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            jobSchedulerService.InitSchedulerAsync(ClockType.None);
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
            if (e.User.IsGettingCookie)
            {
                chromiumBrowser.IsFree = true;
                telegramService.SendLoginSuccess(e.User);
                e.User.IsGettingCookie = false;
                SaveAppSettings();
            }
        }
        private void ChromiumBrowserCookie_OnError(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            if (e.User.IsGettingCookie)
            {
                chromiumBrowser.IsFree = true;
                telegramService.SendTryAgain(e.User);
                e.User.IsGettingCookie = false;
                SaveAppSettings();
            }
        }

        private void ChromiumBrowserCookie_OnBrowserReady(object sender, Controls.ChromiumBrowser.EventsArgs.HanbiroArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("System is started!!!");
            stringBuilder.AppendLine("*******************");
            stringBuilder.AppendLine("Days Off :");
            foreach (var date in appSettings.TimeWork.DaysOff)
            {
                stringBuilder.AppendLine($"- {date.ToString("dd/MM/yyyy")}");
            }
            telegramService.SendMessageToAdminitrators(stringBuilder.ToString());

            var timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += (s, e) =>
            {
                var usersCookie = appSettings.Users.Where(u => u.IsActive
                && !string.IsNullOrEmpty(u.UserName) && !string.IsNullOrEmpty(u.Password)
                && string.IsNullOrEmpty(u.Cookie)
                && u.IsGettingCookie == false);
                foreach (var user in usersCookie)
                {
                    if (!UsersCookie.Contains(user))
                    {
                        telegramService.SendMessageToAdminitrators($"User added to Queue Cookie : {user.UserName}");
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
                    if (chromiumBrowser.IsFree)
                    {
                        telegramService.SendMessageToAdminitrators($"Getting cookie : {user.UserName}");
                        chromiumBrowser.IsFree = false;
                        user.IsGettingCookie = true;
                        chromiumBrowser.LoadUserCookie(user);
                    }
                }
            };
            timer2.Start();

            //var timer3 = new System.Timers.Timer();
            //timer3.Interval = 1800000; // every 30m
            //timer3.Elapsed += (s, e) =>
            //{
            //    telegramService.SendMessageToAdminitrators("System is actived !!!");
            //};
            //timer3.Start();
        }

        private void JobSchedulerService_OnLogMessage(object sender, string e)
        {
            telegramService.SendMessageToAdminitrators(e);
        }

        private void JobSchedulerService_OnClockingStateChanged(object sender, (bool, ClockType) e)
        {
            bool isActive = e.Item1;
            if (isActive)
            {
                results.Clear();
                // reset is free chromium
                chromiumBrowser.IsFree = false;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("****************");
                stringBuilder.AppendLine($"Report at {DateTime.Now.ToString()}");
                stringBuilder.AppendLine($"Total : {results.Count}");
                stringBuilder.AppendLine($"Success : {results.Where(r => r.Item2 == ActionStatus.Success).Count()}");
                stringBuilder.AppendLine($"Error : {results.Where(r => r.Item2 == ActionStatus.Error).Count()}");
                stringBuilder.AppendLine("****************");
                foreach (var item in results)
                {
                    var user = item.Item1;
                    var status = item.Item2;
                    stringBuilder.AppendLine($"*[{status}]-{user.FullName}");
                }
                stringBuilder.AppendLine("****************");
                telegramService.SendMessageToAdminitrators(stringBuilder.ToString());

                // reset is free chromium
                chromiumBrowser.IsFree = true;

                // reset random schedule
                jobSchedulerService.UnScheduler(e.Item2);
                jobSchedulerService.InitSchedulerAsync(e.Item2);
            }
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

            var tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            var tab1 = new TabPage();
            tab1.Text = "Job";
            tab1.Controls.Add(panel);

            tabControl.TabPages.Add(tab1);

            frmMain.Controls.Add(tabControl);
        }
        private void InitVariables()
        {
            appSettings = LoadAppSettings();
            telegramHandlers = new TelegramHandlers(appSettings);
            telegramService = new TelegramService(appSettings.TelegramToken, telegramHandlers, appSettings.Adminitrators);
            telegramHandlers.telegramService = telegramService;
            chromiumBrowser = new HanbiroChromiumBrowser(appSettings.BaseUrl, appSettings.TimeWork);
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

            chromiumBrowser.OnBrowserReady += ChromiumBrowserCookie_OnBrowserReady;
            chromiumBrowser.OnSavedCookie += ChromiumBrowserCookie_OnSavedCookie;
            chromiumBrowser.OnError += ChromiumBrowserCookie_OnError;

            telegramHandlers.OnAddingUser += TelegramHandlers_OnAddingUser;
            telegramHandlers.OnUpdatingUser += TelegramHandlers_OnUpdatingUser;

            jobSchedulerService.OnLogMessage += JobSchedulerService_OnLogMessage;
            jobSchedulerService.OnClockingStateChanged += JobSchedulerService_OnClockingStateChanged;
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
