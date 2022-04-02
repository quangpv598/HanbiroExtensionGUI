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
        private readonly CurrentUserSettings currentUserSettings = null;
        #endregion

        #region Properties
        public CurrentUserSettings CurrentUserSettings => currentUserSettings;
        #endregion

        #region Constructors
        public ServicesBase(CurrentUserSettings currentUserSettings)
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
