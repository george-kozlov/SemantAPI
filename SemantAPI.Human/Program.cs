//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace SemantAPI.Human
{
	static class Program
	{

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			SemantAPIHuman form = null;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (args.Length > 0)
			{
				if (args.Contains("-debug"))
				{
					AllocConsole();
					Console.WriteLine("Greetings from SemantAPI.Human application!");
					Console.WriteLine("SemantAPI.Human supports benchmark mode. The command line arguments are following:");
					Console.WriteLine("\t-benchmark");

					form = new SemantAPIHuman(true);
					if (args.Contains("-benchmark"))
						form.Benchmark = true;
				}
			}
			else
				form = new SemantAPIHuman(false);

			Application.Run(form);
		}
	}
}
