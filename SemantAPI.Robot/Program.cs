// <copyright file="Form.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>Main SemantAPI.Robot program class</summary>

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
					Console.WriteLine("\t-benchmark_Semantria\r\n\t-benchmark_Alchemy\r\n\t-benchmark_Chatterbox\r\n\t-benchmark_Viralheat\r\n\t-benchmark_Bitext\r\n");

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

			ThreadPool.SetMinThreads(5, 5);
			Application.Run(form);
		}
	}
}
