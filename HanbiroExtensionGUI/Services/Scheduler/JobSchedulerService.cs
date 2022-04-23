﻿using HanbiroExtensionGUI.Controls.ChromiumBrowser;
using HanbiroExtensionGUI.Enums;
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
    public class JobSchedulerService
    {
        #region Fields
        private IScheduler scheduler;
        private readonly TimeWork timeWork;
        private Queue<(User, ClockType)> Users = new Queue<(User, ClockType)>();
        private readonly HanbiroChromiumBrowser chromiumBrowser;
        private List<User> allUsers;
        private ClockType clockType = ClockType.None;
        public event EventHandler<string> OnLogMessage;
        public event EventHandler<(bool, ClockType)> OnClockingStateChanged;
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
        public async Task UnScheduler(ClockType clockType)
        {
            if (scheduler != null)
            {
                if (clockType == ClockType.In)
                {
                    scheduler.UnscheduleJobs(new List<TriggerKey>() { new TriggerKey("trigger2", "group1") });
                }
                else if (clockType == ClockType.Out)
                {
                    scheduler.UnscheduleJobs(new List<TriggerKey>() { new TriggerKey("trigger1", "group1") });
                }
            }
        }
        public async Task InitSchedulerAsync(ClockType clockType)
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
                //.WithSimpleSchedule(x => x
                //   .WithIntervalInSeconds(1)
                //   .WithRepeatCount(0))
                .UsingJobData(nameof(ClockType), nameof(ClockType.In))
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartNow()
                .WithCronSchedule(cronExpressionEndTime)
                //.WithSimpleSchedule(x => x
                // .WithIntervalInSeconds(30)
                //    .WithRepeatCount(5))
                .UsingJobData(nameof(ClockType), nameof(ClockType.Out))
                .Build();

            var triggerSet = new HashSet<ITrigger>();

            if (clockType == ClockType.None)
            {
                triggerSet.Add(trigger);
                triggerSet.Add(trigger2);
            }
            else if (clockType == ClockType.In)
            {
                triggerSet.Add(trigger2);
            }
            else if (clockType == ClockType.Out)
            {
                triggerSet.Add(trigger);
            }
            
            await scheduler.ScheduleJob(job, triggerSet, true);
        }


        private string GetExpressionForDayMonthYear()
        {
            return "? * " + string.Join(",", timeWork.DaysOfWeek.Where(d => d.Value == true)
                .Select(d => d.Key.ToString().Substring(0, 3).ToUpper()));
        }

        private string GetExpressionForStartTime()
        {
            //return $"0 {timeWork.StartTime.Minute} {timeWork.StartTime.Hour} {GetExpressionForDayMonthYear()}";
            Random random = new Random();
            int minute = random.Next(timeWork.StartTime.Minute, timeWork.StartTime.Minute + 10);
            return $"0 {minute} {timeWork.StartTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        private string GetExpressionForEndTime()
        {
            Random random = new Random();
            int minute = random.Next(timeWork.EndTime.Minute, timeWork.EndTime.Minute + 5);
            return $"0 {minute} {timeWork.EndTime.Hour} {GetExpressionForDayMonthYear()}";
        }

        public void Reset(ClockType clockType)
        {
            this.clockType = clockType;

            lock (allUsers)
            {
                Console.WriteLine("===========================");
                OnClockingStateChanged?.Invoke(this, (true, clockType));
                foreach (var user in allUsers.Where(u => u.IsActive/* && u.LoginDate.Date < DateTime.Now.Date*/))
                {
                    Users.Enqueue((user, clockType));
                }

                OnLogMessage?.Invoke(this, $"===========================\nCount Queue : {Users.Count}");
                Console.WriteLine($"Count Queue : {Users.Count}");
            }

            ClockInOut();
        }
        public void ClockInOut()
        {
            if (Users.Count == 0)
            {
                OnClockingStateChanged?.Invoke(this, (false, this.clockType));
                OnLogMessage?.Invoke(this, $"Finished");
                return;
            }

            var item = Users.Dequeue();
            var user = item.Item1;
            var clockType = item.Item2;
            this.clockType = clockType;
            Console.WriteLine("====");
            Console.WriteLine(DateTime.Now.ToString() + $"-Start-{clockType}-{user.UserName}");
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
