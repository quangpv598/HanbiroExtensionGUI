using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services.JobSchedulerServices
{
    public class HanbiroJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            CheckInCheckOutService worker = (CheckInCheckOutService)dataMap["CheckInCheckOutService"];
            worker.DoWorkAsync();

            return null;
        }
    }
}
