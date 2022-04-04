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
        }
        #endregion

        #region Events
        
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
        public void SaveUserSettings(UserSettings settings)
        {
            currentUserSettings = settings;
            if (jobScheduler == null)
            {
                jobScheduler = new JobSchedulerService(currentUserSettings);
                checkingPasswordService = new CheckingPasswordService(currentUserSettings);
                checkingPasswordService.OnSuccess += CheckingPasswordService_OnSuccess;
                checkingPasswordService.OnError += CheckingPasswordService_OnError;
            }
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(uesrSettingsPath, json);
        }

        private void CheckingPasswordService_OnError(object sender, EventArgs e)
        {
            MessageBox.Show("Login Fail.");
        }

        private void CheckingPasswordService_OnSuccess(object sender, EventArgs e)
        {
            MessageBox.Show("Login Success.");
        }

        public async void CheckPassword(User user)
        {
            await checkingPasswordService.DoWorkAsync(user);
        }


        #endregion
    }
}
