//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

using System.Windows.Forms;

namespace SemantAPI.Human
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
