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

namespace SemantAPI.Robot
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
			SemantAPIRobot form = null;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (args.Length > 0)
			{
				if (args.Contains("-debug"))
				{
					AllocConsole();
					Console.WriteLine("Greetings from SemantAPI.Robot application!");
					Console.WriteLine("SemantAPI.Robot supports benchmark mode for all integrated servises.");
					Console.WriteLine("The command line arguments are following:");
                    Console.WriteLine("\t-benchmark_Semantria\r\n\t-benchmark_Alchemy\r\n\t-benchmark_Repustate\r\n\t-benchmark_Chatterbox\r\n\t-benchmark_Viralheat\r\n\t-benchmark_Bitext\r\n\t-benchmark_Skyttle\r\n");

					form = new SemantAPIRobot(true);
					List<string> services = new List<string>();

					if (args.Any(item => item.IndexOf("-benchmark") > -1))
					{
						IEnumerable<string> list = args.Where(item => item.Contains("-benchmark"));
						foreach (string service in list)
						{
							string name = service.Substring(service.IndexOf('_') + 1);
							services.Add(name);
						}
					}

					form.Benchmark = services;
				}
			}
			else
				form = new SemantAPIRobot(false);

			ThreadPool.SetMinThreads(7, 7);
			Application.Run(form);
		}
	}
}
