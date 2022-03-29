using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public class CheckInCheckOutJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Test");
            return null;
        }
    }
}
