using HanbiroExtensionGUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Models
{
    public class ErrorArgs
    {
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public ErrorArgs(ErrorType ErrorType, string ErrorMessage)
        {
            this.ErrorType = ErrorType;
            this.ErrorMessage = ErrorMessage;
        }
    }
}
