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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI
{
    public partial class frmMain : Form
    {
        #region Fields
        private IScheduler scheduler;
        private UserSettings CurrentUserSettings = null;
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
            if(CurrentUserSettings == null 
                || string.IsNullOrEmpty(CurrentUserSettings?.UserName)
                || string.IsNullOrEmpty(CurrentUserSettings?.Password))
            {
                MessageBox.Show("Please fill your username and password!!!", "Notification");
                return;
            }
            EnableControl(true);
            InitSchedulerAsync();
        }

        private void btnSaveSettings_ClickAsync(object sender, EventArgs e)
        {
            SaveUserSettings();
        }

        private void btnStop_ClickAsync(object sender, EventArgs e)
        {
            EnableControl(false);
            ShutdownScheduler();
        }
        #endregion

        #region Methods

        private void EnableControl(bool b)
        {
            panelInput.Enabled = !b;
            btnStart.Enabled = !b;
            btnStop.Enabled = b;
            btnSaveSettings.Enabled = !b;
        }

        private async Task ShutdownScheduler()
        {
            await scheduler.Shutdown();
        }
        private async Task InitSchedulerAsync()
        {
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<CheckInCheckOutJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }
        
        private void LoadUserSettings()
        {
            if (File.Exists(uesrSettingsPath))
            {
                File.Decrypt(uesrSettingsPath);
                var json = File.ReadAllText(uesrSettingsPath);
                CurrentUserSettings = JsonSerializer.Deserialize<UserSettings>(json);

                txtUsername.Text = CurrentUserSettings.UserName;
                txtPassword.Text = CurrentUserSettings.Password;
                dtpStartTime.Text = CurrentUserSettings.StartTime;
                dtpEndTime.Text = CurrentUserSettings.EndTime;
                chkReciveEmailNotifications.Checked = CurrentUserSettings.IsSendResultToEmail;
                txtEmail.Text = CurrentUserSettings.Email;
                chkMon.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Monday];
                chkTue.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Tuesday];
                chkWed.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Wednesday];
                chkThu.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Thursday];
                chkFri.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Friday];
                chkSat.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Saturday];
                chkSun.Checked = CurrentUserSettings.DaysOfWeek[DayOfWeek.Sunday];
            }
        }
        private void SaveUserSettings()
        {
            CurrentUserSettings = new UserSettings()
            {
                UserName = txtUsername.Text,
                Password = txtPassword.Text,
                Email = txtEmail.Text,
                IsSendResultToEmail = chkReciveEmailNotifications.Checked,
                StartTime = dtpStartTime.Text,
                EndTime = dtpEndTime.Text,
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

            string json = JsonSerializer.Serialize(CurrentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
            File.Encrypt(uesrSettingsPath);
            MessageBox.Show("Success!!!", "Notification");
        }
        
        #endregion
    }
}
