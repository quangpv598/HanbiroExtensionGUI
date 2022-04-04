using HanbiroExtensionGUI.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services.JobSchedulerServices
{
    public class JobSchedulerService : ServicesBase
    {
        #region Fields
        private IScheduler scheduler;
        #endregion

        #region Properties
        public CheckInCheckOutService CheckInCheckOutService { get; private set; }
        #endregion

        #region Constructors
        public JobSchedulerService(UserSettings userSettings, CheckInCheckOutService CheckInCheckOutService) : base(userSettings)
        {
            this.CheckInCheckOutService = CheckInCheckOutService;
        }
        #endregion

        #region Events

        #endregion

        #region Methods
        public async Task ShutdownScheduler()
        {
            if (scheduler != null)
                await scheduler.Shutdown();
        }
        public async Task InitSchedulerAsync()
        {
            //Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            var newJobData = new JobDataMap();
            newJobData.Add(nameof(CheckInCheckOutService), CheckInCheckOutService);

            IJobDetail job = JobBuilder.Create<HanbiroJob>()
                .WithIdentity("job1", "group1")
                .UsingJobData(newJobData)
                .Build();

            string cronExpressionEndTime = GetExpressionForEndTime();
            string cronExpressionStartTime = GetExpressionForStartTime();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever())
                //.WithCronSchedule(cronExpressionStartTime)
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


        private string GetExpressionForDayMonthYear()
        {
            return "? * " + string.Join(",", CurrentUserSettings.TimeWork.DaysOfWeek.Where(d => d.Value == true)
                .Select(d => d.Key.ToString().Substring(0, 3).ToUpper()));
        }

        private string GetExpressionForStartTime()
        {
            return $"0 {CurrentUserSettings.TimeWork.StartTime.Minute} {CurrentUserSettings.TimeWork.StartTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        private string GetExpressionForEndTime()
        {
            return $"0 {CurrentUserSettings.TimeWork.EndTime.Minute} {CurrentUserSettings.TimeWork.EndTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        #endregion
    }
}
