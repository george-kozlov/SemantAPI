// <copyright file="About.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>About form class</summary>

using System.Windows.Forms;

namespace SemantAPI.Robot
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
			lVersion.Text = string.Format("Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}

		private void llEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llEmail.LinkVisited = true;
			System.Diagnostics.Process.Start("mailto:george.kozlov@outlook.com");
		}
	}
}
