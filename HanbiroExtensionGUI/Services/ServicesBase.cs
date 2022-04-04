using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public class ServicesBase
    {
        #region Fields
        private readonly UserSettings currentUserSettings = null;
        #endregion

        #region Properties
        public UserSettings CurrentUserSettings { get; set; }
        #endregion

        #region Constructors
        public ServicesBase(UserSettings currentUserSettings)
        {
            this.currentUserSettings = currentUserSettings;
        }
        #endregion

        #region Events

        #endregion

        #region Methods

        #endregion
    }
}
