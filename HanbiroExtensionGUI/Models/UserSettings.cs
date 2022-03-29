using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Models
{
    public class UserSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSendResultToEmail { get; set; }
        public string Email { get; set; }
        public Dictionary<DayOfWeek, bool> DaysOfWeek { get; set; } = new Dictionary<DayOfWeek, bool>();
    }
}
