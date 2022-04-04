using HanbiroExtensionGUI.Controls;
using HanbiroExtensionGUI.Controls.ChromiumBrowser.Utils;
using HanbiroExtensionGUI.Enums;
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
    public class CheckInCheckOutService : HanbiroServiceBase, IJob
    {
        #region Fields 
        public new event EventHandler OnSuccess;
        public new event EventHandler OnError;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public CheckInCheckOutService(UserSettings currentUserSettings) : base(currentUserSettings)
        {
            Browser.IsCheckHealth = true;

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

            foreach (var user in CurrentUserSettings.Users)
            {
                Browser.DoWork(user);

                try
                {
                    await TaskWaiter.WaitUntil(() => Task.FromResult(!Browser.IsBusy), 1000, 300000);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new BrowserEventArgs(Browser,
                        new ErrorArgs(ErrorType.TimeOut, "Fail To Complete Check In Check Out!!!")));
                }
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Raise on :  " + DateTime.Now.ToString());
            //DoWorkAsync();
            return null;
        }

        #endregion
    }
}
