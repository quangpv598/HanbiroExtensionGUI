using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Models
{
    public class AppSettings
    {
        public string TelegramToken { get; set; }
        public string BaseUrl { get; set; }
        public TimeWork TimeWork { get; set; } = new TimeWork();
        public List<User> Users { get; set; } = new List<User>();
    }
    public class TimeWork
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Dictionary<DayOfWeek, bool> DaysOfWeek { get; set; } = new Dictionary<DayOfWeek, bool>();
    }

    public class User
    {
        public long TelegramId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsLoginSuccess { get; set; }
        public bool IsSendResultToEmail { get; set; }
        public string Email { get; set; }
        public bool IsCheckHealthUser { get; set; }
        public string LastCommand { get; set; }
        public bool IsActive { get; set; }
        public string Cookie { get; set; }
    }

}
