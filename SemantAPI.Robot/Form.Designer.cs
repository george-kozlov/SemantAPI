using System;
using System.Collections.Generic;

using SemantAPI.Common;

namespace SemantAPI.Robot
{
	partial class SemantAPIRobot
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

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbLanguage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbSource;
		private System.Windows.Forms.Button btnSource;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbOutput;
		private System.Windows.Forms.Button btnOutput;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbDocSize;
		private System.Windows.Forms.GroupBox gbSettings;
		private System.Windows.Forms.GroupBox gbBitext;
		private System.Windows.Forms.LinkLabel llBitext;
		private System.Windows.Forms.CheckBox cbBitext;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbBitextPassword;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ProgressBar pbBitext;
		private System.Windows.Forms.TextBox tbBitextLogin;
		private System.Windows.Forms.GroupBox gpSemantria;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ProgressBar pbSemantria;
		private System.Windows.Forms.TextBox tbSemantriaKey;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbSemantriaSecret;
		private System.Windows.Forms.CheckBox cbSemantria;
		private System.Windows.Forms.Button btProcess;
		private System.Windows.Forms.GroupBox gbChatterbox;
		private System.Windows.Forms.CheckBox cbChatterbox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ProgressBar pbChatterbox;
		private System.Windows.Forms.TextBox tbChatterboxKey;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox tbAlchemyKey;
		private System.Windows.Forms.ProgressBar pbAlchemy;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox cbAlchemy;
		private System.Windows.Forms.GroupBox gpAlchemy;
		private System.Windows.Forms.LinkLabel llSemantria;
		private System.Windows.Forms.LinkLabel llAlchemy;
		private System.Windows.Forms.LinkLabel llChatterbox;
		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.GroupBox gbViralheat;
		private System.Windows.Forms.LinkLabel llViralheat;
		private System.Windows.Forms.CheckBox cbViralheat;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ProgressBar pbViralheat;
		private System.Windows.Forms.TextBox tbViralheatKey;

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SemantAPIRobot));
			this.gpSemantria = new System.Windows.Forms.GroupBox();
			this.llSemantria = new System.Windows.Forms.LinkLabel();
			this.cbSemantria = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbSemantriaSecret = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.pbSemantria = new System.Windows.Forms.ProgressBar();
			this.tbSemantriaKey = new System.Windows.Forms.TextBox();
			this.btProcess = new System.Windows.Forms.Button();
			this.gbChatterbox = new System.Windows.Forms.GroupBox();
			this.llChatterbox = new System.Windows.Forms.LinkLabel();
			this.cbChatterbox = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.pbChatterbox = new System.Windows.Forms.ProgressBar();
			this.tbChatterboxKey = new System.Windows.Forms.TextBox();
			this.tbAlchemyKey = new System.Windows.Forms.TextBox();
			this.pbAlchemy = new System.Windows.Forms.ProgressBar();
			this.label9 = new System.Windows.Forms.Label();
			this.cbAlchemy = new System.Windows.Forms.CheckBox();
			this.gpAlchemy = new System.Windows.Forms.GroupBox();
			this.llAlchemy = new System.Windows.Forms.LinkLabel();
			this.btClose = new System.Windows.Forms.Button();
			this.gbViralheat = new System.Windows.Forms.GroupBox();
			this.llViralheat = new System.Windows.Forms.LinkLabel();
			this.cbViralheat = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.pbViralheat = new System.Windows.Forms.ProgressBar();
			this.tbViralheatKey = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbLanguage = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbSource = new System.Windows.Forms.TextBox();
			this.btnSource = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.tbOutput = new System.Windows.Forms.TextBox();
			this.btnOutput = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tbDocSize = new System.Windows.Forms.TextBox();
			this.gbSettings = new System.Windows.Forms.GroupBox();
			this.gbBitext = new System.Windows.Forms.GroupBox();
			this.llBitext = new System.Windows.Forms.LinkLabel();
			this.cbBitext = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbBitextPassword = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.pbBitext = new System.Windows.Forms.ProgressBar();
			this.tbBitextLogin = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btAbout = new System.Windows.Forms.Button();
			this.gpSemantria.SuspendLayout();
			this.gbChatterbox.SuspendLayout();
			this.gpAlchemy.SuspendLayout();
			this.gbViralheat.SuspendLayout();
			this.gbSettings.SuspendLayout();
			this.gbBitext.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// gpSemantria
			// 
			this.gpSemantria.Controls.Add(this.llSemantria);
			this.gpSemantria.Controls.Add(this.cbSemantria);
			this.gpSemantria.Controls.Add(this.label7);
			this.gpSemantria.Controls.Add(this.tbSemantriaSecret);
			this.gpSemantria.Controls.Add(this.label4);
			this.gpSemantria.Controls.Add(this.pbSemantria);
			this.gpSemantria.Controls.Add(this.tbSemantriaKey);
			this.gpSemantria.Location = new System.Drawing.Point(14, 154);
			this.gpSemantria.Name = "gpSemantria";
			this.gpSemantria.Size = new System.Drawing.Size(502, 80);
			this.gpSemantria.TabIndex = 0;
			this.gpSemantria.TabStop = false;
			this.gpSemantria.Text = "Semantria (  www.semantria.com  )";
			// 
			// llSemantria
			// 
			this.llSemantria.AutoSize = true;
			this.llSemantria.Location = new System.Drawing.Point(65, 0);
			this.llSemantria.Name = "llSemantria";
			this.llSemantria.Size = new System.Drawing.Size(102, 13);
			this.llSemantria.TabIndex = 0;
			this.llSemantria.TabStop = true;
			this.llSemantria.Text = "www.semantria.com";
			this.llSemantria.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llSemantria_LinkClicked);
			// 
			// cbSemantria
			// 
			this.cbSemantria.AutoSize = true;
			this.cbSemantria.Location = new System.Drawing.Point(11, 20);
			this.cbSemantria.Name = "cbSemantria";
			this.cbSemantria.Size = new System.Drawing.Size(61, 17);
			this.cbSemantria.TabIndex = 5;
			this.cbSemantria.Text = "Include";
			this.cbSemantria.UseVisualStyleBackColor = true;
			this.cbSemantria.CheckedChanged += new System.EventHandler(this.cbSemantria_CheckedChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(300, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(38, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Secret";
			// 
			// tbSemantriaSecret
			// 
			this.tbSemantriaSecret.Enabled = false;
			this.tbSemantriaSecret.Location = new System.Drawing.Point(345, 17);
			this.tbSemantriaSecret.Name = "tbSemantriaSecret";
			this.tbSemantriaSecret.Size = new System.Drawing.Size(148, 20);
			this.tbSemantriaSecret.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(93, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(25, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Key";
			// 
			// pbSemantria
			// 
			this.pbSemantria.Location = new System.Drawing.Point(11, 45);
			this.pbSemantria.Name = "pbSemantria";
			this.pbSemantria.Size = new System.Drawing.Size(482, 23);
			this.pbSemantria.TabIndex = 0;
			// 
			// tbSemantriaKey
			// 
			this.tbSemantriaKey.Enabled = false;
			this.tbSemantriaKey.Location = new System.Drawing.Point(124, 17);
			this.tbSemantriaKey.Name = "tbSemantriaKey";
			this.tbSemantriaKey.Size = new System.Drawing.Size(148, 20);
			this.tbSemantriaKey.TabIndex = 6;
			// 
			// btProcess
			// 
			this.btProcess.Location = new System.Drawing.Point(865, 332);
			this.btProcess.Name = "btProcess";
			this.btProcess.Size = new System.Drawing.Size(75, 23);
			this.btProcess.TabIndex = 14;
			this.btProcess.Text = "Process";
			this.btProcess.UseVisualStyleBackColor = true;
			this.btProcess.Click += new System.EventHandler(this.btProcess_Click);
			// 
			// gbChatterbox
			// 
			this.gbChatterbox.Controls.Add(this.llChatterbox);
			this.gbChatterbox.Controls.Add(this.cbChatterbox);
			this.gbChatterbox.Controls.Add(this.label11);
			this.gbChatterbox.Controls.Add(this.pbChatterbox);
			this.gbChatterbox.Controls.Add(this.tbChatterboxKey);
			this.gbChatterbox.Location = new System.Drawing.Point(530, 68);
			this.gbChatterbox.Name = "gbChatterbox";
			this.gbChatterbox.Size = new System.Drawing.Size(502, 80);
			this.gbChatterbox.TabIndex = 0;
			this.gbChatterbox.TabStop = false;
			this.gbChatterbox.Text = "Chatterbox (  www.chatterbox.co  )";
			// 
			// llChatterbox
			// 
			this.llChatterbox.AutoSize = true;
			this.llChatterbox.Location = new System.Drawing.Point(68, 0);
			this.llChatterbox.Name = "llChatterbox";
			this.llChatterbox.Size = new System.Drawing.Size(99, 13);
			this.llChatterbox.TabIndex = 0;
			this.llChatterbox.TabStop = true;
			this.llChatterbox.Text = "www.chatterbox.co";
			this.llChatterbox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llChatterbox_LinkClicked);
			// 
			// cbChatterbox
			// 
			this.cbChatterbox.AutoSize = true;
			this.cbChatterbox.Location = new System.Drawing.Point(9, 20);
			this.cbChatterbox.Name = "cbChatterbox";
			this.cbChatterbox.Size = new System.Drawing.Size(61, 17);
			this.cbChatterbox.TabIndex = 10;
			this.cbChatterbox.Text = "Include";
			this.cbChatterbox.UseVisualStyleBackColor = true;
			this.cbChatterbox.CheckedChanged += new System.EventHandler(this.cbChatterbox_CheckedChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(93, 21);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(25, 13);
			this.label11.TabIndex = 0;
			this.label11.Text = "Key";
			// 
			// pbChatterbox
			// 
			this.pbChatterbox.Location = new System.Drawing.Point(9, 45);
			this.pbChatterbox.Name = "pbChatterbox";
			this.pbChatterbox.Size = new System.Drawing.Size(482, 23);
			this.pbChatterbox.TabIndex = 0;
			// 
			// tbChatterboxKey
			// 
			this.tbChatterboxKey.Enabled = false;
			this.tbChatterboxKey.Location = new System.Drawing.Point(124, 17);
			this.tbChatterboxKey.Name = "tbChatterboxKey";
			this.tbChatterboxKey.Size = new System.Drawing.Size(367, 20);
			this.tbChatterboxKey.TabIndex = 11;
			// 
			// tbAlchemyKey
			// 
			this.tbAlchemyKey.Enabled = false;
			this.tbAlchemyKey.Location = new System.Drawing.Point(124, 17);
			this.tbAlchemyKey.Name = "tbAlchemyKey";
			this.tbAlchemyKey.Size = new System.Drawing.Size(367, 20);
			this.tbAlchemyKey.TabIndex = 9;
			// 
			// pbAlchemy
			// 
			this.pbAlchemy.Location = new System.Drawing.Point(9, 45);
			this.pbAlchemy.Name = "pbAlchemy";
			this.pbAlchemy.Size = new System.Drawing.Size(482, 23);
			this.pbAlchemy.TabIndex = 0;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(93, 21);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(25, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "Key";
			// 
			// cbAlchemy
			// 
			this.cbAlchemy.AutoSize = true;
			this.cbAlchemy.Location = new System.Drawing.Point(9, 20);
			this.cbAlchemy.Name = "cbAlchemy";
			this.cbAlchemy.Size = new System.Drawing.Size(61, 17);
			this.cbAlchemy.TabIndex = 8;
			this.cbAlchemy.Text = "Include";
			this.cbAlchemy.UseVisualStyleBackColor = true;
			this.cbAlchemy.CheckedChanged += new System.EventHandler(this.cbAlchemy_CheckedChanged);
			// 
			// gpAlchemy
			// 
			this.gpAlchemy.Controls.Add(this.llAlchemy);
			this.gpAlchemy.Controls.Add(this.cbAlchemy);
			this.gpAlchemy.Controls.Add(this.label9);
			this.gpAlchemy.Controls.Add(this.pbAlchemy);
			this.gpAlchemy.Controls.Add(this.tbAlchemyKey);
			this.gpAlchemy.Location = new System.Drawing.Point(14, 240);
			this.gpAlchemy.Name = "gpAlchemy";
			this.gpAlchemy.Size = new System.Drawing.Size(502, 80);
			this.gpAlchemy.TabIndex = 0;
			this.gpAlchemy.TabStop = false;
			this.gpAlchemy.Text = "AlchemyAPI (  www.alchemyapi.com  )";
			// 
			// llAlchemy
			// 
			this.llAlchemy.AutoSize = true;
			this.llAlchemy.Location = new System.Drawing.Point(75, 0);
			this.llAlchemy.Name = "llAlchemy";
			this.llAlchemy.Size = new System.Drawing.Size(110, 13);
			this.llAlchemy.TabIndex = 0;
			this.llAlchemy.TabStop = true;
			this.llAlchemy.Text = "www.alchemyapi.com";
			this.llAlchemy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAlchemy_LinkClicked);
			// 
			// btClose
			// 
			this.btClose.Location = new System.Drawing.Point(946, 332);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(75, 23);
			this.btClose.TabIndex = 15;
			this.btClose.Text = "Close";
			this.btClose.UseVisualStyleBackColor = true;
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// gbViralheat
			// 
			this.gbViralheat.Controls.Add(this.llViralheat);
			this.gbViralheat.Controls.Add(this.cbViralheat);
			this.gbViralheat.Controls.Add(this.label3);
			this.gbViralheat.Controls.Add(this.pbViralheat);
			this.gbViralheat.Controls.Add(this.tbViralheatKey);
			this.gbViralheat.Location = new System.Drawing.Point(530, 154);
			this.gbViralheat.Name = "gbViralheat";
			this.gbViralheat.Size = new System.Drawing.Size(502, 80);
			this.gbViralheat.TabIndex = 0;
			this.gbViralheat.TabStop = false;
			this.gbViralheat.Text = "Viralheat (  www.viralheat.com  )";
			// 
			// llViralheat
			// 
			this.llViralheat.AutoSize = true;
			this.llViralheat.Location = new System.Drawing.Point(60, 0);
			this.llViralheat.Name = "llViralheat";
			this.llViralheat.Size = new System.Drawing.Size(97, 13);
			this.llViralheat.TabIndex = 0;
			this.llViralheat.TabStop = true;
			this.llViralheat.Text = "www.viralheat.com";
			this.llViralheat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llViralheat_LinkClicked);
			// 
			// cbViralheat
			// 
			this.cbViralheat.AutoSize = true;
			this.cbViralheat.Location = new System.Drawing.Point(9, 20);
			this.cbViralheat.Name = "cbViralheat";
			this.cbViralheat.Size = new System.Drawing.Size(61, 17);
			this.cbViralheat.TabIndex = 12;
			this.cbViralheat.Text = "Include";
			this.cbViralheat.UseVisualStyleBackColor = true;
			this.cbViralheat.CheckedChanged += new System.EventHandler(this.cbViralheat_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(93, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(25, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Key";
			// 
			// pbViralheat
			// 
			this.pbViralheat.Location = new System.Drawing.Point(9, 45);
			this.pbViralheat.Name = "pbViralheat";
			this.pbViralheat.Size = new System.Drawing.Size(482, 23);
			this.pbViralheat.TabIndex = 0;
			// 
			// tbViralheatKey
			// 
			this.tbViralheatKey.Enabled = false;
			this.tbViralheatKey.Location = new System.Drawing.Point(124, 17);
			this.tbViralheatKey.Name = "tbViralheatKey";
			this.tbViralheatKey.Size = new System.Drawing.Size(367, 20);
			this.tbViralheatKey.TabIndex = 13;
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
			this.cbLanguage.Location = new System.Drawing.Point(86, 17);
			this.cbLanguage.Name = "cbLanguage";
			this.cbLanguage.Size = new System.Drawing.Size(121, 21);
			this.cbLanguage.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(213, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Source file";
			// 
			// tbSource
			// 
			this.tbSource.Enabled = false;
			this.tbSource.Location = new System.Drawing.Point(277, 17);
			this.tbSource.Name = "tbSource";
			this.tbSource.Size = new System.Drawing.Size(133, 20);
			this.tbSource.TabIndex = 8;
			// 
			// btnSource
			// 
			this.btnSource.Location = new System.Drawing.Point(416, 15);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(75, 23);
			this.btnSource.TabIndex = 10;
			this.btnSource.Text = "Browse";
			this.btnSource.UseVisualStyleBackColor = true;
			this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(213, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Output file";
			// 
			// tbOutput
			// 
			this.tbOutput.Enabled = false;
			this.tbOutput.Location = new System.Drawing.Point(277, 47);
			this.tbOutput.Name = "tbOutput";
			this.tbOutput.Size = new System.Drawing.Size(133, 20);
			this.tbOutput.TabIndex = 9;
			// 
			// btnOutput
			// 
			this.btnOutput.Location = new System.Drawing.Point(416, 45);
			this.btnOutput.Name = "btnOutput";
			this.btnOutput.Size = new System.Drawing.Size(75, 23);
			this.btnOutput.TabIndex = 11;
			this.btnOutput.Text = "Browse";
			this.btnOutput.UseVisualStyleBackColor = true;
			this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(7, 50);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76, 13);
			this.label6.TabIndex = 3;
			this.label6.Text = "Shorten text to";
			// 
			// tbDocSize
			// 
			this.tbDocSize.Location = new System.Drawing.Point(86, 47);
			this.tbDocSize.Name = "tbDocSize";
			this.tbDocSize.Size = new System.Drawing.Size(121, 20);
			this.tbDocSize.TabIndex = 5;
			this.tbDocSize.Text = "300";
			this.tbDocSize.TextChanged += new System.EventHandler(this.tbDocSize_TextChanged);
			this.tbDocSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbDocSize_KeyPress);
			// 
			// gbSettings
			// 
			this.gbSettings.Controls.Add(this.tbDocSize);
			this.gbSettings.Controls.Add(this.label6);
			this.gbSettings.Controls.Add(this.btnOutput);
			this.gbSettings.Controls.Add(this.tbOutput);
			this.gbSettings.Controls.Add(this.label5);
			this.gbSettings.Controls.Add(this.btnSource);
			this.gbSettings.Controls.Add(this.tbSource);
			this.gbSettings.Controls.Add(this.label2);
			this.gbSettings.Controls.Add(this.cbLanguage);
			this.gbSettings.Controls.Add(this.label1);
			this.gbSettings.Location = new System.Drawing.Point(14, 68);
			this.gbSettings.Name = "gbSettings";
			this.gbSettings.Size = new System.Drawing.Size(502, 80);
			this.gbSettings.TabIndex = 1;
			this.gbSettings.TabStop = false;
			this.gbSettings.Text = "Settings";
			// 
			// gbBitext
			// 
			this.gbBitext.Controls.Add(this.llBitext);
			this.gbBitext.Controls.Add(this.cbBitext);
			this.gbBitext.Controls.Add(this.label8);
			this.gbBitext.Controls.Add(this.tbBitextPassword);
			this.gbBitext.Controls.Add(this.label10);
			this.gbBitext.Controls.Add(this.pbBitext);
			this.gbBitext.Controls.Add(this.tbBitextLogin);
			this.gbBitext.Location = new System.Drawing.Point(530, 240);
			this.gbBitext.Name = "gbBitext";
			this.gbBitext.Size = new System.Drawing.Size(502, 80);
			this.gbBitext.TabIndex = 16;
			this.gbBitext.TabStop = false;
			this.gbBitext.Text = "Bitext (  www.bitext.com  )";
			// 
			// llBitext
			// 
			this.llBitext.AutoSize = true;
			this.llBitext.Location = new System.Drawing.Point(45, 0);
			this.llBitext.Name = "llBitext";
			this.llBitext.Size = new System.Drawing.Size(82, 13);
			this.llBitext.TabIndex = 0;
			this.llBitext.TabStop = true;
			this.llBitext.Text = "www.bitext.com";
			this.llBitext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llBitext_LinkClicked);
			// 
			// cbBitext
			// 
			this.cbBitext.AutoSize = true;
			this.cbBitext.Location = new System.Drawing.Point(11, 20);
			this.cbBitext.Name = "cbBitext";
			this.cbBitext.Size = new System.Drawing.Size(61, 17);
			this.cbBitext.TabIndex = 5;
			this.cbBitext.Text = "Include";
			this.cbBitext.UseVisualStyleBackColor = true;
			this.cbBitext.CheckedChanged += new System.EventHandler(this.cbBitext_CheckedChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(286, 21);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(53, 13);
			this.label8.TabIndex = 0;
			this.label8.Text = "Password";
			// 
			// tbBitextPassword
			// 
			this.tbBitextPassword.Enabled = false;
			this.tbBitextPassword.Location = new System.Drawing.Point(345, 17);
			this.tbBitextPassword.Name = "tbBitextPassword";
			this.tbBitextPassword.Size = new System.Drawing.Size(148, 20);
			this.tbBitextPassword.TabIndex = 7;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(89, 21);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(29, 13);
			this.label10.TabIndex = 0;
			this.label10.Text = "User";
			// 
			// pbBitext
			// 
			this.pbBitext.Location = new System.Drawing.Point(11, 45);
			this.pbBitext.Name = "pbBitext";
			this.pbBitext.Size = new System.Drawing.Size(482, 23);
			this.pbBitext.TabIndex = 0;
			// 
			// tbBitextLogin
			// 
			this.tbBitextLogin.Enabled = false;
			this.tbBitextLogin.Location = new System.Drawing.Point(124, 17);
			this.tbBitextLogin.Name = "tbBitextLogin";
			this.tbBitextLogin.Size = new System.Drawing.Size(148, 20);
			this.tbBitextLogin.TabIndex = 6;
			// 
			// pictureBox1
			// 
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.Image = global::SemantAPI.Robot.Properties.Resources.main_logo_png;
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(13, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(1018, 50);
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			// 
			// btAbout
			// 
			this.btAbout.Location = new System.Drawing.Point(14, 332);
			this.btAbout.Name = "btAbout";
			this.btAbout.Size = new System.Drawing.Size(75, 23);
			this.btAbout.TabIndex = 17;
			this.btAbout.Text = "About";
			this.btAbout.UseVisualStyleBackColor = true;
			this.btAbout.Click += new System.EventHandler(this.btAbout_Click);
			// 
			// SemantAPIRobot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(1044, 367);
			this.Controls.Add(this.btAbout);
			this.Controls.Add(this.gbBitext);
			this.Controls.Add(this.gbViralheat);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.gbChatterbox);
			this.Controls.Add(this.gpAlchemy);
			this.Controls.Add(this.btProcess);
			this.Controls.Add(this.gpSemantria);
			this.Controls.Add(this.gbSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SemantAPIRobot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SemantAPI";
			this.gpSemantria.ResumeLayout(false);
			this.gpSemantria.PerformLayout();
			this.gbChatterbox.ResumeLayout(false);
			this.gbChatterbox.PerformLayout();
			this.gpAlchemy.ResumeLayout(false);
			this.gpAlchemy.PerformLayout();
			this.gbViralheat.ResumeLayout(false);
			this.gbViralheat.PerformLayout();
			this.gbSettings.ResumeLayout(false);
			this.gbSettings.PerformLayout();
			this.gbBitext.ResumeLayout(false);
			this.gbBitext.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btAbout;
	}
}

