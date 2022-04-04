using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services;
using HanbiroExtensionGUI.Services.JobSchedulerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI
{
    public class App
    {
        #region Fields
        private UserSettings currentUserSettings = null;
        private string uesrSettingsPath = @"UserSettings.json";
        private JobSchedulerService jobScheduler;
        private CheckingPasswordService checkingPasswordService;
        private CheckHealthService healthService;
        private CheckInCheckOutService checkInCheckOutService;
        int countReadyToUse = 0;
        public event EventHandler OnReady;
        #endregion

        #region Properties
        public UserSettings CurrentUserSettings => currentUserSettings;
        public JobSchedulerService JobScheduler => jobScheduler;
        #endregion

        #region Constructors
        public App()
        {
            LoadUserSettings();
            InitCefSharp();
            
            checkingPasswordService = new CheckingPasswordService(currentUserSettings);
            healthService = new CheckHealthService(currentUserSettings);
            checkInCheckOutService = new CheckInCheckOutService(currentUserSettings);

            jobScheduler = new JobSchedulerService(currentUserSettings, checkInCheckOutService);

            checkingPasswordService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;
            healthService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;
            checkInCheckOutService.OnReadyToUse += CheckingPasswordService_OnReadyToUse;

            checkingPasswordService.OnSuccess += CheckingPasswordService_OnSuccess;
            checkingPasswordService.OnError += CheckingPasswordService_OnError;

            checkInCheckOutService.OnSuccess += CheckInCheckOutService_OnSuccess;
            checkInCheckOutService.OnError += CheckInCheckOutService_OnError;
        }

        #endregion

        #region Events
        private void CheckingPasswordService_OnReadyToUse(object sender, EventArgs e)
        {
            countReadyToUse++;
            if (countReadyToUse == 3)
            {
                OnReady?.Invoke(this, new EventArgs());
            }
        }

        private void CheckInCheckOutService_OnError(object sender, EventArgs e)
        {
            var service = sender as CheckInCheckOutService;
            string fileName = string.Format("ERROR_{0}_{1}.txt", service.Browser.CurrentUser.UserName, Guid.NewGuid().ToString());
            File.WriteAllText($"{Application.StartupPath}/Logs/{fileName}", service.Browser.CheckHealthResult.ToString());
        }

        private void CheckInCheckOutService_OnSuccess(object sender, EventArgs e)
        {
            var service = sender as CheckInCheckOutService;
            string fileName = string.Format("SUCCESS_{0}_{1}.txt", service.Browser.CurrentUser.UserName, Guid.NewGuid().ToString());
            File.WriteAllText($"{Application.StartupPath}/Logs/{fileName}", service.Browser.CheckHealthResult.ToString());
        }

        private void CheckingPasswordService_OnError(object sender, EventArgs e)
        {
            MessageBox.Show("Login Fail.");
        }

        private void CheckingPasswordService_OnSuccess(object sender, EventArgs e)
        {
            MessageBox.Show("Login Success.");
        }

        #endregion

        #region Methods

        private void InitCefSharp()
        {
            var settings = new CefSettings();
            settings.DisableGpuAcceleration();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(settings);
        }

        public void LoadUserSettings()
        {
            if (File.Exists(uesrSettingsPath))
            {
                var json = File.ReadAllText(uesrSettingsPath);
                currentUserSettings = JsonSerializer.Deserialize<Models.UserSettings>(json);
            }
        }
        public void SaveUserSettings(TimeWork timeWork = null)
        {
            if(timeWork is not null)
            {
                currentUserSettings.TimeWork = timeWork;
            }
            
            checkInCheckOutService.CurrentUserSettings = currentUserSettings;
            healthService.CurrentUserSettings = currentUserSettings;
            checkingPasswordService.CurrentUserSettings = currentUserSettings;
            jobScheduler.CurrentUserSettings = currentUserSettings;
            string json = JsonSerializer.Serialize(currentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
        }

        public async void CheckPassword(User user)
        {
            await checkingPasswordService.DoWorkAsync(user);
        }

        public async void AddNewUser(User user)
        {
            currentUserSettings.Users.Add(user);
            SaveUserSettings();
        }
        #endregion
    }
}
