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
        private readonly App app;
        private UserSettings currentUserSettings => app.CurrentUserSettings;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public frmMain()
        {
            InitializeComponent();
            app = new App();
            if(currentUserSettings is not null)
            {
                LoadUserSettings();
            }
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

            app.JobScheduler.ShutdownScheduler();
            app.JobScheduler.InitSchedulerAsync();
        }

        private void btnSaveSettings_ClickAsync(object sender, EventArgs e)
        {
            SaveUserSettings();
            MessageBox.Show("Success!!!", "Notification");
        }

        private void btnStop_ClickAsync(object sender, EventArgs e)
        {
            EnableControl(false);
            app.JobScheduler.ShutdownScheduler();
        }

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            var user = new User
            {
                UserName = txtUsername.Text,
                Password = txtPassword.Text,
                Email = txtEmail.Text,
                IsSendResultToEmail = chkReciveEmailNotifications.Checked,
            };
            app.CheckPassword(user);
        }

        private void btnUserManagement_Click(object sender, EventArgs e)
        {

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
            dtpStartTime.Value = currentUserSettings.TimeWork.StartTime;
            dtpEndTime.Value = currentUserSettings.TimeWork.EndTime;
            chkMon.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Monday];
            chkTue.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Tuesday];
            chkWed.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Wednesday];
            chkThu.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Thursday];
            chkFri.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Friday];
            chkSat.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Saturday];
            chkSun.Checked = currentUserSettings.TimeWork.DaysOfWeek[DayOfWeek.Sunday];
        }
        private void SaveUserSettings()
        {
            var userSettings = new UserSettings();

            userSettings.TimeWork = new TimeWork()
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

            app.SaveUserSettings(userSettings);
        }

        #endregion

    }
}
