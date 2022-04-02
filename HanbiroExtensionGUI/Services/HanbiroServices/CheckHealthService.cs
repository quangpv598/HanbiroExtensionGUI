using HanbiroExtensionGUI.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public class CheckHealthService : HanbiroServiceBase, IJob
    {
        #region Fields 
        public new event EventHandler OnSuccess;
        public new event EventHandler OnError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public CheckHealthService(CurrentUserSettings currentUserSettings) : base(currentUserSettings)
        {
            base.Browser.IsCheckHealth = true;

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

        private async void DoWorkAsync()
        {
            if (CurrentUserSettings == null) return;

            var checkHealthUser = CurrentUserSettings.Users.FirstOrDefault(u => u.IsCheckHealthUser);
            if (checkHealthUser != null)
            {
                Browser.DoWork(checkHealthUser);
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Raise on :  " + DateTime.Now.ToString());
            DoWorkAsync();
            return null;
        }

        #endregion
    }
}
