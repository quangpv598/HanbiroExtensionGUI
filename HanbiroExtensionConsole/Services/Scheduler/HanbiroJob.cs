using HanbiroExtensionConsole.Enums;
using HanbiroExtensionConsole.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Services.JobSchedulerServices
{
    public class HanbiroJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            ClockType clockType = (ClockType)Enum.Parse(typeof(ClockType), 
                context.Trigger.JobDataMap[nameof(ClockType)].ToString(), 
                true);
            JobSchedulerService jobSchedulerService = (JobSchedulerService)dataMap[nameof(JobSchedulerService)];
            jobSchedulerService.Reset(clockType);
            return null;
        }
    }
}
