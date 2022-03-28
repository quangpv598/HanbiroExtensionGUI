using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionGUI
{
    public partial class frmMain : Form
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void chkReciveEmailNotifications_CheckedChanged(object sender, EventArgs e)
        {
            txtEmail.Visible = chkReciveEmailNotifications.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Methods

        #endregion
    }
}
