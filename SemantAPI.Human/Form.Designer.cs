using System;
using System.Collections.Generic;

using SemantAPI.Common;

namespace SemantAPI.Human
{
	partial class SemantAPIHuman
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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

		#region Controls definition

		private System.Windows.Forms.GroupBox gbSettings;
		private System.Windows.Forms.Button btnSource;
		private System.Windows.Forms.TextBox tbSource;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btProcess;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.TextBox tbMechanicalTurkKey;
		private System.Windows.Forms.ProgressBar pbMechanicalTurk;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox tbMechanicalTurkSecret;
		private System.Windows.Forms.LinkLabel llMechanicalTurk;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox gbMechanicalTurk;
		private System.Windows.Forms.TextBox tbMechanicalTurkEmail;
		private System.Windows.Forms.CheckBox cbMechanicalTurkNotification;
		private System.Windows.Forms.MaskedTextBox tbMechanicalTurkReward;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox cbMechanicalTurkAssignments;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox tbMechanicalTurkApprove;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox tbMechanicalTurkTime;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.DateTimePicker dtMechanicalTurk;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.ComboBox cbMechanicalTurkLocale;
		private System.Windows.Forms.TextBox tbMechanicalTurkPercent;
		private System.Windows.Forms.ComboBox cbLanguage;
		private System.Windows.Forms.Label label1;

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SemantAPIHuman));
			this.gbSettings = new System.Windows.Forms.GroupBox();
			this.btnSource = new System.Windows.Forms.Button();
			this.tbSource = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbLanguage = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkKey = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkSecret = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btProcess = new System.Windows.Forms.Button();
			this.btClose = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pbMechanicalTurk = new System.Windows.Forms.ProgressBar();
			this.llMechanicalTurk = new System.Windows.Forms.LinkLabel();
			this.gbMechanicalTurk = new System.Windows.Forms.GroupBox();
			this.tbMechanicalTurkPercent = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.cbMechanicalTurkLocale = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.dtMechanicalTurk = new System.Windows.Forms.DateTimePicker();
			this.label16 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkApprove = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkTime = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkReward = new System.Windows.Forms.MaskedTextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.cbMechanicalTurkAssignments = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tbMechanicalTurkEmail = new System.Windows.Forms.TextBox();
			this.cbMechanicalTurkNotification = new System.Windows.Forms.CheckBox();
			this.btAbout = new System.Windows.Forms.Button();
			this.gbSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.gbMechanicalTurk.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbSettings
			// 
			this.gbSettings.Controls.Add(this.btnSource);
			this.gbSettings.Controls.Add(this.tbSource);
			this.gbSettings.Controls.Add(this.label2);
			this.gbSettings.Controls.Add(this.cbLanguage);
			this.gbSettings.Controls.Add(this.label1);
			this.gbSettings.Controls.Add(this.tbMechanicalTurkKey);
			this.gbSettings.Controls.Add(this.label10);
			this.gbSettings.Controls.Add(this.tbMechanicalTurkSecret);
			this.gbSettings.Controls.Add(this.label8);
			this.gbSettings.Location = new System.Drawing.Point(14, 90);
			this.gbSettings.Name = "gbSettings";
			this.gbSettings.Size = new System.Drawing.Size(501, 77);
			this.gbSettings.TabIndex = 1;
			this.gbSettings.TabStop = false;
			this.gbSettings.Text = "Settings";
			// 
			// btnSource
			// 
			this.btnSource.Location = new System.Drawing.Point(416, 16);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(75, 23);
			this.btnSource.TabIndex = 10;
			this.btnSource.Text = "Browse";
			this.btnSource.UseVisualStyleBackColor = true;
			this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
			// 
			// tbSource
			// 
			this.tbSource.Enabled = false;
			this.tbSource.Location = new System.Drawing.Point(277, 18);
			this.tbSource.Name = "tbSource";
			this.tbSource.Size = new System.Drawing.Size(133, 20);
			this.tbSource.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(214, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Source file";
			// 
			// cbLanguage
			// 
			this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLanguage.FormattingEnabled = true;
			this.cbLanguage.Items.AddRange(new object[] {
            "English",
            "French",
            "Spanish",
            "Portuguese",
            "German"});
			this.cbLanguage.Location = new System.Drawing.Point(68, 17);
			this.cbLanguage.Name = "cbLanguage";
			this.cbLanguage.Size = new System.Drawing.Size(121, 21);
			this.cbLanguage.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Language";
			// 
			// tbMechanicalTurkKey
			// 
			this.tbMechanicalTurkKey.Location = new System.Drawing.Point(68, 44);
			this.tbMechanicalTurkKey.Name = "tbMechanicalTurkKey";
			this.tbMechanicalTurkKey.Size = new System.Drawing.Size(121, 20);
			this.tbMechanicalTurkKey.TabIndex = 10;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(37, 47);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(25, 13);
			this.label10.TabIndex = 9;
			this.label10.Text = "Key";
			// 
			// tbMechanicalTurkSecret
			// 
			this.tbMechanicalTurkSecret.Location = new System.Drawing.Point(277, 44);
			this.tbMechanicalTurkSecret.Name = "tbMechanicalTurkSecret";
			this.tbMechanicalTurkSecret.Size = new System.Drawing.Size(133, 20);
			this.tbMechanicalTurkSecret.TabIndex = 11;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(233, 47);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 13);
			this.label8.TabIndex = 8;
			this.label8.Text = "Secret";
			// 
			// btProcess
			// 
			this.btProcess.Location = new System.Drawing.Point(349, 450);
			this.btProcess.Name = "btProcess";
			this.btProcess.Size = new System.Drawing.Size(75, 23);
			this.btProcess.TabIndex = 14;
			this.btProcess.Text = "Process";
			this.btProcess.UseVisualStyleBackColor = true;
			this.btProcess.Click += new System.EventHandler(this.btProcess_Click);
			// 
			// btClose
			// 
			this.btClose.Location = new System.Drawing.Point(430, 450);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 15;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.Image = global::SemantAPI.Human.Properties.Resources.main_logo;
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(14, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(500, 70);
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			// 
			// pbMechanicalTurk
			// 
			this.pbMechanicalTurk.Location = new System.Drawing.Point(10, 230);
			this.pbMechanicalTurk.Name = "pbMechanicalTurk";
			this.pbMechanicalTurk.Size = new System.Drawing.Size(481, 23);
			this.pbMechanicalTurk.TabIndex = 0;
			// 
			// llMechanicalTurk
			// 
			this.llMechanicalTurk.AutoSize = true;
			this.llMechanicalTurk.Location = new System.Drawing.Point(95, 0);
			this.llMechanicalTurk.Name = "llMechanicalTurk";
			this.llMechanicalTurk.Size = new System.Drawing.Size(83, 13);
			this.llMechanicalTurk.TabIndex = 0;
			this.llMechanicalTurk.TabStop = true;
			this.llMechanicalTurk.Text = "www.mturk.com";
			this.llMechanicalTurk.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llMechanicalTurk_LinkClicked);
			// 
			// gbMechanicalTurk
			// 
			this.gbMechanicalTurk.Controls.Add(this.tbMechanicalTurkPercent);
			this.gbMechanicalTurk.Controls.Add(this.label18);
			this.gbMechanicalTurk.Controls.Add(this.cbMechanicalTurkLocale);
			this.gbMechanicalTurk.Controls.Add(this.label17);
			this.gbMechanicalTurk.Controls.Add(this.dtMechanicalTurk);
			this.gbMechanicalTurk.Controls.Add(this.label16);
			this.gbMechanicalTurk.Controls.Add(this.tbMechanicalTurkApprove);
			this.gbMechanicalTurk.Controls.Add(this.label15);
			this.gbMechanicalTurk.Controls.Add(this.tbMechanicalTurkTime);
			this.gbMechanicalTurk.Controls.Add(this.label14);
			this.gbMechanicalTurk.Controls.Add(this.tbMechanicalTurkReward);
			this.gbMechanicalTurk.Controls.Add(this.label13);
			this.gbMechanicalTurk.Controls.Add(this.cbMechanicalTurkAssignments);
			this.gbMechanicalTurk.Controls.Add(this.label12);
			this.gbMechanicalTurk.Controls.Add(this.tbMechanicalTurkEmail);
			this.gbMechanicalTurk.Controls.Add(this.cbMechanicalTurkNotification);
			this.gbMechanicalTurk.Controls.Add(this.llMechanicalTurk);
			this.gbMechanicalTurk.Controls.Add(this.pbMechanicalTurk);
			this.gbMechanicalTurk.Location = new System.Drawing.Point(14, 173);
			this.gbMechanicalTurk.Name = "gbMechanicalTurk";
			this.gbMechanicalTurk.Size = new System.Drawing.Size(501, 264);
			this.gbMechanicalTurk.TabIndex = 14;
			this.gbMechanicalTurk.TabStop = false;
			this.gbMechanicalTurk.Text = "Mechanical turk (  www.mturk.com  )";
			// 
			// tbMechanicalTurkPercent
			// 
			this.tbMechanicalTurkPercent.Location = new System.Drawing.Point(239, 204);
			this.tbMechanicalTurkPercent.Name = "tbMechanicalTurkPercent";
			this.tbMechanicalTurkPercent.Size = new System.Drawing.Size(253, 20);
			this.tbMechanicalTurkPercent.TabIndex = 29;
			this.tbMechanicalTurkPercent.Text = "75";
			this.tbMechanicalTurkPercent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMechanicalTurkPercent_KeyPress);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(9, 207);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(165, 13);
			this.label18.TabIndex = 28;
			this.label18.Text = "Percent of approved assignments";
			// 
			// cbMechanicalTurkLocale
			// 
			this.cbMechanicalTurkLocale.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.cbMechanicalTurkLocale.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbMechanicalTurkLocale.FormattingEnabled = true;
			this.cbMechanicalTurkLocale.Location = new System.Drawing.Point(239, 175);
			this.cbMechanicalTurkLocale.Name = "cbMechanicalTurkLocale";
			this.cbMechanicalTurkLocale.Size = new System.Drawing.Size(253, 21);
			this.cbMechanicalTurkLocale.TabIndex = 27;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(9, 178);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(183, 13);
			this.label17.TabIndex = 26;
			this.label17.Text = "Target countries to work on the tasks";
			// 
			// dtMechanicalTurk
			// 
			this.dtMechanicalTurk.CustomFormat = "MM dd yyyy hh mm ss";
			this.dtMechanicalTurk.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtMechanicalTurk.Location = new System.Drawing.Point(239, 148);
			this.dtMechanicalTurk.Name = "dtMechanicalTurk";
			this.dtMechanicalTurk.Size = new System.Drawing.Size(253, 20);
			this.dtMechanicalTurk.TabIndex = 24;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(9, 151);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(140, 13);
			this.label16.TabIndex = 23;
			this.label16.Text = "Task expiration dat and time";
			// 
			// tbMechanicalTurkApprove
			// 
			this.tbMechanicalTurkApprove.Location = new System.Drawing.Point(239, 121);
			this.tbMechanicalTurkApprove.Name = "tbMechanicalTurkApprove";
			this.tbMechanicalTurkApprove.Size = new System.Drawing.Size(253, 20);
			this.tbMechanicalTurkApprove.TabIndex = 22;
			this.tbMechanicalTurkApprove.Text = "30";
			this.tbMechanicalTurkApprove.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericTextBox_KeyPress);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(9, 124);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(178, 13);
			this.label15.TabIndex = 21;
			this.label15.Text = "Tasks are auto-accepted after (min) ";
			// 
			// tbMechanicalTurkTime
			// 
			this.tbMechanicalTurkTime.Location = new System.Drawing.Point(239, 69);
			this.tbMechanicalTurkTime.Name = "tbMechanicalTurkTime";
			this.tbMechanicalTurkTime.Size = new System.Drawing.Size(253, 20);
			this.tbMechanicalTurkTime.TabIndex = 20;
			this.tbMechanicalTurkTime.Text = "5";
			this.tbMechanicalTurkTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericTextBox_KeyPress);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(9, 72);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(126, 13);
			this.label14.TabIndex = 19;
			this.label14.Text = "Task expiration time (min)";
			// 
			// tbMechanicalTurkReward
			// 
			this.tbMechanicalTurkReward.Location = new System.Drawing.Point(239, 95);
			this.tbMechanicalTurkReward.Mask = "0.00";
			this.tbMechanicalTurkReward.Name = "tbMechanicalTurkReward";
			this.tbMechanicalTurkReward.Size = new System.Drawing.Size(253, 20);
			this.tbMechanicalTurkReward.TabIndex = 18;
			this.tbMechanicalTurkReward.Text = "010";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(9, 98);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(196, 13);
			this.label13.TabIndex = 17;
			this.label13.Text = "Price per processing document one time";
			// 
			// cbMechanicalTurkAssignments
			// 
			this.cbMechanicalTurkAssignments.FormattingEnabled = true;
			this.cbMechanicalTurkAssignments.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
			this.cbMechanicalTurkAssignments.Location = new System.Drawing.Point(239, 42);
			this.cbMechanicalTurkAssignments.Name = "cbMechanicalTurkAssignments";
			this.cbMechanicalTurkAssignments.Size = new System.Drawing.Size(253, 21);
			this.cbMechanicalTurkAssignments.TabIndex = 16;
			this.cbMechanicalTurkAssignments.Text = "3";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(9, 45);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(192, 13);
			this.label12.TabIndex = 15;
			this.label12.Text = "# of times each document is processed";
			// 
			// tbMechanicalTurkEmail
			// 
			this.tbMechanicalTurkEmail.Enabled = false;
			this.tbMechanicalTurkEmail.Location = new System.Drawing.Point(239, 16);
			this.tbMechanicalTurkEmail.Name = "tbMechanicalTurkEmail";
			this.tbMechanicalTurkEmail.Size = new System.Drawing.Size(253, 20);
			this.tbMechanicalTurkEmail.TabIndex = 14;
			// 
			// cbMechanicalTurkNotification
			// 
			this.cbMechanicalTurkNotification.AutoSize = true;
			this.cbMechanicalTurkNotification.Location = new System.Drawing.Point(11, 19);
			this.cbMechanicalTurkNotification.Name = "cbMechanicalTurkNotification";
			this.cbMechanicalTurkNotification.Size = new System.Drawing.Size(211, 17);
			this.cbMechanicalTurkNotification.TabIndex = 13;
			this.cbMechanicalTurkNotification.Text = "Notify me by email when HITs are done";
			this.cbMechanicalTurkNotification.UseVisualStyleBackColor = true;
			this.cbMechanicalTurkNotification.CheckedChanged += new System.EventHandler(this.cbMechanicalTurkNotification_CheckedChanged);
			// 
			// btAbout
			// 
			this.btAbout.Location = new System.Drawing.Point(26, 450);
			this.btAbout.Name = "btAbout";
			this.btAbout.Size = new System.Drawing.Size(75, 23);
			this.btAbout.TabIndex = 16;
			this.btAbout.Text = "About";
			this.btAbout.UseVisualStyleBackColor = true;
			this.btAbout.Click += new System.EventHandler(this.btAbout_Click);
			// 
			// SemantAPIHuman
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(529, 488);
			this.Controls.Add(this.btAbout);
			this.Controls.Add(this.gbMechanicalTurk);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btProcess);
			this.Controls.Add(this.gbSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SemantAPIHuman";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SemantAPI";
			this.gbSettings.ResumeLayout(false);
			this.gbSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.gbMechanicalTurk.ResumeLayout(false);
			this.gbMechanicalTurk.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btAbout;
	}
}

