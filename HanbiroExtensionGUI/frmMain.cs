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
            if (CurrentUserSettings == null
                || string.IsNullOrEmpty(CurrentUserSettings?.UserName)
                || string.IsNullOrEmpty(CurrentUserSettings?.Password))
            {
                MessageBox.Show("Please fill your username and password!!!", "Notification");
                return;
            }
            SaveUserSettings();
            EnableControl(true);

            ShutdownScheduler();
            InitSchedulerAsync();
        }

        private void btnSaveSettings_ClickAsync(object sender, EventArgs e)
        {
            SaveUserSettings();
            MessageBox.Show("Success!!!", "Notification");
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
            if (scheduler != null)
                await scheduler.Shutdown();
        }
        private async Task InitSchedulerAsync()
        {
            //Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<CheckInCheckOutJob>()
                .WithIdentity("job1", "group1")
                .Build();

            string cronExpressionEndTime = GetExpressionForEndTime();
            string cronExpressionStartTime = GetExpressionForStartTime();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithCronSchedule(cronExpressionStartTime)
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartNow()
                .WithCronSchedule(cronExpressionEndTime)
                .Build();

            var triggerSet = new HashSet<ITrigger>();
            triggerSet.Add(trigger);
            triggerSet.Add(trigger2);
            await scheduler.ScheduleJob(job, triggerSet, true);
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
                dtpStartTime.Value = CurrentUserSettings.StartTime;
                dtpEndTime.Value = CurrentUserSettings.EndTime;
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

            string json = JsonSerializer.Serialize(CurrentUserSettings);
            File.WriteAllText(uesrSettingsPath, json);
            File.Encrypt(uesrSettingsPath);
        }

        private string GetExpressionForDayMonthYear()
        {
            return "? * " + string.Join(",", CurrentUserSettings.DaysOfWeek.Where(d => d.Value == true)
                .Select(d => d.Key.ToString().Substring(0, 3).ToUpper()));
        }

        private string GetExpressionForStartTime()
        {
            return $"0 {CurrentUserSettings.StartTime.Minute} {CurrentUserSettings.StartTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        private string GetExpressionForEndTime()
        {
            return $"0 {CurrentUserSettings.EndTime.Minute} {CurrentUserSettings.EndTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        #endregion
    }
}
