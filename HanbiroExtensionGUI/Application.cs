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
    public class Application
    {
        #region Fields
        public static CurrentUserSettings CurrentUserSettings = new CurrentUserSettings();
        private string uesrSettingsPath = @"UserSettings.json";
        private readonly JobSchedulerService jobScheduler;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public Application()
        {
            LoadUserSettings();
            InitCefSharp();

            jobScheduler = new JobSchedulerService(CurrentUserSettings);
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
                CurrentUserSettings = JsonSerializer.Deserialize<Models.CurrentUserSettings>(json);
            }
        }
        public void SaveUserSettings()
        {
            string json = JsonSerializer.Serialize(CurrentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
        }


        #endregion
    }
}
