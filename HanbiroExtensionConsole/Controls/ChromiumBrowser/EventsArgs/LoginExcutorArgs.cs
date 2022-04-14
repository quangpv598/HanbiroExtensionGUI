using HanbiroExtensionConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Controls.ChromiumBrowser.EventsArgs
{
    public class LoginExcutorArgs : EventArgs
    {
        public User User { get; set; } 
        public LoginExcutorArgs(User user)
        {
            this.User = user;
        }
    }
}
