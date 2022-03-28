
namespace HanbiroExtensionGUI
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.chkMon = new System.Windows.Forms.CheckBox();
            this.chkTue = new System.Windows.Forms.CheckBox();
            this.chkThu = new System.Windows.Forms.CheckBox();
            this.chkWed = new System.Windows.Forms.CheckBox();
            this.chkSun = new System.Windows.Forms.CheckBox();
            this.chkSat = new System.Windows.Forms.CheckBox();
            this.chkFri = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.chkReciveEmailNotifications = new System.Windows.Forms.CheckBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(12, 33);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(389, 23);
            this.txtUsername.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(12, 89);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(389, 23);
            this.txtPassword.TabIndex = 2;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "HH:mm";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(12, 144);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(94, 23);
            this.dtpStartTime.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Start time";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(307, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "End time";
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpEndTime.CustomFormat = "HH:mm";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(307, 144);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Size = new System.Drawing.Size(94, 23);
            this.dtpEndTime.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Days of week";
            // 
            // chkMon
            // 
            this.chkMon.AutoSize = true;
            this.chkMon.Checked = true;
            this.chkMon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMon.Location = new System.Drawing.Point(13, 201);
            this.chkMon.Name = "chkMon";
            this.chkMon.Size = new System.Drawing.Size(51, 19);
            this.chkMon.TabIndex = 9;
            this.chkMon.Text = "Mon";
            this.chkMon.UseVisualStyleBackColor = true;
            // 
            // chkTue
            // 
            this.chkTue.AutoSize = true;
            this.chkTue.Checked = true;
            this.chkTue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTue.Location = new System.Drawing.Point(70, 201);
            this.chkTue.Name = "chkTue";
            this.chkTue.Size = new System.Drawing.Size(45, 19);
            this.chkTue.TabIndex = 10;
            this.chkTue.Text = "Tue";
            this.chkTue.UseVisualStyleBackColor = true;
            // 
            // chkThu
            // 
            this.chkThu.AutoSize = true;
            this.chkThu.Checked = true;
            this.chkThu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkThu.Location = new System.Drawing.Point(185, 201);
            this.chkThu.Name = "chkThu";
            this.chkThu.Size = new System.Drawing.Size(46, 19);
            this.chkThu.TabIndex = 12;
            this.chkThu.Text = "Thu";
            this.chkThu.UseVisualStyleBackColor = true;
            // 
            // chkWed
            // 
            this.chkWed.AutoSize = true;
            this.chkWed.Checked = true;
            this.chkWed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWed.Location = new System.Drawing.Point(128, 201);
            this.chkWed.Name = "chkWed";
            this.chkWed.Size = new System.Drawing.Size(50, 19);
            this.chkWed.TabIndex = 11;
            this.chkWed.Text = "Wed";
            this.chkWed.UseVisualStyleBackColor = true;
            // 
            // chkSun
            // 
            this.chkSun.AutoSize = true;
            this.chkSun.Location = new System.Drawing.Point(354, 201);
            this.chkSun.Name = "chkSun";
            this.chkSun.Size = new System.Drawing.Size(46, 19);
            this.chkSun.TabIndex = 15;
            this.chkSun.Text = "Sun";
            this.chkSun.UseVisualStyleBackColor = true;
            // 
            // chkSat
            // 
            this.chkSat.AutoSize = true;
            this.chkSat.Location = new System.Drawing.Point(297, 201);
            this.chkSat.Name = "chkSat";
            this.chkSat.Size = new System.Drawing.Size(42, 19);
            this.chkSat.TabIndex = 14;
            this.chkSat.Text = "Sat";
            this.chkSat.UseVisualStyleBackColor = true;
            // 
            // chkFri
            // 
            this.chkFri.AutoSize = true;
            this.chkFri.Checked = true;
            this.chkFri.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFri.Location = new System.Drawing.Point(242, 201);
            this.chkFri.Name = "chkFri";
            this.chkFri.Size = new System.Drawing.Size(39, 19);
            this.chkFri.TabIndex = 13;
            this.chkFri.Text = "Fri";
            this.chkFri.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 261);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 16;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(326, 261);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 17;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(156, 261);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(96, 23);
            this.btnSaveSettings.TabIndex = 18;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // chkReciveEmailNotifications
            // 
            this.chkReciveEmailNotifications.AutoSize = true;
            this.chkReciveEmailNotifications.Location = new System.Drawing.Point(12, 231);
            this.chkReciveEmailNotifications.Name = "chkReciveEmailNotifications";
            this.chkReciveEmailNotifications.Size = new System.Drawing.Size(169, 19);
            this.chkReciveEmailNotifications.TabIndex = 19;
            this.chkReciveEmailNotifications.Text = "Recieve Email Notifications";
            this.chkReciveEmailNotifications.UseVisualStyleBackColor = true;
            this.chkReciveEmailNotifications.CheckedChanged += new System.EventHandler(this.chkReciveEmailNotifications_CheckedChanged);
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(181, 226);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(219, 23);
            this.txtEmail.TabIndex = 20;
            this.txtEmail.Text = "abc@gmail.com";
            this.txtEmail.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 296);
            this.ControlBox = false;
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.chkReciveEmailNotifications);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.chkFri);
            this.Controls.Add(this.chkSun);
            this.Controls.Add(this.chkSat);
            this.Controls.Add(this.chkThu);
            this.Controls.Add(this.chkWed);
            this.Controls.Add(this.chkTue);
            this.Controls.Add(this.chkMon);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpEndTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUsername);
            this.MaximumSize = new System.Drawing.Size(429, 335);
            this.MinimumSize = new System.Drawing.Size(429, 335);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HanbiroExtensionGUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkMon;
        private System.Windows.Forms.CheckBox chkTue;
        private System.Windows.Forms.CheckBox chkThu;
        private System.Windows.Forms.CheckBox chkWed;
        private System.Windows.Forms.CheckBox chkSun;
        private System.Windows.Forms.CheckBox chkSat;
        private System.Windows.Forms.CheckBox chkFri;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.CheckBox chkReciveEmailNotifications;
        private System.Windows.Forms.TextBox txtEmail;
    }
}

