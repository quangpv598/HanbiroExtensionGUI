using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Models
{
    public class AppSettings
    {
        public string TelegramToken { get; set; }
        public string BaseUrl { get; set; }
        public bool IsClockingIn
        {
            get
            {
                return DateTime.Now.TimeOfDay <= TimeWork.StartTime.TimeOfDay.Add(TimeSpan.FromMinutes(15))
                    && DateTime.Now.TimeOfDay >= TimeWork.StartTime.TimeOfDay.Add(TimeSpan.FromMinutes(-5));
            }
        }
        public bool IsClockingOut
        {
            get
            {
                return DateTime.Now.TimeOfDay <= TimeWork.EndTime.TimeOfDay.Add(TimeSpan.FromMinutes(15))
                    && DateTime.Now.TimeOfDay >= TimeWork.StartTime.TimeOfDay.Add(TimeSpan.FromMinutes(-5));
            }
        }
        public List<long> Adminitrators { get; set; }
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
        public string FullName { get; set; }
        public bool IsLoginSuccess { get; set; }
        public bool IsGettingCookie { get; set; }
        public bool IsSendResultToEmail { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCheckHealthUser { get; set; }
        public string LastCommand { get; set; }
        public bool IsActive { get; set; }
        public string Cookie { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime ClockOutTime { get; set; }
        public DateTime LoginDate { get; set; } = DateTime.Now;
    }

}
