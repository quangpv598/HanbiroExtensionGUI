using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI.Services.Scheduler
{
    public class AutoRestartJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Application.Restart();
            return null;
        }
    }
}
