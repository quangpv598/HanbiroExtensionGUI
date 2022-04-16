using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionConsole.Constants
{
    public static class ApiResources
    {
        public static readonly string LoginSignal = "/ngw/app/lib/css/input-password.css";
        public static readonly string Auth = "/ngw/sign/auth";
        public static readonly string AuthSuccessSignal = "/ngw/app/template/main/dashboard.html";
        public static readonly string ClockIn = "/ngw/timecard/punch_v2/in";
        public static readonly string ClockOut = "/ngw/timecard/punch_v2/out";
        public static readonly string ClockInOutPostPayload = "{\"mimeType\": \"application / x - www - form - urlencoded; charset = UTF - 8\",\"text\": \"check_type = 1\",\"params\": [{\"name\": \"check_type\",\"value\": \"1\"}]}";
    }
}
