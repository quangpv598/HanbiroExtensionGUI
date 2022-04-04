using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services.JobSchedulerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI
{
    public class App
    {
        #region Fields
        private UserSettings currentUserSettings = new UserSettings();
        private string uesrSettingsPath = @"UserSettings.json";
        private readonly JobSchedulerService jobScheduler;
        #endregion

        #region Properties
        public UserSettings CurrentUserSettings => currentUserSettings;
        #endregion

        #region Constructors
        public App()
        {
            LoadUserSettings();
            InitCefSharp();

            jobScheduler = new JobSchedulerService(currentUserSettings);
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
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(uesrSettingsPath, json);
        }


        #endregion
    }
}
