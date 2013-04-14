namespace SemantAPI.Robot
{
	partial class About
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.llEmail = new System.Windows.Forms.LinkLabel();
			this.lVersion = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::SemantAPI.Robot.Properties.Resources.about_logo;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(300, 260);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 280);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(293, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Copyright © George Kozlov (  george.kozlov@outlook.com  )";
			// 
			// llEmail
			// 
			this.llEmail.AutoSize = true;
			this.llEmail.Location = new System.Drawing.Point(159, 280);
			this.llEmail.Name = "llEmail";
			this.llEmail.Size = new System.Drawing.Size(143, 13);
			this.llEmail.TabIndex = 2;
			this.llEmail.TabStop = true;
			this.llEmail.Text = "george.kozlov@outlook.com";
			this.llEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llEmail_LinkClicked);
			// 
			// lVersion
			// 
			this.lVersion.AutoSize = true;
			this.lVersion.Location = new System.Drawing.Point(210, 20);
			this.lVersion.Name = "lVersion";
			this.lVersion.Size = new System.Drawing.Size(84, 13);
			this.lVersion.TabIndex = 3;
			this.lVersion.Text = "Version 1.0.1.69";
			// 
			// About
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(324, 302);
			this.Controls.Add(this.lVersion);
			this.Controls.Add(this.llEmail);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About SemantAPI.Robot";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel llEmail;
		private System.Windows.Forms.Label lVersion;
	}
}