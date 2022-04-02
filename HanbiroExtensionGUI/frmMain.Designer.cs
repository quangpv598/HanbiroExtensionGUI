
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.chkReciveEmailNotifications = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxTimeManagement = new System.Windows.Forms.GroupBox();
            this.chkFri = new System.Windows.Forms.CheckBox();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.chkSun = new System.Windows.Forms.CheckBox();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.chkSat = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkThu = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkWed = new System.Windows.Forms.CheckBox();
            this.chkTue = new System.Windows.Forms.CheckBox();
            this.chkMon = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxTimeManagement.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(11, 38);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(389, 23);
            this.txtUsername.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(11, 94);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(389, 23);
            this.txtPassword.TabIndex = 2;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(11, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 16;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(325, 8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 17;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_ClickAsync);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveSettings.Location = new System.Drawing.Point(155, 8);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(96, 23);
            this.btnSaveSettings.TabIndex = 18;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_ClickAsync);
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(181, 123);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(219, 23);
            this.txtEmail.TabIndex = 34;
            this.txtEmail.Text = "abc@gmail.com";
            this.txtEmail.Visible = false;
            // 
            // chkReciveEmailNotifications
            // 
            this.chkReciveEmailNotifications.AutoSize = true;
            this.chkReciveEmailNotifications.Location = new System.Drawing.Point(12, 128);
            this.chkReciveEmailNotifications.Name = "chkReciveEmailNotifications";
            this.chkReciveEmailNotifications.Size = new System.Drawing.Size(169, 19);
            this.chkReciveEmailNotifications.TabIndex = 33;
            this.chkReciveEmailNotifications.Text = "Recieve Email Notifications";
            this.chkReciveEmailNotifications.UseVisualStyleBackColor = true;
            this.chkReciveEmailNotifications.CheckedChanged += new System.EventHandler(this.chkReciveEmailNotifications_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 25;
            this.label5.Text = "Days of week";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSaveSettings);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 328);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(413, 40);
            this.panel1.TabIndex = 22;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtEmail);
            this.groupBox1.Controls.Add(this.chkReciveEmailNotifications);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 183);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quản lý người dùng";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(11, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 23);
            this.button1.TabIndex = 35;
            this.button1.Text = "Thêm mới người dùng";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBoxTimeManagement
            // 
            this.groupBoxTimeManagement.Controls.Add(this.chkFri);
            this.groupBoxTimeManagement.Controls.Add(this.dtpEndTime);
            this.groupBoxTimeManagement.Controls.Add(this.chkSun);
            this.groupBoxTimeManagement.Controls.Add(this.dtpStartTime);
            this.groupBoxTimeManagement.Controls.Add(this.chkSat);
            this.groupBoxTimeManagement.Controls.Add(this.label3);
            this.groupBoxTimeManagement.Controls.Add(this.chkThu);
            this.groupBoxTimeManagement.Controls.Add(this.label4);
            this.groupBoxTimeManagement.Controls.Add(this.chkWed);
            this.groupBoxTimeManagement.Controls.Add(this.label5);
            this.groupBoxTimeManagement.Controls.Add(this.chkTue);
            this.groupBoxTimeManagement.Controls.Add(this.chkMon);
            this.groupBoxTimeManagement.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxTimeManagement.Location = new System.Drawing.Point(0, 183);
            this.groupBoxTimeManagement.Name = "groupBoxTimeManagement";
            this.groupBoxTimeManagement.Size = new System.Drawing.Size(413, 133);
            this.groupBoxTimeManagement.TabIndex = 24;
            this.groupBoxTimeManagement.TabStop = false;
            this.groupBoxTimeManagement.Text = "Quản lý thời gian";
            // 
            // chkFri
            // 
            this.chkFri.AutoSize = true;
            this.chkFri.Checked = true;
            this.chkFri.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFri.Location = new System.Drawing.Point(244, 98);
            this.chkFri.Name = "chkFri";
            this.chkFri.Size = new System.Drawing.Size(39, 19);
            this.chkFri.TabIndex = 30;
            this.chkFri.Text = "Fri";
            this.chkFri.UseVisualStyleBackColor = true;
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpEndTime.CustomFormat = "HH:mm";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(306, 44);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Size = new System.Drawing.Size(94, 23);
            this.dtpEndTime.TabIndex = 23;
            // 
            // chkSun
            // 
            this.chkSun.AutoSize = true;
            this.chkSun.Location = new System.Drawing.Point(356, 98);
            this.chkSun.Name = "chkSun";
            this.chkSun.Size = new System.Drawing.Size(46, 19);
            this.chkSun.TabIndex = 32;
            this.chkSun.Text = "Sun";
            this.chkSun.UseVisualStyleBackColor = true;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "HH:mm";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(11, 44);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(94, 23);
            this.dtpStartTime.TabIndex = 21;
            // 
            // chkSat
            // 
            this.chkSat.AutoSize = true;
            this.chkSat.Location = new System.Drawing.Point(299, 98);
            this.chkSat.Name = "chkSat";
            this.chkSat.Size = new System.Drawing.Size(42, 19);
            this.chkSat.TabIndex = 31;
            this.chkSat.Text = "Sat";
            this.chkSat.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "Clock In";
            // 
            // chkThu
            // 
            this.chkThu.AutoSize = true;
            this.chkThu.Checked = true;
            this.chkThu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkThu.Location = new System.Drawing.Point(187, 98);
            this.chkThu.Name = "chkThu";
            this.chkThu.Size = new System.Drawing.Size(46, 19);
            this.chkThu.TabIndex = 29;
            this.chkThu.Text = "Thu";
            this.chkThu.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(306, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 24;
            this.label4.Text = "Clock Out";
            // 
            // chkWed
            // 
            this.chkWed.AutoSize = true;
            this.chkWed.Checked = true;
            this.chkWed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWed.Location = new System.Drawing.Point(130, 98);
            this.chkWed.Name = "chkWed";
            this.chkWed.Size = new System.Drawing.Size(50, 19);
            this.chkWed.TabIndex = 28;
            this.chkWed.Text = "Wed";
            this.chkWed.UseVisualStyleBackColor = true;
            // 
            // chkTue
            // 
            this.chkTue.AutoSize = true;
            this.chkTue.Checked = true;
            this.chkTue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTue.Location = new System.Drawing.Point(72, 98);
            this.chkTue.Name = "chkTue";
            this.chkTue.Size = new System.Drawing.Size(45, 19);
            this.chkTue.TabIndex = 27;
            this.chkTue.Text = "Tue";
            this.chkTue.UseVisualStyleBackColor = true;
            // 
            // chkMon
            // 
            this.chkMon.AutoSize = true;
            this.chkMon.Checked = true;
            this.chkMon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMon.Location = new System.Drawing.Point(15, 98);
            this.chkMon.Name = "chkMon";
            this.chkMon.Size = new System.Drawing.Size(51, 19);
            this.chkMon.TabIndex = 26;
            this.chkMon.Text = "Mon";
            this.chkMon.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 368);
            this.Controls.Add(this.groupBoxTimeManagement);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(429, 335);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HanbiroExtensionGUI";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxTimeManagement.ResumeLayout(false);
            this.groupBoxTimeManagement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.CheckBox chkReciveEmailNotifications;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxTimeManagement;
        private System.Windows.Forms.CheckBox chkFri;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.CheckBox chkSun;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.CheckBox chkSat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkThu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkWed;
        private System.Windows.Forms.CheckBox chkTue;
        private System.Windows.Forms.CheckBox chkMon;
        private System.Windows.Forms.Button button1;
    }
}

