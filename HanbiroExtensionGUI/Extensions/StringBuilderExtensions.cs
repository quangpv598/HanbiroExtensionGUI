using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendLineWithShortTime(this StringBuilder stringBuilder,string functionName, bool status,string content)
        {
            string stringStatus = status ? "PASS" : "FAIL";
            stringBuilder.AppendLine($"[{DateTime.Now}][{functionName}][{stringStatus}]-{content}");
        }
    }
}
