// <copyright file="Program.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>Main AnalysisComparer program class</summary>

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
