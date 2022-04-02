using CefSharp;
using CefSharp.OffScreen;
using HanbiroExtensionGUI.Controls;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Services;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI
{
    public partial class frmMain : Form
    {
        #region Fields
        public static Models.CurrentUserSettings CurrentUserSettings = new Models.CurrentUserSettings();
        private string uesrSettingsPath = @"UserSettings.json";
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public frmMain()
        {
            InitializeComponent();
            LoadUserSettings();
        }
        #endregion

        #region Events
        private void chkReciveEmailNotifications_CheckedChanged(object sender, EventArgs e)
        {
            txtEmail.Visible = chkReciveEmailNotifications.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SaveUserSettings();
            EnableControl(true);

            //ShutdownScheduler();
            //InitSchedulerAsync();
        }

        private void btnSaveSettings_ClickAsync(object sender, EventArgs e)
        {
            SaveUserSettings();
            MessageBox.Show("Success!!!", "Notification");
        }

        private void btnStop_ClickAsync(object sender, EventArgs e)
        {
            EnableControl(false);
            //ShutdownScheduler();
        }
        #endregion

        #region Methods
        private void EnableControl(bool b)
        {
            groupBoxTimeManagement.Enabled = !b;
            btnStart.Enabled = !b;
            btnStop.Enabled = b;
            btnSaveSettings.Enabled = !b;
        }

      
        private void LoadUserSettings()
        {
            if (File.Exists(uesrSettingsPath))
            {
                var json = File.ReadAllText(uesrSettingsPath);
                CurrentUserSettings = JsonSerializer.Deserialize<Models.CurrentUserSettings>(json);

                dtpStartTime.Value = CurrentUserSettings.TimeWork.StartTime;
                dtpEndTime.Value = CurrentUserSettings.TimeWork.EndTime;
                chkMon.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Monday];
                chkTue.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Tuesday];
                chkWed.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Wednesday];
                chkThu.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Thursday];
                chkFri.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Friday];
                chkSat.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Saturday];
                chkSun.Checked = CurrentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Sunday];
            }
        }
        private void SaveUserSettings()
        {
            CurrentUserSettings = new Models.CurrentUserSettings();

            CurrentUserSettings.TimeWork = new TimeWork()
            {
                StartTime = dtpStartTime.Value,
                EndTime = dtpEndTime.Value,
                DaysOfWeek = new Dictionary<DayOfWeek, bool>()
                {
                    {DayOfWeek.Monday, chkMon.Checked },
                    {DayOfWeek.Tuesday, chkTue.Checked },
                    {DayOfWeek.Wednesday, chkWed.Checked },
                    {DayOfWeek.Thursday, chkThu.Checked },
                    {DayOfWeek.Friday, chkFri.Checked },
                    {DayOfWeek.Saturday, chkSat.Checked },
                    {DayOfWeek.Sunday, chkSun.Checked }
                }
            };

            CurrentUserSettings.Users.Add(new User {
                UserName = txtUsername.Text,
                Password = txtPassword.Text,
                Email = txtEmail.Text,
                IsSendResultToEmail = chkReciveEmailNotifications.Checked,
            });

            string json = JsonSerializer.Serialize(CurrentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
        }

        
        #endregion
    }
}
