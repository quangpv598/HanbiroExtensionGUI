using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Enums
{
    public enum ErrorType
    {
        WrongUsernameOrPassword,
        FailToLogin,
        FailToClockIn,
        FailToClockOut,
        CallApi,
        TimeOut,
        None
    }
}
