using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
using HanbiroExtensionGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public class CheckingPasswordService : HanbiroServiceBase
    {
        #region Fields 
        public new event EventHandler OnSuccess;
        public new event EventHandler OnError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public CheckingPasswordService(CurrentUserSettings currentUserSettings) : base(currentUserSettings)
        {
            base.Browser.IsCheckAccountValid = true;

            base.OnSuccess += CheckInCheckOutService_OnSuccess;
            base.OnError += CheckInCheckOutService_OnError;
        }

        #endregion

        #region Events
        private void CheckInCheckOutService_OnError(object sender, EventArgs e)
        {
            // send error to admin
            this.OnError?.Invoke(sender, e);
        }

        private void CheckInCheckOutService_OnSuccess(object sender, EventArgs e)
        {
            //send success noti to user
            this.OnSuccess?.Invoke(sender, e);
        }
        #endregion

        #region Methods

        public async Task<bool> DoWorkAsync(User user)
        {
            if (CurrentUserSettings == null) return false;
            Browser.DoWork(user);

            try
            {
                await TaskWaiter.WaitUntil(() => Task.FromResult(!Browser.IsBusy), 1000, 300000);
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new BrowserEventArgs(Browser,
                    new ErrorArgs(ErrorType.WrongUsernameOrPassword, "Wrong User Name or Password!!!")));
                return false;
            }
        }
        #endregion
    }
}