using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Controls.ChromiumBrowser.EventsArgs
{
    public class HanbiroArgs : EventArgs
    {
        public User User { get; set; }
        public string Message { get; set; }
        public ErrorType Type { get; set; }
        public ClockType ClockType { get; set; }
        public ActionStatus ActionStatus { get; set; }
        public HanbiroArgs(User user, string message, ErrorType type, ClockType clockType, ActionStatus actionStatus)
        {
            User = user;
            Message = message;
            Type = type;
            ClockType = clockType;
            ActionStatus = actionStatus;
        }
    }
}
