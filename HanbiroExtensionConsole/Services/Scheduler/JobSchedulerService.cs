﻿using HanbiroExtensionConsole.Controls.ChromiumBrowser;
using HanbiroExtensionConsole.Enums;
using HanbiroExtensionConsole.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Services.JobSchedulerServices
{
    public class JobSchedulerService 
    {
        #region Fields
        private IScheduler scheduler;
        private readonly TimeWork timeWork;
        private Queue<(User, ClockType)> Users = new Queue<(User, ClockType)>();
        private readonly HanbiroChromiumBrowser chromiumBrowser;
        private List<User> allUsers;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public JobSchedulerService(TimeWork timeWork, List<User> allUsers, HanbiroChromiumBrowser chromiumBrowser)
        {
            this.timeWork = timeWork;
            this.allUsers = allUsers;
            this.chromiumBrowser = chromiumBrowser;
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
            newJobData.Add(nameof(JobSchedulerService), this);

            IJobDetail job = JobBuilder.Create<HanbiroJob>()
                .WithIdentity("job1", "group1")
                .UsingJobData(newJobData)
                .Build();

            string cronExpressionStartTime = GetExpressionForStartTime();
            string cronExpressionEndTime = GetExpressionForEndTime();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithCronSchedule(cronExpressionStartTime)
                .UsingJobData(nameof(ClockType), nameof(ClockType.In))
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartNow()
                .WithCronSchedule(cronExpressionEndTime)
                .UsingJobData(nameof(ClockType), nameof(ClockType.Out))
                .Build();

            var triggerSet = new HashSet<ITrigger>();
            triggerSet.Add(trigger);
            triggerSet.Add(trigger2);
            await scheduler.ScheduleJob(job, triggerSet, true);
        }


        private string GetExpressionForDayMonthYear()
        {
            return "? * " + string.Join(",",timeWork.DaysOfWeek.Where(d => d.Value == true)
                .Select(d => d.Key.ToString().Substring(0, 3).ToUpper()));
        }

        private string GetExpressionForStartTime()
        {
            return $"0 {timeWork.StartTime.Minute} {timeWork.StartTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        private string GetExpressionForEndTime()
        {
            return $"0 {timeWork.EndTime.Minute} {timeWork.EndTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        public void Reset(ClockType clockType)
        {
            Console.WriteLine("===========================");
            foreach (var user in allUsers)
            {
                Users.Enqueue((user, clockType));
            }

            ClockInOut();
        }
        public void ClockInOut()
        {
            if (Users.Count == 0) return;
            var item = Users.Dequeue();
            var user = item.Item1;
            var clockType = item.Item2;
            switch (clockType)
            {
                case ClockType.In:
                    chromiumBrowser.ClockIn(user);
                    break;
                case ClockType.Out:
                    chromiumBrowser.ClockOut(user);
                    break;
            }
        }

        #endregion
    }
}
